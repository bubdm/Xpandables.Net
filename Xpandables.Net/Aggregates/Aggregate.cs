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
    public abstract class Aggregate : OperationResults, IAggregate, IEventSourcing, INotificationSourcing
    {
        private readonly ICollection<IDomainEvent> _events = new LinkedList<IDomainEvent>();
        private readonly ICollection<INotification> _notifications = new LinkedList<INotification>();
        private readonly IDictionary<Type, Action<IDomainEvent>> _eventHandlers = new Dictionary<Type, Action<IDomainEvent>>();

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

        void INotificationSourcing.MarkNotificationsAsCommitted() => _notifications.Clear();

        IOrderedEnumerable<INotification> INotificationSourcing.GetNotifications() => _notifications.OrderBy(o => o.OccurredOn);

        void IEventSourcing.MarkEventsAsCommitted() => _events.Clear();

        IOrderedEnumerable<IDomainEvent> IEventSourcing.GetUncommittedEvents() => _events.OrderBy(o => o.Version);

        void IEventSourcing.LoadFromHistory(IOrderedEnumerable<IDomainEvent> events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                ((IEventSourcing)this).Mutate(@event);
                Version = @event.Version;
            }
        }

        void IEventSourcing.LoadFromHistory(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IEventSourcing)this).Mutate(@event);
            Version = @event.Version;
        }

        void IEventSourcing.Apply(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_events.Any(e => Equals(e.Guid, @event.Guid)))
                ((IEventSourcing)this).Mutate(@event);
            else
                return;

            _events.Add(@event);
        }

        void IEventSourcing.Mutate(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                throw new InvalidOperationException($"The {@event.GetType().Name} requested handler is not registered.");

            Guid = @event.AggregateId;
            eventHandler(@event);
        }

        /// <summary>
        /// Adds the specified notification to the entity collection of events.
        /// This event will be published using the out-box pattern.
        /// </summary>
        /// <param name="notification">The notification to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="notification"/> is null. </exception>
        /// <exception cref="InvalidOperationException">The target notification already exist in the collection.</exception>
        protected void AddNotification(INotification notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            if (_notifications.Any(e => e.Guid == notification.Guid))
                throw new InvalidOperationException($"The {notification.Guid} already exist in the collection");

            _notifications.Add(notification);
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
            _eventHandlers.Add(typeof(TEvent), @event => eventHandler((TEvent)@event));
        }

        /// <summary>
        /// Returns the new version of the instance.
        /// </summary>
        /// <returns>A <see cref="long"/> value that represents the new version of the instance</returns>
        protected long GetNewVersion() => ++Version;
    }
}
