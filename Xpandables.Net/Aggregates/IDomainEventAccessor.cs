
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

using Xpandables.Net.Aggregates;

using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Provides with methods to retrieve and persist domain events.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public interface IDomainEventAccessor<TAggregateId, TAggregate>
        where TAggregate : notnull, IAggregate<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Asynchronously returns a collection of domain events matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IDomainEvent{TAggregateId}"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllDomainEventsAsync(
            EventStoreEntityCriteria<DomainEventStoreEntity> criteria,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified domain event.
        /// </summary>
        /// <param name="event">Then target domain event to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendDomainEventAsync(IDomainEvent<TAggregateId> @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the number of domain events matching the criteria.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the number of domain  events.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<int> CountDomainEventsAsync(EventStoreEntityCriteria<DomainEventStoreEntity> criteria, CancellationToken cancellationToken = default);
    }
}
