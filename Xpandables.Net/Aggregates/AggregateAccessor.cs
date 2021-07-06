
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;
using Xpandables.Net.NotificationEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The implementation of <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class AggregateAccessor<TAggregateId, TAggregate>
        : OperationResults, IAggregateAccessor<TAggregateId, TAggregate>
        where TAggregate : class, IAggregate<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {

        private readonly IDomainEventDataContext _domainEventContext;
        private readonly INotificationEventDataContext _notificationEventContext;
        private readonly ISnapShotDataContext _snapShotDataContext;

        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="domainEventContext">The target domain event context.</param>
        /// <param name="notificationEventContext">The target notification event context.</param>
        /// <param name="snapShotDataContext">The target snapShot context.</param>
        /// <param name="domainEventPublisher">The target domain event publisher.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        public AggregateAccessor(
            IDomainEventDataContext domainEventContext,
            INotificationEventDataContext notificationEventContext,
            ISnapShotDataContext snapShotDataContext,
            IDomainEventPublisher domainEventPublisher,
            IInstanceCreator instanceCreator)
        {
            _domainEventContext = domainEventContext ?? throw new ArgumentNullException(nameof(domainEventContext));
            _notificationEventContext = notificationEventContext ?? throw new ArgumentNullException(nameof(notificationEventContext));
            _snapShotDataContext = snapShotDataContext ?? throw new ArgumentNullException(nameof(snapShotDataContext));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IDomainEventSourcing<TAggregateId> aggregateEventSourcing)
                throw new ArgumentException($"{typeof(TAggregate).Name} must " +
                    $"implement {nameof(IDomainEventSourcing<TAggregateId>)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event, SerializerOptions, DocumentOptions);

                await _domainEventContext.InsertAsync(entity, cancellationToken).ConfigureAwait(false);

                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing<TAggregateId> aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event, SerializerOptions, DocumentOptions);

                    await _notificationEventContext.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
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
                    $"must implement {nameof(IDomainEventSourcing<TAggregateId>)}");

            var criteria = new EventStoreEntityCriteria<EventStoreEntity>
            {
                AggregateId = aggregateId.AsString()
            };

            var domainEvents = ReadAllDomainEventsAsync(criteria, cancellationToken);

            await foreach (var @event in domainEvents.ConfigureAwait(false))
            {
                aggregateEventSourcing.LoadFromHistory(@event);
            }

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllDomainEventsAsync(
             EventStoreEntityCriteria<EventStoreEntity> criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _domainEventContext.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _domainEventContext.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in _domainEventContext.FetchAllAsync<EventStoreEntity, EventStoreEntity>(_ => selector, cancellationToken))
            {
                if (entity.ToObject(SerializerOptions) is IDomainEvent<TAggregateId> @event)
                    yield return @event;
            }
        }

        ///<inheritdoc/>
        public virtual async Task AppendDomainEventAsync(
            IDomainEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event, SerializerOptions, DocumentOptions);

            await _domainEventContext.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<ISnapShot<TAggregateId>?> ReadSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default)
        {
            var criteria = new EventStoreEntityCriteria<EventStoreEntity>()
            {
                AggregateId = aggreagteId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() != -1
            };

            var result = await _snapShotDataContext.TryFindAsync((IQueryable<EventStoreEntity> query) =>
                 query.Where(criteria)
                 .OrderBy(o => o.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64())
                 .Select(entity => entity), cancellationToken)
                .ConfigureAwait(false);

            return result is not null ? result.ToObject(SerializerOptions) as ISnapShot<TAggregateId> : default;
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadFromSnapShot(
            TAggregateId aggregateId,
            CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must " +
                    $"implement {typeof(IOriginator).Name}");

            var snapShot = await ReadSnapShotAsync(aggregateId, cancellationToken)
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

            if (aggregate is not IOriginator originator)
                throw new InvalidOperationException($"{aggregate.GetType().Name} must " +
                    $"implement '{nameof(IOriginator)}' interface");

            long version = aggregate.Version;
            var criteria = new EventStoreEntityCriteria<EventStoreEntity>
            {
                AggregateId = aggregate.AggregateId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() == version
            };

            var oldSnapShot = await _snapShotDataContext.TryFindAsync((IQueryable<EventStoreEntity> query) =>
                query
                .Where(criteria)
                .Select(entity => entity), cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
                await _snapShotDataContext.DeleteAsync<EventStoreEntity>(entity => entity.Id == oldSnapShot.Id, cancellationToken)
                    .ConfigureAwait(false);

            var snapShot = new SnapShot<TAggregateId>(
                originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);

            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(snapShot, SerializerOptions, DocumentOptions);

            await _snapShotDataContext.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadDomainEventsSinceLastSnapShotAsync(
            TAggregateId aggregateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var snapShot = await ReadSnapShotAsync(aggregateId, cancellationToken)
                .ConfigureAwait(false);

            long snapShotVersion = snapShot?.Version ?? -1;

            var criteria = new EventStoreEntityCriteria<EventStoreEntity>
            {
                AggregateId = aggregateId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() > snapShotVersion
            };

            await foreach (var domainEvent in ReadAllDomainEventsAsync(criteria, cancellationToken)
                .ConfigureAwait(false))
                yield return domainEvent;
        }


        ///<inheritdoc/>
        public virtual async Task<int> ReadDomainEventCountSinceLastSnapShot(
            TAggregateId aggregateId,
            CancellationToken cancellationToken = default)
        {
            var snapShot = await ReadSnapShotAsync(aggregateId, cancellationToken)
                .ConfigureAwait(false);

            long snapShotVersion = snapShot?.Version ?? 0;

            var criteria = new EventStoreEntityCriteria<EventStoreEntity>
            {
                AggregateId = aggregateId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() > snapShotVersion
            };

            return await _domainEventContext
                .CountAsync<EventStoreEntity>(criteria, cancellationToken)
                .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountDomainEventsAsync(
            EventStoreEntityCriteria<EventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            => await _domainEventContext
                .CountAsync<EventStoreEntity>(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<int> CountNotificationEventsAsync(
            EventStoreEntityCriteria<EventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            => await _notificationEventContext
                .CountAsync<EventStoreEntity>(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task AppendNotificationEventAsync(
            INotificationEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event, SerializerOptions, DocumentOptions);

            await _notificationEventContext.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<INotificationEvent<TAggregateId>> ReadAllNotificationEventsAsync(
            EventStoreEntityCriteria<EventStoreEntity> criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _notificationEventContext.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _notificationEventContext.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in _notificationEventContext.FetchAllAsync<EventStoreEntity, EventStoreEntity>(_ => selector, cancellationToken))
            {
                if (entity.ToObject(SerializerOptions) is INotificationEvent<TAggregateId> @event)
                    yield return @event;
            }
        }

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
