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
using Xpandables.Net.NotificationEvents;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IAggregate{TAggregateId}"/>.
    /// It contains a collection of <see cref="IDomainEvent{TAggregateId}"/>, <see cref="INotificationEvent{TAggregateId}"/>,
    /// and a dictionary of domain event handlers. You must register event handlers using 
    /// the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/>
    /// method when overriding the <see cref="RegisterEventHandlers"/> method. 
    /// You can add a notification using the <see cref="AddNotification(INotificationEvent{TAggregateId})"/>
    /// method and you may use the <see cref="RaiseEvent{TEvent}(TEvent)"/> method to raise the specified event.
    /// When creating an event (<see cref="IDomainEvent{TAggregateId}"/>), you may use of <see cref="GetNewVersion()"/> 
    /// function to get the new version number according to the event creation.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    [DebuggerDisplay("Guid = {" + nameof(AggregateId) + "} Version = {" + nameof(Version) + "}")]
    public abstract class Aggregate<TAggregateId> :
        OperationResults, IAggregate<TAggregateId>, IDomainEventSourcing<TAggregateId>,
        INotificationSourcing<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        private readonly ICollection<IDomainEvent<TAggregateId>> _events
            = new LinkedList<IDomainEvent<TAggregateId>>();
        private readonly ICollection<INotificationEvent<TAggregateId>> _notifications
            = new LinkedList<INotificationEvent<TAggregateId>>();
        private readonly IDictionary<Type, Action<IDomainEvent<TAggregateId>>> _eventHandlers
            = new Dictionary<Type, Action<IDomainEvent<TAggregateId>>>();

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        public AggregateVersion Version { get; protected set; } = -1;

        ///<inheritdoc/>
        public TAggregateId AggregateId { get; protected set; } = default!;

        /// <summary>
        /// Constructs the default instance of an aggregate root.
        /// </summary>
        protected Aggregate()
        {
            RegisterEventHandlers();
        }

        void INotificationSourcing<TAggregateId>.MarkNotificationsAsCommitted() => _notifications.Clear();

        IOrderedEnumerable<INotificationEvent<TAggregateId>> INotificationSourcing<TAggregateId>.GetNotifications()
            => _notifications.OrderBy(o => o.OccurredOn);

        void IDomainEventSourcing<TAggregateId>.MarkEventsAsCommitted() => _events.Clear();

        IOrderedEnumerable<IDomainEvent<TAggregateId>> IDomainEventSourcing<TAggregateId>.GetUncommittedEvents()
            => _events.OrderBy(o => o.Version);

        void IDomainEventSourcing<TAggregateId>.LoadFromHistory(IOrderedEnumerable<IDomainEvent<TAggregateId>> events)
        {
            _ = events ?? throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                ((IDomainEventSourcing<TAggregateId>)this).Mutate(@event);
                Version = @event.Version;
            }
        }

        void IDomainEventSourcing<TAggregateId>.LoadFromHistory(IDomainEvent<TAggregateId> @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IDomainEventSourcing<TAggregateId>)this).Mutate(@event);
            Version = @event.Version;
        }

        void IDomainEventSourcing<TAggregateId>.Apply(IDomainEvent<TAggregateId> @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_events.Any(e => Equals(e.Guid, @event.Guid)))
                ((IDomainEventSourcing<TAggregateId>)this).Mutate(@event);
            else
                return;

            _events.Add(@event);
        }

        void IDomainEventSourcing<TAggregateId>.Mutate(IDomainEvent<TAggregateId> @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            if (!_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                throw new InvalidOperationException($"The {@event.GetType().Name} requested handler is not registered.");

            AggregateId = @event.AggregateId;
            eventHandler(@event);
        }

        /// <summary>
        /// Adds the specified notification to the entity collection of notifications.
        /// This notification will be published using the out-box pattern.
        /// </summary>
        /// <param name="notification">The notification to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="notification"/> is null. </exception>
        /// <exception cref="InvalidOperationException">The target notification already exist in the collection.</exception>
        protected void AddNotification(INotificationEvent<TAggregateId> notification)
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
            where TEvent : class, IDomainEvent<TAggregateId>
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            ((IDomainEventSourcing<TAggregateId>)this).Apply(@event);
        }

        /// <summary>
        /// Registers all required event handlers for the underlying aggregate.
        /// You may use the <see cref="RegisterEventHandler{TEvent}(Action{TEvent})"/> method for each event.
        /// </summary>
        protected abstract void RegisterEventHandlers();

        /// <summary>
        /// Registers an handler for the <typeparamref name="TEvent"/> event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">the target handler to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exist in the collection.</exception>
        protected void RegisterEventHandler<TEvent>(Action<TEvent> eventHandler)
            where TEvent : class, IDomainEvent<TAggregateId>
        {
            _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            if (!_eventHandlers.TryAdd(typeof(TEvent), @event => eventHandler((TEvent)@event)))
                throw new ArgumentException($"An element with the same key '{typeof(TEvent).GetNameWithoutGenericArity()}' " +
                    $"already exists in the collection");
        }

        /// <summary>
        /// Returns the new version of the instance when creating an <see cref="IDomainEvent{TAggregateId}"/>.
        /// </summary>
        /// <returns>A <see cref="AggregateVersion"/> value that represents the new version of the instance</returns>
        protected AggregateVersion GetNewVersion() => ++Version;
    }
}
