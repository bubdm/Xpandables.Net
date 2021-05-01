
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The default implementation of <see cref="IAggregateRootAccessor{TAggregateRoot}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the target aggregate.</typeparam>
    public class AggregateRootAccessor<TAggregateRoot> : OperationResultBase, IAggregateRootAccessor<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot, new()
    {
        private readonly IEventStore _eventStore;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        private Exception? instanceCreatorException;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateRootAccessor{TAggregateRoot}"/>.
        /// </summary>
        /// <param name="eventStore">The target event store.</param>
        /// <param name="eventPublisher">The target event publisher.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventStore"/> or <paramref name="eventPublisher"/> or <paramref name="instanceCreator"/> is null.</exception>
        public AggregateRootAccessor(IEventStore eventStore, IDomainEventPublisher eventPublisher, IInstanceCreator instanceCreator)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _instanceCreator.OnException = dispatchException => instanceCreatorException = dispatchException.SourceException;
        }

        /// <summary>
        /// Asynchronously appends the specified <typeparamref name="TAggregateRoot"/> aggregate to the event store.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        public async Task<IOperationResult> AppendAsync(TAggregateRoot aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            foreach (var @event in aggregate.GetUncommittedEvents())
            {
                var appendresult = await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                if (appendresult.Failed)
                    return appendresult;

                await _eventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            aggregate.MarkEventsAsCommitted();

            return OkOperation();
        }

        /// <summary>
        /// Asynchronously returns the <typeparamref name="TAggregateRoot"/> aggregate that matches the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TAggregateRoot>> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            instanceCreatorException = default;
            if (_instanceCreator.Create(typeof(TAggregateRoot)) is not TAggregateRoot aggregate)
            {
                if (instanceCreatorException is not null)
                    return BadOperation<TAggregateRoot>(new OperationError(typeof(TAggregateRoot).Name, instanceCreatorException));
                return
                    BadOperation<TAggregateRoot>(typeof(TAggregateRoot).Name, "Unable to create a new instance.");
            }

            var eventCount = 0;
            await foreach (var @event in _eventStore.ReadEventsAsync(aggregateId, cancellationToken))
            {
                aggregate.Apply(@event);
                eventCount++;
            }

            if (eventCount <= 0)
                return NotFoundOperation<TAggregateRoot>(typeof(TAggregateRoot).Name, "Event not found.");

            return OkOperation(aggregate);
        }
    }
}
