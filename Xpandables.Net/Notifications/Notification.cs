
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
using System.Text.Json.Serialization;

using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="INotification"/>.
    /// </summary>
    [Serializable]
    public abstract class Notification : Event, INotification
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        protected Notification(Guid aggregateId) : base(aggregateId) { }

        /// <summary>
        /// Constructs a new instance of <see cref="Notification"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name.</param>
        protected Notification(Guid aggregateId, Guid eventId, DateTimeOffset occurredOn, string createdBy)
            : base(aggregateId, eventId, occurredOn, createdBy) { }
    }

    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="INotification{TDomainEvent}"/>.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    [Serializable]
    public abstract class IntegrationEvent<TDomainEvent> : Notification, INotification<TDomainEvent>
        where TDomainEvent : notnull, IDomainEvent
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="IntegrationEvent{TDomainEvent}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        /// <param name="domainEvent">The target domain event.</param>
        protected IntegrationEvent(TDomainEvent domainEvent, Guid aggregateId)
            : base(aggregateId)
        {
            DomainEvent = domainEvent;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="Notification"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="domainEvent">The target domain event.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name.</param>
        protected IntegrationEvent(TDomainEvent domainEvent, Guid aggregateId, Guid eventId, DateTimeOffset occurredOn, string createdBy)
            : base(aggregateId, eventId, occurredOn, createdBy)
        {
            DomainEvent = domainEvent;
        }

        /// <summary>
        /// Gets the target domain event.
        /// </summary>
        [JsonIgnore]
        public TDomainEvent DomainEvent { get; }
    }
}
