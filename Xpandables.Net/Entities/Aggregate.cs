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
    [DebuggerDisplay("Guid = {" + nameof(Guid) + "} Version = {" + nameof(Version) + "}")]
    public abstract class Aggregate : OperationResultBase, IAggregate
    {
        private readonly ICollection<IEvent> _events = new LinkedList<IEvent>();
        private readonly IDictionary<Type, Action<IDomainEvent>> _eventHandles = new Dictionary<Type, Action<IDomainEvent>>();

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        public long Version { get; internal set; } = -1;

        /// <summary>
        /// Gets the aggregate unique identifier. The default value is <see cref="Guid.NewGuid"/>.
        /// </summary>
        public Guid Guid { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Constructs the default instance of an aggregate root.
        /// </summary>
        protected Aggregate()
        {
            RegisterEventHandlers();
        }

        /// <summary>
        /// Marks all the events as committed. Just clear the list.
        /// </summary>
        public virtual void MarkEventsAsCommitted() => _events.Clear();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        public virtual IOrderedEnumerable<IDomainEvent> GetUncommittedEvents() => _events.OfType<IDomainEvent>().OrderBy(o => o.Version);

        /// <summary>
        /// Initializes the underlying aggregate with the specified events.
        /// </summary>
        /// <param name="events">The collection of events to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="events"/> is null.</exception>
        public virtual void LoadFromHistory(IOrderedEnumerable<IDomainEvent> events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var domainEvent in events)
            {
                Mutate(domainEvent);
                Version = domainEvent.Version;
            }
        }

        /// <summary>
        /// Registers all required event handlers for the underlying aggregate.
        /// You may use the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/> method.
        /// </summary>
        protected abstract void RegisterEventHandlers();

        /// <summary>
        /// Registers an handler for the specified event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">the target handler to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
        protected virtual void RegisterEventHandler<TEvent>(Action<TEvent> eventHandler)
            where TEvent : class, IDomainEvent
        {
            _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            _eventHandles.Add(typeof(TEvent), @event => eventHandler((TEvent)@event));
        }

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
        protected virtual void Apply(IOrderedEnumerable<IEvent> @events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                Apply(@event);
        }

        /// <summary>
        /// Raises the handler that matches the specified event.
        /// The default behavior uses a dictionary of type event and handlers.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The expected handler is not registered.</exception>
        protected virtual void Mutate(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_eventHandles.TryGetValue(@event.GetType(), out var eventHandler))
                throw new InvalidOperationException($"The {@event.GetType().Name} requested handler is not registered.");

            Guid = @event.AggregateId;
            eventHandler(@event);
        }

        /// <summary>
        /// Returns the new version of the instance.
        /// </summary>
        /// <returns>A <see cref="long"/> value that represents the new version of the instance</returns>
        protected long GetNewVersion() => ++Version;

        /// <summary>
        /// Applies the specified domain event to the underlying aggregate.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        void IAggregateEventSourcing.Apply(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            Mutate(@event);
            Version = @event.Version;
        }
    }
}
