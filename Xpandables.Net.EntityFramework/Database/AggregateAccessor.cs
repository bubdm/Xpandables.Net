
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
using Microsoft.EntityFrameworkCore;

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
    /// The EFCore implementation of <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class AggregateAccessor<TAggregateId, TAggregate>
        : OperationResults, IAggregateAccessor<TAggregateId, TAggregate>
        where TAggregate : class, IAggregate<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {

        private readonly AggregateDataContext _context;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="context">The target aggregate context.</param>
        /// <param name="domainEventPublisher">The target domain event publisher.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public AggregateAccessor(
            IAggregateDataContext context,
            IDomainEventPublisher domainEventPublisher,
            IInstanceCreator instanceCreator)
        {
            _context = (AggregateDataContext)context;
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IDomainEventSourcing<TAggregateId> aggregateEventSourcing)
                throw new ArgumentException($"{typeof(TAggregate).Name} must " +
                    $"implement {nameof(Aggregate<TAggregateId>)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                var entity = EventStoreEntity.From<TAggregateId, TAggregate, DomainEventStoreEntity>(
                    @event, SerializerOptions, DocumentOptions);

                await _context.DomainEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);

                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing<TAggregateId> aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var entity = EventStoreEntity.From<TAggregateId, TAggregate, NotificationEventStoreEntity>(
                        @event, SerializerOptions, DocumentOptions);

                    await _context.NotificationEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
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

            var criteria = new EventStoreEntityCriteria<DomainEventStoreEntity>
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
             EventStoreEntityCriteria<DomainEventStoreEntity> criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<DomainEventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.DomainEvents.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.DomainEvents.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in selector.AsNoTracking().AsAsyncEnumerable())
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
            var entity = EventStoreEntity.From<TAggregateId, TAggregate, DomainEventStoreEntity>(
                @event, SerializerOptions, DocumentOptions);

            await _context.DomainEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<ISnapShot<TAggregateId>?> ReadSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default)
        {
            var criteria = new EventStoreEntityCriteria<SnapShotStoreEntity>()
            {
                AggregateId = aggreagteId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() != -1
            };

            var result = await _context.SnapShotEvents
                .AsNoTracking()
                .Where(criteria)
                .OrderBy(o => o.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64())
                .FirstOrDefaultAsync(cancellationToken)
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
            var criteria = new EventStoreEntityCriteria<SnapShotStoreEntity>
            {
                AggregateId = aggregate.AggregateId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() == version
            };

            var oldSnapShot = await _context.SnapShotEvents
                .Where(criteria)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
                _context.SnapShotEvents.Remove(oldSnapShot);

            var snapShot = new SnapShot<TAggregateId>(
                originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);

            var entity = EventStoreEntity.From<TAggregateId, TAggregate, SnapShotStoreEntity>(
                snapShot, SerializerOptions, DocumentOptions);

            await _context.SnapShotEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadDomainEventsSinceLastSnapShotAsync(
            TAggregateId aggregateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var snapShot = await ReadSnapShotAsync(aggregateId, cancellationToken)
                .ConfigureAwait(false);

            long snapShotVersion = snapShot?.Version ?? -1;

            var criteria = new EventStoreEntityCriteria<DomainEventStoreEntity>
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

            var criteria = new EventStoreEntityCriteria<DomainEventStoreEntity>
            {
                AggregateId = aggregateId,
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() > snapShotVersion
            };

            return await _context.DomainEvents
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountDomainEventsAsync(
            EventStoreEntityCriteria<DomainEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            => await _context.DomainEvents
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<int> CountNotificationEventsAsync(
            EventStoreEntityCriteria<NotificationEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            => await _context.NotificationEvents
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task AppendNotificationEventAsync(
            INotificationEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate, NotificationEventStoreEntity>(
                @event, SerializerOptions, DocumentOptions);

            await _context.NotificationEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<INotificationEvent<TAggregateId>> ReadAllNotificationEventsAsync(
            EventStoreEntityCriteria<NotificationEventStoreEntity> criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.NotificationEvents.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.NotificationEvents.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in selector.AsNoTracking().AsAsyncEnumerable())
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
