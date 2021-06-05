
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
using System.Linq;

using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Event-sourcing pattern interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    public interface IDomainEventSourcing<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the unique identifier of the aggregate.
        /// </summary>
        TAggregateId AggregateId { get; }

        /// <summary>
        /// Marks all the domain events as committed.
        /// </summary>
        void MarkEventsAsCommitted();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        IOrderedEnumerable<IDomainEvent<TAggregateId>> GetUncommittedEvents();

        /// <summary>
        /// Initializes the underlying aggregate with the specified history collection of events.
        /// </summary>
        /// <param name="events">The collection of events to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="events"/> is null.</exception>
        void LoadFromHistory(IOrderedEnumerable<IDomainEvent<TAggregateId>> events);

        /// <summary>
        /// Applies the history specified domain event to the underlying aggregate.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        void LoadFromHistory(IDomainEvent<TAggregateId> @event);

        /// <summary>
        /// Applies the specified event to the current instance.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentException">The <paramref name="event"/> is null.</exception>
        void Apply(IDomainEvent<TAggregateId> @event);

        /// <summary>
        /// Applies the mutation calling the handler that matches the specified event.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The expected handler is not registered.</exception>
        void Mutate(IDomainEvent<TAggregateId> @event);
    }
}
