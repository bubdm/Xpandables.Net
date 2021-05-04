
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

namespace Xpandables.Net.Events.DomainEvents
{
    /// <summary>
    /// The domain event publisher.
    /// </summary>
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventPublisher"/> class with the handlers provider.
        /// </summary>
        /// <param name="handlerAccessor">The handlers provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public DomainEventPublisher(IHandlerAccessor handlerAccessor)
            => _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));

        /// <summary>
        /// Publishes the specified domain event.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public virtual async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var genericHandlerType = typeof(IDomainEventHandler<>);

            if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out var typeException, @event.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building domain event Handler type failed.", typeException));
                return;
            }

            if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching domain event handlers for {@event.GetType().Name} are missing.", ex));
                return;
            }

            if (!foundHandlers.Any())
                return;

            var handlers = (IEnumerable<IDomainEventHandler>)foundHandlers;
            var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

            await Task.WhenAll(tasks).ConfigureAwait(false);
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
