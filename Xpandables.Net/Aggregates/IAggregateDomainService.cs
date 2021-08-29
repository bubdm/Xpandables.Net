
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

using System.Text.Json;

using Xpandables.Net.Entities;
using Xpandables.Net.Services;

namespace Xpandables.Net.Aggregates;

/// <summary>
/// Represents a set of commands to manage aggregates.
/// </summary>
public interface IAggregateDomainService : IDomainService
{
    /// <summary>
    /// Gets or sets the current <see cref="JsonSerializerOptions"/> to be used for serialization.
    /// </summary>
    JsonSerializerOptions? SerializerOptions { get; set; }

    /// <summary>
    /// Gets or sets the current <see cref="JsonDocumentOptions"/> to be used for <see cref="JsonDocument"/> parsing.
    /// </summary>
    JsonDocumentOptions DocumentOptions { get; set; }

    /// <summary>
    /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the 
    /// specified aggregate identifier.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate to be returned.</typeparam>
    /// <param name="aggregateId">The aggregate identifier to search for.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <typeparamref name="TAggregate"/> type if found or null.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
    Task<TAggregate?> ReadAsync<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot;

    /// <summary>
    /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the 
    /// specified criteria.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate to be returned.</typeparam>
    /// <param name="criteria">The criteria to be applied to entities.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <typeparamref name="TAggregate"/> type if found or null.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
    Task<TAggregate?> ReadAsync<TAggregate>(DomainCriteria criteria, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot;

    /// <summary>
    /// Asynchronously appends the specified aggregate to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    /// <param name="aggregate">The aggregate to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="aggregate"/> must 
    /// implement <see cref="IDomainEventSourcing"/> interface.</exception>
    Task AppendAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot;

    /// <summary>
    /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the specified
    /// aggregate identifier from its snapShot. The aggregate must implement <see cref="IOriginator"/> interface.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate to be returned.</typeparam>
    /// <param name="aggregateId">The aggregate identifier to search for.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <typeparamref name="TAggregate"/> type if found or null.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
    Task<TAggregate?> ReadFromSnapShot<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot;

    /// <summary>
    /// Asynchronously appends the specified aggregate as snapshot.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate to be returned.</typeparam>
    /// <param name="aggregate">The aggregate to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
    Task AppendAsSnapShotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregateRoot;

    /// <summary>
    /// Asynchronously returns a collection of events matching the criteria.
    /// if not found, returns an empty collection.
    /// </summary>
    /// <typeparam name="TStoreEntity">The type of store entity.</typeparam>
    /// <param name="criteria">The criteria to be applied to entities.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An enumerator of <typeparamref name="TStoreEntity"/> that can be asynchronously enumerated.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
    IAsyncEnumerable<TStoreEntity> ReadEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
        where TStoreEntity : class, IStoreEntity;

    /// <summary>
    /// Asynchronously returns the number of event store entities matching the criteria.
    /// </summary>
    /// <typeparam name="TStoreEntity">The type of store entity.</typeparam>
    /// <param name="criteria">The criteria to be applied to entities.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the number of event store entities matching the criteria.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
    Task<int> CountEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
        where TStoreEntity : class, IStoreEntity;
}
