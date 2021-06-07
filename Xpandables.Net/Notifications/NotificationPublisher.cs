
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Handlers;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// The default implementation of <see cref="INotificationPublisher"/>.
    /// You can derive from this class in order to customize its behaviors.
    /// </summary>
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Constructs a new instance of <see cref="NotificationPublisher"/>.
        /// </summary>
        /// <param name="handlerAccessor">The handler accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public NotificationPublisher(IHandlerAccessor handlerAccessor)
        {
            _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));
        }

        ///<inheritdoc/>
        public virtual async Task PublishAsync(ICommandQueryEvent @event, CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var handlers = GetNotificationHandlers(@event);
            if (!handlers.Any()) return;

            var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            // TODO : think about thread pool
            foreach (var result in results.Where(r => r.HasValue))
            {
                if (result.Value is ICommand command)
                {
                    var handler = TryGetCommandHandler(command);
                    if (handler is not null)
                        await handler.HandleAsync((dynamic)command, (dynamic)cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private IEnumerable<INotificationHandler> GetNotificationHandlers(ICommandQueryEvent @event)
        {
            var genericInterfaceType = @event.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(INotification<>) || i.GetGenericTypeDefinition() == typeof(INotification<,>)))
                ?? throw new ArgumentException($"The type '{@event.GetType().Name}' must implement one of '{typeof(INotification<>).Name}' interfaces.");

            var aggregateIdType = genericInterfaceType.GetGenericArguments()[0];

            var types = genericInterfaceType.GetGenericTypeDefinition() == typeof(INotification<>)
                ? new[] { aggregateIdType, @event.GetType() }
                : new[] { aggregateIdType, genericInterfaceType.GetGenericArguments()[1], @event.GetType() };

            var genericHandlerType = genericInterfaceType == typeof(INotification<>) ? typeof(INotificationHandler<,>) : typeof(INotificationHandler<,,>);

            if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out var typeException, aggregateIdType, @event.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building notification Handler type failed.", typeException));
                return Enumerable.Empty<INotificationHandler>();
            }

            if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching notification handlers for {@event.GetType().Name} are missing.", ex));
                return Enumerable.Empty<INotificationHandler>();
            }

            return (IEnumerable<INotificationHandler>)foundHandlers;
        }

        private dynamic? TryGetCommandHandler(ICommand command)
        {
            var genericHandlerType = typeof(ICommandHandler<>);
            if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out _, command.GetType()))
                return default;

            if (!_handlerAccessor.TryGetHandler(typeHandler, out var foundHandler, out _))
                return default;

            return foundHandler;
        }

        private static void WriteLineException(Exception exception)
        {
#if DEBUG
            Debug.WriteLine(exception);
#endif
        }
    }
}
