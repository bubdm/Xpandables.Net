
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

using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="IIntegrationEvent"/> with <see cref="DateTime.UtcNow"/> as default <see cref="OccurredOn"/> value.
    /// </summary>
    [Serializable]
    public abstract class IntegrationEvent : EventBase, IIntegrationEvent
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="IntegrationEvent"/>
        /// class assigning <see cref="DateTime.UtcNow"/> to the <see cref="OccurredOn"/> property.
        /// </summary>
        protected IntegrationEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }

        /// <summary>
        /// When the event occurred.
        /// </summary>
        public DateTime OccurredOn { get; }
    }

    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="IIntegrationEvent{TDomainEvent}"/>.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    [Serializable]
    public abstract class IntegrationEvent<TDomainEvent> : IntegrationEvent, IIntegrationEvent<TDomainEvent>
        where TDomainEvent : notnull, IDomainEvent
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="IntegrationEvent{TDomainEvent}"/>.
        /// </summary>
        protected IntegrationEvent(TDomainEvent domainEvent)
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
