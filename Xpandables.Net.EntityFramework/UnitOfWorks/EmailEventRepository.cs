using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Events;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IEmailEventRepository{TEmailMessage}"/>.
    /// </summary>
    /// <typeparam name="TEmailMessage">The type of the message.</typeparam>
    public class EmailEventRepository<TEmailMessage> : Repository<EmailEventStoreEntity>, IEmailEventRepository<TEmailMessage>
        where TEmailMessage : class
    {
        /// <summary>
        /// Constructs a new instance of <see cref="EmailEventRepository{TEmailMessage}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EmailEventRepository(DataContext context) : base(context) { }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<EmailEventStoreEntity> ReadEventsAsync(
              EventStoreEntityCriteria<EmailEventStoreEntity> criteria,
              CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EmailEventStoreEntity> selector = (criteria.Size, criteria.Index) switch
            {
                (null, null) => Context.Set<EmailEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn),
                (null, { }) => Context.Set<EmailEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value),
                ({ }, null) => Context.Set<EmailEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Size.Value),
                (_, _) => Context.Set<EmailEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value * criteria.Size.Value).Take(criteria.Size.Value)
            };

            return selector.AsNoTracking().AsAsyncEnumerable();
        }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(
            IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
        {
            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = @event.AggregateId.GetType().GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
            var entity = new EmailEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, "Created");

            await Context.Set<EmailEventStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
