using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// Represents a set of methods to read/write email event to/from an event store.
    /// </summary>
    public interface IEmailEventRepository : IRepository<EmailEventStoreEntity>
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
        /// Asynchronously returns a collection of email events matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <typeparam name="TEmailMessage">the type of the message.</typeparam>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IEmailEvent{TEmailMessage}"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<EmailEventStoreEntity> ReadEventsAsync<TEmailMessage>(
            EventStoreEntityCriteria<EmailEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TEmailMessage : class;

        /// <summary>
        /// Asynchronously appends the specified email event.
        /// </summary>
        /// <typeparam name="TEmailMessage">the type of the message.</typeparam>
        /// <param name="event">Then target email to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendEventAsync<TEmailMessage>(IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
            where TEmailMessage : class;
    }
}
