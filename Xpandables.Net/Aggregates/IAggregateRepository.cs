using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Events;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a set of methods to read/write aggregates to/from an event store.
    /// For persistence, decorate your command/event with <see cref="IPersistenceDecorator"/> interface.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    public interface IAggregateRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
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
        /// implement <see cref="IDomainEventSourcing{TAggregateId}"/> interface.</exception>
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

        /// <summary>
        /// Asynchronously returns a collection of event store entities matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <typeparam name="TEventStoreEntity">The type of the event store entity.</typeparam>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TEventStoreEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TEventStoreEntity> ReadEventsAsync<TEventStoreEntity>(
            EventStoreEntityCriteria<TEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TEventStoreEntity : EventStoreEntity;

        /// <summary>
        /// Asynchronously returns the number of event store entities matching the criteria.
        /// </summary>
        /// <typeparam name="TEventStoreEntity">The type of the event store entity.</typeparam>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the number of event store entities matching the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<int> CountEventsAsync<TEventStoreEntity>(
            EventStoreEntityCriteria<TEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TEventStoreEntity : EventStoreEntity;
    }
}
