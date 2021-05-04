﻿
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

using Xpandables.Net.Database;
using Xpandables.Net.Entities;
using Xpandables.Net.Events;
using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// The EFCore implementation of <see cref="IEventStore"/>.
    /// You can derived from this class to customize its behaviors.
    /// </summary>
    public class EventStore : OperationResultBase, IEventStore
    {
        private readonly IEventStoreDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="EventStore"/>.
        /// </summary>
        /// <param name="context">The context to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EventStore(IEventStoreDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult> AppendEventAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event is IDomainEvent domainEvent)
            {
                var entityEvent = new DomainEventEntity(domainEvent);
                await _context.InsertAsync(entityEvent, cancellationToken).ConfigureAwait(false);
            }
            else if(@event is IIntegrationEvent integrationEvent)
            {
                var entityEvent = new IntegrationEventEntity(integrationEvent);
                await _context.InsertAsync(entityEvent, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return BadOperation(@event.GetType().Name, "Unexpected event type.");
            }

            return OkOperation();
        }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerable<IDomainEvent> ReadDomainEventsAsync(Guid aggreagateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var entityEvent in _context.FetchAllAsync<DomainEventEntity, DomainEventEntity>(
                e => e.Where(x => x.AggregateId == aggreagateId)
                    .OrderBy(o => o.Version), cancellationToken)
                .ConfigureAwait(false))
            {
                if (entityEvent.Deserialize() is { } @event)
                    yield return @event;
            }
        }
    }
}
