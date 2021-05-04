
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

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The default implementation of <see cref="IIntegrationEventPublisher"/>.
    /// You can derive from this class in order to customize its behaviors.
    /// </summary>
    public class IntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Constructs a new instance of <see cref="IntegrationEventPublisher"/>.
        /// </summary>
        /// <param name="handlerAccessor">The handler accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public IntegrationEventPublisher(IHandlerAccessor handlerAccessor)
        {
            _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));
        }

        /// <summary>
        /// Persists the target event to the context.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public virtual async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var handlers = GetIntegrationEventHandlers(@event);
            if (!handlers.Any()) return;

            var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            var errorResults = results.Where(result => result.Failed);

            // TODO : think about thread pool
            foreach(var error in errorResults)
            {
                var correctiveCommand = error.Value;
                var handler = TryGetCommandHandler(correctiveCommand);
                if (handler is not null)
                    await handler.HandleAsync((dynamic)correctiveCommand, (dynamic)cancellationToken).ConfigureAwait(false);
            }
        }

        private IEnumerable<IIntegrationEventHandler> GetIntegrationEventHandlers(IIntegrationEvent @event)
        {
            var genericHandlerType = typeof(IIntegrationEventHandler<>);
            if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out var typeException, @event.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building integration event Handler type failed.", typeException));
                return Enumerable.Empty<IIntegrationEventHandler>();
            }

            if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching integration event handlers for {@event.GetType().Name} are missing.", ex));
                return Enumerable.Empty<IIntegrationEventHandler>();
            }

            return (IEnumerable<IIntegrationEventHandler>)foundHandlers;
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
#else
            Trace.WriteLine(exception);
#endif
        }
    }
}
