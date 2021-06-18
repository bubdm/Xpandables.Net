
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
    /// Provides with methods to retrieve and persist events.
    /// </summary>
    /// /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    public interface IEventStore<TAggregateId> : ISnapShotStore<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Asynchronously returns a collection of domain events where aggregate identifier matches the specified one.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IDomainEvent{TAggregateId}"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllEventsAsync(TAggregateId aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns a collection of store entities where aggregate identifier matches the specified one 
        /// and entities match the specified criteria.
        /// </summary>
        /// <typeparam name="TStoreEntity">The type of the store entity to be returned.</typeparam>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TStoreEntity"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<TStoreEntity> ReadStoreEntitiesAsync<TStoreEntity>(
            TAggregateId aggregateId,
            StoreEntityCriteria<TStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity;

        /// <summary>
        /// Asynchronously appends the specified event.
        /// </summary>
        /// <param name="event">Then target event to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendEventAsync(IEvent<TAggregateId> @event, CancellationToken cancellationToken = default);
    }
}
