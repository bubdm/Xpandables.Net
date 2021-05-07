
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
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Represents a set of methods to read/write aggregates from an event store.
    /// For persistence, decorate your command with <see cref="Decorators.IAggregatePersistenceDecorator"/> interface.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public interface IAggregateAccessor<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
        /// <summary>
        /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        Task<IOperationResult<TAggregate>> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns a collection of domain events where aggregate identifier matches the specified one.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="aggreagateId">The target aggregate identifier.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IDomainEvent"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<IDomainEvent> ReadAllEventsAsync(Guid aggreagateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified <typeparamref name="TAggregate"/> aggregate to the event store.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        Task<IOperationResult> AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the specified aggregate identifier from its snapShot. The aggregate must implement <see cref="IOriginator"/> interface.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        Task<IOperationResult<TAggregate>> ReadFromSnapShot(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns a collection of domain events where aggregate identifier matches the specified one since last snapshot.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IDomainEvent"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<IDomainEvent> ReadEventsSinceLastSnapShotAsync(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the number of events since the last snapshot.
        /// </summary>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the number of  events.</returns>
        Task<int> ReadEventCountSinceLastSnapShot(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified aggregate as snapshot. The aggregate must implement <see cref="IOriginator"/> interface.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        Task<IOperationResult> AppendAsSnapShotAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the snapshot matching the specified aggregate identifier or null if not found.
        /// </summary>
        /// <param name="aggreagteId">the aggregate id to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <see cref="ISnapShot"/>.</returns>
        Task<ISnapShot?> GetSnapShotAsync(Guid aggreagteId, CancellationToken cancellationToken = default);
    }
}
