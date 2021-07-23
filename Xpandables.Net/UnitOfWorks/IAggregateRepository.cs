
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Aggregates.Notifications;
using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents a set of methods to read/write aggregates to/from an event store.
    /// For persistence, decorate your command/event with <see cref="IPersistenceDecorator"/> interface.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    public interface IAggregateRepository<TAggregate> : IEventRepository<DomainEventStoreEntity>
        where TAggregate : class, IAggregate
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
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TAggregate"/> type if found or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        Task<TAggregate?> ReadAsync(IAggregateId aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified aggregate to the event store.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="aggregate"/> must 
        /// implement <see cref="IDomainEventSourcing"/> interface.</exception>
        Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the <typeparamref name="TAggregate"/> aggregate that matches the specified
        /// aggregate identifier from its snapShot. The aggregate must implement <see cref="IOriginator"/> interface.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier to search for.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TAggregate"/> type if found or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        Task<TAggregate?> ReadFromSnapShot(IAggregateId aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified aggregate as snapshot.
        /// </summary>
        /// <param name="aggregate">The aggregate to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregate"/> is null.</exception>
        Task AppendAsSnapShotAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified notification event.
        /// </summary>
        /// <param name="event">Then target notification to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendNotificationAsync(INotificationEvent @event, CancellationToken cancellationToken = default);
    }
}
