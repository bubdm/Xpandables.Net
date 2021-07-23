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

using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents a methods to read event from an event store.
    /// </summary>
    /// <typeparam name="TStoreEntity">The type of the event.</typeparam>
    public interface IEventRepository<TStoreEntity>
        where TStoreEntity : StoreEntity
    {
        /// <summary>
        /// Asynchronously returns a collection of events matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TStoreEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TStoreEntity> ReadEventsAsync(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the number of event store entities matching the criteria.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the number of event store entities matching the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<int> CountEventsAsync(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default);
    }
}
