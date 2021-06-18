﻿
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;
using Xpandables.Net.Entities;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The EFCore implementation of <see cref="IEventStore{TAggregateId}"/>.
    /// You can derived from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type the aggregate identity.</typeparam>
    public class EventStore<TAggregateId> : OperationResults, IEventStore<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        private readonly IEventStoreContext _context;
        private readonly IEventStoreEntityTypeConverter _converter;

        /// <summary>
        /// Constructs a new instance of <see cref="EventStore{TAggregateId}"/>.
        /// </summary>
        /// <param name="context">The context to be used.</param>
        /// <param name="converter">The converter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EventStore(IEventStoreContext context, IEventStoreEntityTypeConverter converter)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(IEvent<TAggregateId> @event, CancellationToken cancellationToken = default)
        {
            var entityEvent = GetEventEntity(_converter, @event);
            await _context.InsertAsync(entityEvent, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task AppendSnapShotAsync(IAggregate<TAggregateId> aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IOriginator originator)
                throw new InvalidOperationException($"{aggregate.GetType().Name} must implement '{nameof(IOriginator)}' interface");

            var oldSnapShot = await _context.Set<SnapShotEntity>()
                .Where(s => s.AggregateId == aggregate.AggregateId && s.Version == aggregate.Version)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
                _context.Set<SnapShotEntity>()
                    .Remove(oldSnapShot);

            var snapShot = new SnapShot<TAggregateId>(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);
            var snapShotEnity = GetEventEntity(_converter, snapShot);

            await _context.InsertAsync(snapShotEnity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<ISnapShot<TAggregateId>?> GetSnapShotAsync(TAggregateId aggreagteId, CancellationToken cancellationToken = default)
            => await _context.Set<SnapShotEntity>()
                .Where(w => w.AggregateId == aggreagteId && w.Version != -1)
                .OrderByDescending(o => o.CreatedOn)
                .Select(entity => (ISnapShot<TAggregateId>)_converter.Deserialize(Encoding.UTF8.GetString(entity.Data), Type.GetType(entity.TypeFullName)!))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllEventsAsync(TAggregateId aggreagateId, CancellationToken cancellationToken = default)
            => _context.Set<DomainEventEntity>()
                .Where(w => w.AggregateId == aggreagateId)
                .OrderBy(o => o.Version)
                .Select(entity => (IDomainEvent<TAggregateId>)_converter.Deserialize(Encoding.UTF8.GetString(entity.Data), Type.GetType(entity.TypeFullName)!))
                .AsAsyncEnumerable();

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TStoreEntity> ReadStoreEntitiesAsync<TStoreEntity>(
            TAggregateId aggregateId,
            StoreEntityCriteria<TStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity
            => _context.Set<TStoreEntity>()
                .Where(criteria)
                .OrderBy(o => o.CreatedOn)
                .Take(criteria.Count)
                .AsAsyncEnumerable();

        ///<inheritdoc/>
        public async IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadEventsSinceLastSnapShotAsync(TAggregateId aggregateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var snapShot = await GetSnapShotAsync(aggregateId, cancellationToken).ConfigureAwait(false);
            var snapShotVersion = snapShot?.Version ?? -1;

            await foreach (var entity in _context.FetchAllAsync<DomainEventEntity, DomainEventEntity>(
                e => e.Where(w => w.AggregateId == aggregateId && w.Version > snapShotVersion)
                .OrderBy(o => o.Version),
                cancellationToken)
                .ConfigureAwait(false))
            {
                yield return (IDomainEvent<TAggregateId>)_converter.Deserialize(Encoding.UTF8.GetString(entity.Data), Type.GetType(entity.TypeFullName)!);
            }
        }

        ///<inheritdoc/>
        public async Task<int> ReadEventCountSinceLastSnapShot(TAggregateId aggregateId, CancellationToken cancellationToken = default)
        {
            var snapShot = await GetSnapShotAsync(aggregateId, cancellationToken).ConfigureAwait(false);
            var snapShotVersion = snapShot?.Version ?? 0;

            return await _context.Set<DomainEventEntity>()
                .CountAsync(c => c.AggregateId == aggregateId && c.Version > snapShotVersion,
                cancellationToken)
                .ConfigureAwait(false);
        }

        static IEntity GetEventEntity(IEventStoreEntityTypeConverter converter, object source)
        {
            var typeFullName = source.GetType().AssemblyQualifiedName!;
            var typeName = source.GetType().GetNameWithoutGenericArity();
            var isJson = true;
            var json = converter.Serialize(source, source.GetType());
            var data = Encoding.UTF8.GetBytes(json);

            return source switch
            {
                IDomainEvent<TAggregateId> domain => new DomainEventEntity(domain.Guid, domain.AggregateId, typeFullName, typeName, domain.Version, isJson, data),
                INotification<TAggregateId> notifcation => new NotificationEntity(notifcation.AggregateId, typeFullName, typeName, isJson, data),
                ISnapShot<TAggregateId> snapShot => new SnapShotEntity(snapShot.AggregateId, typeFullName, typeName, snapShot.Version, isJson, data),
                _ => throw new ArgumentException($"'{source.GetType().Name}' Expected IDomainEvent, INotification or ISnapShot")
            };
        }
    }
}