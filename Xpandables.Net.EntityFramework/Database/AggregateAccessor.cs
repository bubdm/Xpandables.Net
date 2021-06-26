
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;
using Xpandables.Net.EmailEvents;
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
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must " +
                    $"implement {nameof(Aggregate<TAggregateId>)}");

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
                await _context.DomainEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);

                await _domainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing<TAggregateId> aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
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

            await foreach (var @event in ReadAllDomainEventsAsync(
                new() { AggregateId = aggregateId },
                cancellationToken))
            {
                aggregateEventSourcing.LoadFromHistory(@event);
            }

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllDomainEventsAsync(
             EventStoreEntityCriteria criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.DomainEvents.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.DomainEvents.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in selector.AsNoTracking().AsAsyncEnumerable())
            {
                if (Type.GetType(entity.EventTypeFullName) is { } type)
                {
                    if (entity.EventData.ToObject(type) is IDomainEvent<TAggregateId> @event)
                        yield return @event;
                }
            }
        }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(
            IDomainEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
            await _context.DomainEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<ISnapShot<TAggregateId>?> GetSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default)
        {
            var criteria = new EventStoreEntityCriteria()
            {
                AggregateId = aggreagteId,
                EventDataCriteria = data => data.RootElement.GetProperty("Version").GetInt64() != -1
            };

            var result = await _context.SnapShotEvents
                .AsNoTracking()
                .Where(criteria)
                .OrderBy(o => o.EventData.RootElement.GetProperty("Version").GetInt64())
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            var type = Type.GetType(result.EventTypeFullName);
            return type is not null ? result.EventData.ToObject(type) as ISnapShot<TAggregateId> : default;
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

            var snapShot = await GetSnapShotAsync(aggregateId, cancellationToken)
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

            var criteria = new EventStoreEntityCriteria
            {
                AggregateId = aggregate.AggregateId,
                EventDataCriteria = data => data.RootElement.GetProperty("Version").GetInt64() == aggregate.Version
            };

            var oldSnapShot = await _context.SnapShotEvents
                .Where(criteria)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
                _context.SnapShotEvents.Remove(oldSnapShot);

            var snapShot = new SnapShot<TAggregateId>(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(snapShot);
            await _context.SnapShotEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
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

            await foreach (var domainEvent in ReadAllDomainEventsAsync(criteria, cancellationToken)
                .ConfigureAwait(false))
                yield return domainEvent;
        }


        ///<inheritdoc/>
        public virtual async Task<int> ReadDomainEventCountSinceLastSnapShot(
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

            return await _context.DomainEvents
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountDomainEventsAsync(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            => await _context.DomainEvents
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task AppendNotificationAsync(
            INotificationEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
            await _context.NotificationEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<INotificationEvent<TAggregateId>> ReadAllNotificationsAsync(
            EventStoreEntityCriteria criteria,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.NotificationEvents.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.NotificationEvents.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in selector.AsNoTracking().AsAsyncEnumerable())
            {
                if (Type.GetType(entity.EventTypeFullName) is { } type)
                {
                    if (entity.EventData.ToObject(type) is INotificationEvent<TAggregateId> @event)
                        yield return @event;
                }
            }
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IEmailEvent<TEmailMessage>> ReadAllEmailEventsAsync<TEmailMessage>(
             EventStoreEntityCriteria criteria,
             [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TEmailMessage : notnull
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.EmailEvents.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.EmailEvents.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in selector.AsNoTracking().AsAsyncEnumerable())
            {
                if (Type.GetType(entity.EventTypeFullName) is { } type)
                {
                    if (entity.EventData.ToObject(type) is IEmailEvent<TEmailMessage> @event)
                        yield return @event;
                }
            }
        }

        ///<inheritdoc/>
        public virtual async Task AppendEmailAsync<TEmailMessage>(IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
            where TEmailMessage : notnull
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
            await _context.EmailEvents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
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
