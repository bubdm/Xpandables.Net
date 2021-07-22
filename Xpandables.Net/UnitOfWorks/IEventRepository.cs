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

using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents a methods to read event from an event store.
    /// </summary>
    /// <typeparam name="TEventStoreEntity">The type of the event.</typeparam>
    public interface IEventRepository<TEventStoreEntity>
        where TEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// Asynchronously returns a collection of events matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TEventStoreEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TEventStoreEntity> ReadEventsAsync(EventStoreEntityCriteria<TEventStoreEntity> criteria, CancellationToken cancellationToken = default);
    }
}
