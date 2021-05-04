
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
    /// The default implementation of <see cref="IAggregateAccessor{TAggregateRoot}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class AggregateAccessor<TAggregate> : OperationResultBase, IAggregateAccessor<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
        private readonly IEventStore _eventStore;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        private Exception? instanceCreatorException;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateAccessor{TAggregate}"/>.
        /// </summary>
        /// <param name="eventStore">The target event store.</param>
        /// <param name="domainEventPublisher">The target domain event publisher.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventStore"/> or <paramref name="domainEventPublisher"/> or <paramref name="instanceCreator"/> is null.</exception>
        public AggregateAccessor(
            IEventStore eventStore,
            IDomainEventPublisher domainEventPublisher,
            IInstanceCreator instanceCreator)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _instanceCreator.OnException = dispatchException => instanceCreatorException = dispatchException.SourceException;
        }

        /// <summary>
        /// Asynchronously appends the specified <typeparamref name="TAggregate"/> aggregate to the event store.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        public virtual async Task<IOperationResult> AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IAggregateEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(Aggregate)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                var appendResult = await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                if (appendResult.Failed)
                    return appendResult;

                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            aggregateEventSourcing.MarkEventsAsCommitted();

            if (aggregate is IAggregateOutbox aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetOutboxEvents())
                {
                    var appendResult = await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                    if (appendResult.Failed)
                        return appendResult;
                }

                aggregateOutbox.MarkEventsAsCommitted();
            }

            return OkOperation();
        }

        /// <summary>
        /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public virtual async Task<IOperationResult<TAggregate>> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            instanceCreatorException = default;
            if (_instanceCreator.Create(typeof(TAggregate)) is not TAggregate aggregate)
            {
                if (instanceCreatorException is not null)
                    return BadOperation<TAggregate>(new OperationError(typeof(TAggregate).Name, instanceCreatorException));
                return
                    BadOperation<TAggregate>(typeof(TAggregate).Name, "Unable to create a new instance.");
            }

            if (aggregate is not IAggregateEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(Aggregate)}");

            var eventCount = 0;
            await foreach (var @event in _eventStore.ReadDomainEventsAsync(aggregateId, cancellationToken))
            {
                aggregateEventSourcing.LoadFromHistory(@event);
                eventCount++;
            }

            if (eventCount <= 0)
                return NotFoundOperation<TAggregate>(typeof(TAggregate).Name, "Event not found.");

            return OkOperation(aggregate);
        }
    }
}
