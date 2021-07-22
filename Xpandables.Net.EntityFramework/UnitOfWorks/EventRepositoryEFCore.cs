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
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IEventRepository{TEventStoreEntity}"/>.
    /// </summary>
    /// <typeparam name="TEventStoreEntity">The type of the event.</typeparam>
    public class EventRepositoryEFCore<TEventStoreEntity> : IEventRepository<TEventStoreEntity>
        where TEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// Gets the current context instance.
        /// </summary>
        protected virtual ContextEFCore Context { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="EventRepositoryEFCore{TEventStoreEntity}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EventRepositoryEFCore(ContextEFCore context) => Context = context ?? throw new ArgumentNullException(nameof(context));

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TEventStoreEntity> ReadEventsAsync(EventStoreEntityCriteria<TEventStoreEntity> criteria, CancellationToken cancellationToken = default)
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<TEventStoreEntity> selector = (criteria.Size, criteria.Index) switch
            {
                (null, null) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn),
                (null, { }) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value),
                ({ }, null) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Size.Value),
                (_, _) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value * criteria.Size.Value).Take(criteria.Size.Value)
            };

            return selector.AsNoTracking().AsAsyncEnumerable();
        }
    }
}
