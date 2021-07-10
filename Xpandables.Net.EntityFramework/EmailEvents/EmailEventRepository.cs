using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Entities;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IEmailEventRepository{TAggregateId, TAggregate}"/>.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class EmailEventRepository<TAggregateId, TAggregate> : Repository<EmailEventStoreEntity>, IEmailEventRepository<TAggregateId, TAggregate>
        where TAggregate : class, IAggregate<TAggregateId>
        where TAggregateId : class, IAggregateId
    {
        ///<inheritdoc/>
        protected override EmailEventDataContext Context { get; }

        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Constructs a new instance of <see cref="EmailEventRepository{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EmailEventRepository(EmailEventDataContext context) : base(context) => Context = context ?? throw new ArgumentNullException(nameof(context));

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<EmailEventStoreEntity> ReadEventsAsync<TEmailMessage>(
              EventStoreEntityCriteria<EmailEventStoreEntity> criteria,
              CancellationToken cancellationToken = default)
             where TEmailMessage : class
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EmailEventStoreEntity> selector = (criteria.Size, criteria.Index) switch
            {
                (null, null) => Context.Emails.Where(criteria).OrderBy(o => o.CreatedOn),
                (null, { }) => Context.Emails.Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value),
                ({ }, null) => Context.Emails.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Size.Value),
                (_, _) => Context.Emails.Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value * criteria.Size.Value).Take(criteria.Size.Value)
            };

            return selector.AsNoTracking().AsAsyncEnumerable();
        }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync<TEmailMessage>(
            IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
            where TEmailMessage : class
        {
            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = typeof(TAggregate).GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
            var entity = new EmailEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, "Created");

            await Context.Emails.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the <see cref="JsonDocument"/> from the specified document.
        /// </summary>
        /// <typeparam name="TDocument">The type of document.</typeparam>
        /// <param name="document">An instance of document to parse.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <returns>An instance of <see cref="JsonDocument"/>.</returns>
        protected virtual JsonDocument GetJsonDocument<TDocument>(
            TDocument document,
            JsonSerializerOptions? serializerOptions,
            JsonDocumentOptions documentOptions)
        {
            var eventString = JsonSerializer.Serialize(document, documentOptions.GetType(), serializerOptions);
            return JsonDocument.Parse(eventString, documentOptions);
        }
    }
}
