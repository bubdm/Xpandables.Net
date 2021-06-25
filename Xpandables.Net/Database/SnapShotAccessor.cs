
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="ISnapShotAccessor{TAggregateId, TAggregate}"/> for EFCore.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identifier.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class SnapShotAccessor<TAggregateId, TAggregate> :
        EventStoreEntityAccessor, ISnapShotAccessor<TAggregateId, TAggregate>
        where TAggregateId : AggregateId
        where TAggregate : Aggregate<TAggregateId>
    {
        private readonly ISnapShotStoreContext _snapShotStoreContext;
        /// <summary>
        /// Constructs a new instance of <see cref="SnapShotAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="snapShotStoreContext">The snapShot store context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="snapShotStoreContext"/> is null.</exception>
        public SnapShotAccessor(ISnapShotStoreContext snapShotStoreContext)
            : base(snapShotStoreContext)
            => _snapShotStoreContext = snapShotStoreContext
                ?? throw new ArgumentNullException(nameof(snapShotStoreContext));

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

            var oldSnapShot = await _snapShotStoreContext
                .TryFindAsync<EventStoreEntity, EventStoreEntity>(entity => entity
                    .Where(s => s.AggregateId == aggregate.AggregateId
                        && s.EventData.RootElement.GetProperty("Version").GetInt64() == aggregate.Version),
                    cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
                await _snapShotStoreContext.DeleteAsync<EventStoreEntity>(
                    entity => entity.Id == oldSnapShot.Id, cancellationToken)
                    .ConfigureAwait(false);


            var snapShot = new SnapShot<TAggregateId>(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);
            await AppendAsync<TAggregateId, TAggregate>(snapShot, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<ISnapShot<TAggregateId>?> GetSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default)
        {
            var criteria = new EventStoreEntityCriteria()
            {
                AggregateId = aggreagteId,
                Count = 1,
                EventDataCriteria = data => data.RootElement.GetProperty("Version").GetInt64() != -1
            };

            ISnapShot<TAggregateId>? result = default;
            await foreach (var snapShot in ReadAllAsync<ISnapShot<TAggregateId>>(criteria, cancellationToken)
                .ConfigureAwait(false))
            {
                result = snapShot;
                break;
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task AppendEventAsync(
            IDomainEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
            => await AppendAsync<TAggregateId, TAggregate>(@event, cancellationToken).ConfigureAwait(false);
    }
}
