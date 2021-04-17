
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

using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;
using Xpandables.Net.Handlers;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// /// The base implementation for an event publisher
    /// </summary>
    public abstract class EventPublisher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class with the handlers provider.
        /// </summary>
        /// <param name="handlerAccessor">The handlers provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        protected EventPublisher(IHandlerAccessor handlerAccessor)
            => _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));

        /// <summary>
        /// Asynchronously publishes the events across all domain/integration handlers.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event" /> is null.</exception>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            Type genericHandlerType;
            if (@event is IDomainEvent)
                genericHandlerType = typeof(IDomainEventHandler<>);
            else if (@event is IIntegrationEvent)
                genericHandlerType = typeof(IIntegrationEventHandler<>);
            else
                return;

            if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out var typeException, @event.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building event Handler type failed.", typeException));
                return;
            }

            if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching event handlers for {@event.GetType().Name} are missing.", ex));
                return;
            }

            IEnumerable<Task<IOperationResult>> tasks;
            if (@event is IDomainEvent)
            {
                var handlers = (IEnumerable<IDomainEventHandler>)foundHandlers;
                tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));
            }
            else
            {
                var handlers = (IEnumerable<IIntegrationEventHandler>)foundHandlers;
                tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            if (results.Any(result => result.Failed))
                @event.PublishingStatusFailed();
            else
                @event.PublishingStatusPublished();
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
