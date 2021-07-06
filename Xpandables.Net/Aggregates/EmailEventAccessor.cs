
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.EmailEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The implementation of <see cref="IEmailEventAccessor{TAggregateId, TAggregate}"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class EmailEventAccessor<TAggregateId, TAggregate>
        : OperationResults, IEmailEventAccessor<TAggregateId, TAggregate>
        where TAggregate : class, IAggregate<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {

        private readonly IEmailEventDataContext _context;

        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Constructs a new instance of <see cref="EmailEventAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="context">The target aggregate context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EmailEventAccessor(IEmailEventDataContext context) => _context = context;

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<IEmailEvent<TEmailMessage>> ReadAllEmailEventsAsync<TEmailMessage>(
             EventStoreEntityCriteria<EventStoreEntity> criteria,
             [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TEmailMessage : notnull
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<EventStoreEntity> selector
                = criteria.Count is not null
                    ? _context.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : _context.Query<EventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in _context.FetchAllAsync<EventStoreEntity, EventStoreEntity>(_ => selector, cancellationToken))
            {
                if (entity.ToObject(SerializerOptions) is IEmailEvent<TEmailMessage> @event)
                    yield return @event;
            }
        }

        ///<inheritdoc/>
        public virtual async Task AppendEmailEventAsync<TEmailMessage>(
            IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
            where TEmailMessage : notnull
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(
                @event, SerializerOptions, DocumentOptions);

            await _context.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
