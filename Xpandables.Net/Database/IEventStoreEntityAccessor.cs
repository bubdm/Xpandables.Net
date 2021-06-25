
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

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Provides with methods to retrieve and persist event store entities.
    /// </summary>
    public interface IEventStoreEntityAccessor
    {
        /// <summary>
        /// Asynchronously appends the specified event to the store entity.
        /// </summary>
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// <typeparam name="TAggregateId">The type of the aggregate id.</typeparam>
        /// <param name="event">Then target event to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendAsync<TAggregateId, TAggregate>(
            IEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate<TAggregateId>
            where TAggregateId : class, IAggregateId;

        /// <summary>
        /// Asynchronously returns a collection of events that match the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be returned.</typeparam>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TEvent"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<TEvent> ReadAllAsync<TEvent>(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;

        /// <summary>
        /// Asynchronously returns the number of events matching the criteria.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the number of  events.</returns>
        Task<int> CountAsync(EventStoreEntityCriteria criteria, CancellationToken cancellationToken = default);
    }
}
