
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

using Xpandables.Net.Handlers;

namespace Xpandables.Net.NotificationEvents
{
    /// <summary>
    /// The default implementation of <see cref="INotificationEventPublisher"/>.
    /// You can derive from this class in order to customize its behaviors.
    /// </summary>
    public class NotificationEventPublisher : INotificationEventPublisher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Constructs a new instance of <see cref="NotificationEventPublisher"/>.
        /// </summary>
        /// <param name="handlerAccessor">The handler accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public NotificationEventPublisher(IHandlerAccessor handlerAccessor)
        {
            _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));
        }

        ///<inheritdoc/>
        public virtual async Task PublishAsync(
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var handlers = GetNotificationHandlers(@event);
            if (!handlers.Any()) return;

            var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private IEnumerable<INotificationEventHandler> GetNotificationHandlers(IEvent @event)
        {
            var genericInterfaceType = @event.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(INotificationEvent<>)
                    || i.GetGenericTypeDefinition() == typeof(INotificationEvent<,>)))
                ?? throw new ArgumentException($"The type '{@event.GetType().Name}' must " +
                $"implement one of '{typeof(INotificationEvent<>).Name}' interfaces.");

            var aggregateIdType = genericInterfaceType.GetGenericArguments()[0];

            var types = genericInterfaceType.GetGenericTypeDefinition() == typeof(INotificationEvent<>)
                ? new[] { aggregateIdType, @event.GetType() }
                : new[] { aggregateIdType, genericInterfaceType.GetGenericArguments()[1], @event.GetType() };

            var genericHandlerType = genericInterfaceType.GetGenericTypeDefinition() == typeof(INotificationEvent<>)
                ? typeof(INotificationEventHandler<,>) : typeof(INotificationEventHandler<,,>);

            if (!genericHandlerType
                .TryMakeGenericType(out var typeHandler, out var typeException, aggregateIdType, @event.GetType()))
            {
                WriteLineException(
                    new InvalidOperationException("Building notification Handler type failed.", typeException));
                return Enumerable.Empty<INotificationEventHandler>();
            }

            if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching notification handlers " +
                    $"for {@event.GetType().Name} are missing.", ex));
                return Enumerable.Empty<INotificationEventHandler>();
            }

            return (IEnumerable<INotificationEventHandler>)foundHandlers;
        }

        private static void WriteLineException(Exception exception)
        {
#if DEBUG
            Debug.WriteLine(exception);
#endif
        }
    }
}
