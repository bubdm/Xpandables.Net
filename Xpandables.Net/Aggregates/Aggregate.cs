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

using Xpandables.Net.Aggregates.Events;
using Xpandables.Net.Aggregates.Notifications;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IAggregate{TAggregateId}"/>.
    /// It contains a collection of <see cref="IDomainEvent"/>, <see cref="INotificationEvent"/>,
    /// and a dictionary of domain event handlers. You must register event handlers using 
    /// the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/>
    /// method when overriding the <see cref="RegisterEventHandlers"/> method. 
    /// You can add a notification using the <see cref="AddNotification(INotificationEvent)"/>
    /// method and you may use the <see cref="RaiseEvent{TDomainEvent}(TDomainEvent)"/> method to raise the specified event.
    /// When creating an event (<see cref="IDomainEvent"/>), you may use of <see cref="GetNewVersion()"/> 
    /// function to get the new version number according to the event creation.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    [DebuggerDisplay("Guid = {" + nameof(AggregateId) + "} Version = {" + nameof(Version) + "}")]
    public abstract class Aggregate<TAggregateId> : OperationResults, IAggregate<TAggregateId>, IDomainEventSourcing, INotificationSourcing
        where TAggregateId : notnull, AggregateId
    {
        private static readonly IInstanceCreator _instanceCreator = new InstanceCreator();

        private readonly ICollection<IDomainEvent> _events = new LinkedList<IDomainEvent>();
        private readonly ICollection<INotificationEvent> _notifications = new LinkedList<INotificationEvent>();
        private readonly IDictionary<Type, Action<IDomainEvent>> _eventHandlers = new Dictionary<Type, Action<IDomainEvent>>();

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        public AggregateVersion Version { get; protected set; } = -1;

        ///<inheritdoc/>
        public TAggregateId AggregateId { get; protected set; } = (TAggregateId)_instanceCreator.Create(typeof(TAggregateId), Guid.Empty)!;

        /// <summary>
        /// Constructs the default instance of an aggregate root.
        /// </summary>
        protected Aggregate()
        {
            RegisterEventHandlers();
        }

        void INotificationSourcing.MarkNotificationsAsCommitted() => _notifications.Clear();

        IOrderedEnumerable<INotificationEvent> INotificationSourcing.GetNotifications()
            => _notifications.OrderBy(o => o.OccurredOn);

        void IDomainEventSourcing.MarkEventsAsCommitted() => _events.Clear();

        IOrderedEnumerable<IDomainEvent> IDomainEventSourcing.GetUncommittedEvents()
            => _events.OrderBy(o => o.Version);

        void IDomainEventSourcing.LoadFromHistory(IOrderedEnumerable<IDomainEvent> events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                ((IDomainEventSourcing)this).Mutate(@event);
                Version = @event.Version;
            }
        }

        void IDomainEventSourcing.LoadFromHistory(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IDomainEventSourcing)this).Mutate(@event);
            Version = @event.Version;
        }

        void IDomainEventSourcing.Apply(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_events.Any(e => Equals(e.Guid, @event.Guid)))
                ((IDomainEventSourcing)this).Mutate(@event);
            else
                return;

            _events.Add(@event);
        }

        void IDomainEventSourcing.Mutate(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                throw new InvalidOperationException($"The {@event.GetType().Name} requested handler is not registered.");

            AggregateId = (TAggregateId)_instanceCreator.Create(typeof(TAggregateId), @event.AggregateId.Value)!;
            eventHandler(@event);
        }

        /// <summary>
        /// Adds the specified notification to the entity collection of notifications.
        /// This notification will be published using the out-box pattern.
        /// </summary>
        /// <param name="notification">The notification to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="notification"/> is null. </exception>
        /// <exception cref="InvalidOperationException">The target notification already exist in the collection.</exception>
        protected void AddNotification(INotificationEvent notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            if (_notifications.Any(e => e.Guid == notification.Guid))
                throw new InvalidOperationException($"The {notification.Guid} already exist in the collection");

            _notifications.Add(notification);
        }

        /// <summary>
        /// Raises the handler that matches the specified event.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
        /// <param name="event">The event instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        protected void RaiseEvent<TDomainEvent>(TDomainEvent @event)
            where TDomainEvent : class, IDomainEvent
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IDomainEventSourcing)this).Apply(@event);
        }

        /// <summary>
        /// Registers all required event handlers for the underlying aggregate.
        /// You may use the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/> method for each event.
        /// </summary>
        protected abstract void RegisterEventHandlers();

        /// <summary>
        /// Registers an handler for the <typeparamref name="TDomainEvent"/> event type.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">the target handler to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exist in the collection.</exception>
        protected void RegisterEventHandler<TDomainEvent>(Action<TDomainEvent> eventHandler)
            where TDomainEvent : class, IDomainEvent
        {
            _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            if (!_eventHandlers.TryAdd(typeof(TDomainEvent), @event => eventHandler((TDomainEvent)@event)))
                throw new ArgumentException($"An element with the same key '{typeof(TDomainEvent).GetNameWithoutGenericArity()}' " +
                    $"already exists in the collection");
        }

        /// <summary>
        /// Returns the new version of the instance when creating an <see cref="IDomainEvent"/>.
        /// </summary>
        /// <returns>A <see cref="AggregateVersion"/> value that represents the new version of the instance</returns>
        protected AggregateVersion GetNewVersion() => ++Version;
    }
}
