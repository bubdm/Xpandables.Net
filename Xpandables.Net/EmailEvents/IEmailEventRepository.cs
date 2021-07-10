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
    /// <typeparam name="TEmailMessage">the type of the message.</typeparam>
    public interface IEmailEventRepository<TEmailMessage> : IRepository<EmailEventStoreEntity>
        where TEmailMessage : class
    {
        /// <summary>
        /// Asynchronously returns a collection of email events matching the criteria.
        /// if not found, returns an empty collection.
        /// </summary>
        /// <param name="criteria">The criteria to be applied to entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <see cref="IEmailEvent{TEmailMessage}"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<EmailEventStoreEntity> ReadEventsAsync(
            EventStoreEntityCriteria<EmailEventStoreEntity> criteria,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously appends the specified email event.
        /// </summary>
        /// <param name="event">Then target email to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendEventAsync(IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default);
    }
}
