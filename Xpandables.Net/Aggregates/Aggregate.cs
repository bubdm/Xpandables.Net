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

using Xpandables.Net.DomainEvents;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Guid = {" + nameof(Guid) + "} Version = {" + nameof(Version) + "}")]
    public abstract class Aggregate : OperationResultBase, IAggregate, IEventSourcing, INotificationSourcing
    {
        private readonly ICollection<IDomainEvent> _domainEvents = new LinkedList<IDomainEvent>();
        private readonly ICollection<INotification> _integrationEvents = new LinkedList<INotification>();
        private readonly IDictionary<Type, Action<IDomainEvent>> _domainEventHandlers = new Dictionary<Type, Action<IDomainEvent>>();

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        public long Version { get; protected set; } = -1;

        /// <summary>
        /// Gets the aggregate unique identifier. The default value is <see cref="Guid.Empty"/>.
        /// </summary>
        public Guid Guid { get; protected set; } = Guid.Empty;

        /// <summary>
        /// Constructs the default instance of an aggregate root.
        /// </summary>
        protected Aggregate()
        {
            RegisterEventHandlers();
        }

        /// <summary>
        /// Marks all the integration events as committed.
        /// </summary>
        void INotificationSourcing.MarkEventsAsCommitted() => _integrationEvents.Clear();

        /// <summary>
        /// Returns a collection of integration events.
        /// </summary>
        /// <returns>A list of integration events.</returns>
        IOrderedEnumerable<INotification> INotificationSourcing.GetOutboxEvents() => _integrationEvents.OrderBy(o => o.OccurredOn);

        /// <summary>
        /// Marks all the events as committed. Just clear the list.
        /// </summary>
        void IEventSourcing.MarkEventsAsCommitted() => _domainEvents.Clear();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        IOrderedEnumerable<IDomainEvent> IEventSourcing.GetUncommittedEvents() => _domainEvents.OrderBy(o => o.Version);

        /// <summary>
        /// Initializes the underlying aggregate with the specified history events.
        /// </summary>
        /// <param name="events">The collection of events to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="events"/> is null.</exception>
        void IEventSourcing.LoadFromHistory(IOrderedEnumerable<IDomainEvent> events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                ((IEventSourcing)this).Mutate(@event);
                Version = @event.Version;
            }
        }

        /// <summary>
        /// Applies the specified history domain event to the underlying aggregate.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        void IEventSourcing.LoadFromHistory(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IEventSourcing)this).Mutate(@event);
            Version = @event.Version;
        }

        /// <summary>
        /// Applies the specified event to the current instance.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentException">The <paramref name="event"/> is null.</exception>
        void IEventSourcing.Apply(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_domainEvents.Any(e => Equals(e.Guid, @event.Guid)))
                ((IEventSourcing)this).Mutate(@event);
            else
                return;

            _domainEvents.Add(@event);
        }

        /// <summary>
        /// Applies the mutation calling the handler that matches the specified event.
        /// The default behavior uses a dictionary of type event and handlers.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The expected handler is not registered.</exception>
        void IEventSourcing.Mutate(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_domainEventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                throw new InvalidOperationException($"The {@event.GetType().Name} requested handler is not registered.");

            Guid = @event.AggregateId;
            eventHandler(@event);
        }

        /// <summary>
        /// Adds the specified integration event to the entity collection of events.
        /// This event will be published using the out-box pattern.
        /// </summary>
        /// <param name="event">The event to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="event"/> is null. </exception>
        /// <exception cref="InvalidOperationException">The target event already exist in the collection.</exception>
        protected void AddIntegrationEvent(INotification @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (_integrationEvents.Any(e => e.Guid == @event.Guid))
                throw new InvalidOperationException($"The {@event.Guid} already exist in the collection");

            _integrationEvents.Add(@event);
        }

        /// <summary>
        /// Raises the handler that matches the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        protected void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : class, IDomainEvent
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IEventSourcing)this).Apply(@event);
        }

        /// <summary>
        /// Registers all required event handlers for the underlying aggregate.
        /// You may use the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/> method.
        /// </summary>
        protected abstract void RegisterEventHandlers();

        /// <summary>
        /// Registers an handler for the <typeparamref name="TEvent"/> event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">the target handler to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
        protected virtual void RegisterEventHandler<TEvent>(Action<TEvent> eventHandler)
            where TEvent : class, IDomainEvent
        {
            _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            _domainEventHandlers.Add(typeof(TEvent), @event => eventHandler((TEvent)@event));
        }

        /// <summary>
        /// Returns the new version of the instance.
        /// </summary>
        /// <returns>A <see cref="long"/> value that represents the new version of the instance</returns>
        protected long GetNewVersion() => ++Version;
    }
}
