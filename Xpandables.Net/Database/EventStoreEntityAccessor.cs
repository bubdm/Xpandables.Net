
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Expressions;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="IEventStoreEntityAccessor"/>.
    /// You can derived from this class to customize its behaviors.
    /// </summary>
    public abstract class EventStoreEntityAccessor : IEventStoreEntityAccessor
    {
        private readonly IDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="EventStoreEntityAccessor"/>.
        /// </summary>
        /// <param name="context">The context to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        protected EventStoreEntityAccessor(IDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync<TAggregateId, TAggregate>(
          IEvent<TAggregateId> @event,
          CancellationToken cancellationToken = default)
          where TAggregate : class, IAggregate<TAggregateId>
          where TAggregateId : class, IAggregateId
        {
            var entity = EventStoreEntity.From<TAggregateId, TAggregate>(@event);
            await _context.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<TEvent> ReadAllAsync<TEvent>(
                 EventStoreEntityCriteria criteria,
                 [EnumeratorCancellation] CancellationToken cancellationToken = default)
                 where TEvent : class, IEvent
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            Func<IQueryable<EventStoreEntity>, IQueryable<EventStoreEntity>> selector
                = criteria.Count is not null
                    ? e => e.Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Count.Value)
                    : e => e.Where(criteria).OrderBy(o => o.CreatedOn);

            await foreach (var entity in _context.FetchAllAsync(selector, cancellationToken).ConfigureAwait(false))
            {
                if (Type.GetType(entity.EventTypeFullName) is { } type)
                {
                    if (entity.EventData.ToObject(type) is TEvent @event)
                        yield return @event;
                }
            }
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            return await _context
                .CountAsync<EventStoreEntity>(criteria, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}