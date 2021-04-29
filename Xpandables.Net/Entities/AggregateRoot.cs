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
using System.Diagnostics;
using System.Linq;

using Xpandables.Net.Events;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Guid = {" + nameof(Id) + "} Version = {" + nameof(Version) + "}")]
    public abstract class AggregateRoot : OperationResultBase, IAggregateRoot
    {
        private readonly ICollection<IEvent> _events = new LinkedList<IEvent>();

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        public long Version { get; internal set; } = -1;

        /// <summary>
        /// Gets the aggregate unique identifier. The default value is <see cref="Guid.NewGuid"/>.
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Constructs the default instance of an aggregate root.
        /// </summary>
        protected AggregateRoot()
        {
        }

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateRoot"/> using the specified collection of events.
        /// </summary>
        /// <param name="domainEvents">The events to be applied to the aggregate root</param>
        public AggregateRoot(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                Mutate(domainEvent);
                Version++;
            }
        }

        /// <summary>
        /// Marks all the events as committed. Just clear the list.
        /// </summary>
        public void MarkEventsAsCommitted() => _events.Clear();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _events.OfType<IDomainEvent>();

        /// <summary>
        /// Applies the specified event to the current instance.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentException">The <paramref name="event"/> is null.</exception>
        protected virtual void Apply(IEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (@event is IDomainEvent domainEvent)
                if (!_events.Any(e => Equals(e.Guid, domainEvent.Guid)))
                    Mutate(domainEvent);
                else
                    return;

            _events.Add(@event);
        }

        /// <summary>
        /// Applies the specified collection of events to the current instance.
        /// </summary>
        /// <param name="events">The collection of events to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="events"/> is null.</exception>
        protected virtual void Apply(IEnumerable<IEvent> @events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                Apply(@event);
        }

        /// <summary>
        /// Dynamically applies the event using the specific pattern :
        /// Every event handler should be defined like : void On(EventType event)
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        protected virtual void Mutate(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((dynamic)this).On((dynamic)@event);
        }

        /// <summary>
        /// Returns the new version of the instance.
        /// </summary>
        /// <returns>A <see cref="long"/> value that represents the new version of the instance</returns>
        protected long GetNewVersion() => ++Version;
    }
}
