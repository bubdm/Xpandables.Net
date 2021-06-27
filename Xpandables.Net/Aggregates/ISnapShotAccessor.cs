
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Provides with methods to retrieve and persist <see cref="ISnapShot{TAggregateId}"/>.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public interface ISnapShotAccessor<TAggregateId, TAggregate>
        where TAggregateId : notnull, IAggregateId
        where TAggregate : notnull, IAggregate<TAggregateId>
    {
        /// <summary>
        /// Asynchronously returns the snapshot matching the specified aggregate identifier or null if not found.
        /// </summary>
        /// <param name="aggreagteId">the aggregate id to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="ISnapShot{TAggregateId}"/>.</returns>
        Task<ISnapShot<TAggregateId>?> ReadSnapShotAsync(
            TAggregateId aggreagteId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified aggregate as snapshot.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        Task AppendAsSnapShotAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    }
}
