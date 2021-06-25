
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
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The default implementation of <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class AggregateAccessor<TAggregateId, TAggregate> : OperationResults, IAggregateAccessor<TAggregateId, TAggregate>
        where TAggregate : class, IAggregate<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        private readonly IDomainEventAccessor<TAggregateId, TAggregate> _domainEventAccessor;
        private readonly INotificationEventAccessor<TAggregateId, TAggregate> _notificationEventAccessor;
        private readonly ISnapShotAccessor<TAggregateId, TAggregate> _snapShotAccessor;

        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="domainEventAccessor">The target domain event accessor.</param>
        /// <param name="notificationEventAccessor">The  target notification event accessor.</param>
        /// <param name="snapShotAccessor">The target snapShot accessor.</param>
        /// <param name="domainEventPublisher">The target domain event publisher.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventAccessor"/> is null.</exception>
        public AggregateAccessor(
            IDomainEventAccessor<TAggregateId, TAggregate> domainEventAccessor,
            INotificationEventAccessor<TAggregateId, TAggregate> notificationEventAccessor,
            ISnapShotAccessor<TAggregateId, TAggregate> snapShotAccessor,
            IDomainEventPublisher domainEventPublisher,
            IInstanceCreator instanceCreator)
        {
            _domainEventAccessor = domainEventAccessor ?? throw new ArgumentNullException(nameof(domainEventAccessor));
            _notificationEventAccessor = notificationEventAccessor ?? throw new ArgumentNullException(nameof(notificationEventAccessor));
            _snapShotAccessor = snapShotAccessor ?? throw new ArgumentNullException(nameof(snapShotAccessor));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IDomainEventSourcing<TAggregateId> aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(Aggregate<TAggregateId>)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                await _domainEventAccessor.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing<TAggregateId> aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    await _notificationEventAccessor.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                }

                aggregateOutbox.MarkNotificationsAsCommitted();
            }

            aggregateEventSourcing.MarkEventsAsCommitted();
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync(
            TAggregateId aggregateId,
            CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IDomainEventSourcing<TAggregateId> aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} " +
                    $"must implement {nameof(Aggregate<TAggregateId>)}");

            await foreach (var @event in _domainEventAccessor.ReadAllDomainEventsAsync(
                new() { AggregateId = aggregateId },
                cancellationToken))
            {
                aggregateEventSourcing.LoadFromHistory(@event);
            }

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllDomainEventsAsync(
             EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            => _domainEventAccessor
                .ReadAllDomainEventsAsync(criteria, cancellationToken);

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(
            IDomainEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
            => await _domainEventAccessor
                .AppendEventAsync(@event, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<ISnapShot<TAggregateId>?> GetSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default)
            => await _snapShotAccessor
                .GetSnapShotAsync(aggreagteId, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadFromSnapShot(
            TAggregateId aggregateId,
            CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must " +
                    $"implement {typeof(IOriginator).Name}");

            var snapShot = await _snapShotAccessor
                    .GetSnapShotAsync(aggregateId, cancellationToken)
                    .ConfigureAwait(false);

            if (snapShot is not null)
                originator.SetMemento(snapShot.Memento);

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsSnapShotAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            await _snapShotAccessor.AppendAsSnapShotAsync(aggregate, cancellationToken);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadDomainEventsSinceLastSnapShotAsync(
            TAggregateId aggregateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var snapShot = await GetSnapShotAsync(aggregateId, cancellationToken)
                .ConfigureAwait(false);

            var snapShotVersion = snapShot?.Version ?? -1;

            var criteria = new EventStoreEntityCriteria
            {
                AggregateId = aggregateId,
                EventDataCriteria = data => data.RootElement.GetProperty("Version").GetInt64() > snapShotVersion
            };

            await foreach (var domainEvent in _domainEventAccessor
                .ReadAllDomainEventsAsync(criteria, cancellationToken).ConfigureAwait(false))
                yield return domainEvent;
        }


        ///<inheritdoc/>
        public virtual async Task<int> ReadEventCountSinceLastSnapShot(
            TAggregateId aggregateId,
            CancellationToken cancellationToken = default)
        {
            var snapShot = await GetSnapShotAsync(aggregateId, cancellationToken)
                .ConfigureAwait(false);
            var snapShotVersion = snapShot?.Version ?? 0;

            var criteria = new EventStoreEntityCriteria
            {
                AggregateId = aggregateId,
                EventDataCriteria = data => data.RootElement.GetProperty("Version").GetInt64() > snapShotVersion
            };

            return await _domainEventAccessor
                .CountEventsAsync(criteria, cancellationToken)
                .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountEventsAsync(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            => await _domainEventAccessor
                .CountEventsAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

        private TAggregate CreateInstance()
        {
            var instanceCreatorException = default(ExceptionDispatchInfo?);
            _instanceCreator.OnException = ex => instanceCreatorException = ex;
            if (_instanceCreator.Create(typeof(TAggregate)) is not TAggregate aggregate)
            {
                if (instanceCreatorException is not null)
                    instanceCreatorException.Throw();

                throw new InvalidOperationException($"{typeof(TAggregate).Name} Unable to create a new instance.");
            }

            return aggregate;
        }
    }
}
