
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// The default implementation of <see cref="IAggregateAccessor{TAggregateRoot}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class AggregateAccessor<TAggregate> : OperationResultBase, IAggregateAccessor<TAggregate>
        where TAggregate : class, IAggregate
    {
        private readonly IEventStore _eventStore;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IInstanceCreator _instanceCreator;

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
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(Aggregate)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            aggregateEventSourcing.MarkEventsAsCommitted();

            if (aggregate is INotificationSourcing aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetOutboxEvents())
                {
                    await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                }

                aggregateOutbox.MarkEventsAsCommitted();
            }
        }

        ///<inheritdoc/>
        public async Task<TAggregate?> ReadFromSnapShot(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must implement {typeof(IOriginator).Name}");

            var snapShot = await _eventStore.GetSnapShotAsync(aggregateId, cancellationToken).ConfigureAwait(false);
            if (snapShot is not null)
                originator.SetMemento(snapShot.Memento);

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(Aggregate)}");

            await foreach (var @event in _eventStore.ReadAllEventsAsync(aggregateId, cancellationToken))
            {
                aggregateEventSourcing.LoadFromHistory(@event);
            }

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public async Task AppendAsSnapShotAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            await _eventStore.AppendSnapShotAsync(aggregate, cancellationToken);
        }

        ///<inheritdoc/>
        public async Task<ISnapShot?> GetSnapShotAsync(Guid aggreagteId, CancellationToken cancellationToken = default)
            => await _eventStore.GetSnapShotAsync(aggreagteId, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public IAsyncEnumerable<IDomainEvent> ReadAllEventsAsync(Guid aggreagateId, CancellationToken cancellationToken = default)
            => _eventStore.ReadAllEventsAsync(aggreagateId, cancellationToken);

        ///<inheritdoc/>
        public IAsyncEnumerable<IDomainEvent> ReadEventsSinceLastSnapShotAsync(Guid aggregateId, CancellationToken cancellationToken = default)
            => _eventStore.ReadEventsSinceLastSnapShotAsync(aggregateId, cancellationToken);

        ///<inheritdoc/>
        public async Task<int> ReadEventCountSinceLastSnapShot(Guid aggregateId, CancellationToken cancellationToken = default)
            => await _eventStore.ReadEventCountSinceLastSnapShot(aggregateId, cancellationToken).ConfigureAwait(false);

        private TAggregate CreateInstance()
        {
            var instanceCreatorException = default(Exception?);
            _instanceCreator.OnException = ex => instanceCreatorException = ex.SourceException;
            if (_instanceCreator.Create(typeof(TAggregate)) is not TAggregate aggregate)
            {
                if (instanceCreatorException is not null)
                    instanceCreatorException.ReThrow();

                throw new InvalidOperationException($"{typeof(TAggregate).Name} Unable to create a new instance.");
            }

            return aggregate;
        }
    }
}
