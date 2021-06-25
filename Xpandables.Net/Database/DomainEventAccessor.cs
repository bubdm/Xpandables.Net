
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="IDomainEventAccessor{TAggregateId, TAggregate}"/> for EFCore.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identifier.</typeparam>
    /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
    public class DomainEventAccessor<TAggregateId, TAggregate> :
        EventStoreEntityAccessor, IDomainEventAccessor<TAggregateId, TAggregate>
        where TAggregateId : AggregateId
        where TAggregate : Aggregate<TAggregateId>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="DomainEventAccessor{TAggregateId, TAggregate}"/>.
        /// </summary>
        /// <param name="domainEventStoreContext">The domain event store context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventStoreContext"/> is null.</exception>
        public DomainEventAccessor(IDomainEventStoreContext domainEventStoreContext)
            : base(domainEventStoreContext) { }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(
            IDomainEvent<TAggregateId> @event,
            CancellationToken cancellationToken = default)
            => await AppendAsync<TAggregateId, TAggregate>(@event, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<IDomainEvent<TAggregateId>> ReadAllDomainEventsAsync(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            => ReadAllAsync<IDomainEvent<TAggregateId>>(criteria, cancellationToken);

        ///<inheritdoc/>
        public virtual async Task<int> CountEventsAsync(
            EventStoreEntityCriteria criteria,
            CancellationToken cancellationToken = default)
            => await CountAsync(criteria, cancellationToken).ConfigureAwait(false);
    }
}
