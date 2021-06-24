
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

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="INotification{TAggregateId}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    public abstract class Notification<TAggregateId> : Event<TAggregateId>, INotification<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification{TAggregateId}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        protected Notification(TAggregateId aggregateId) : base(aggregateId) { }
    }

    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="INotification{TAggregateId, TEvent}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    [Serializable]
    public abstract class Notification<TAggregateId, TDomainEvent> : Notification<TAggregateId>, INotification<TAggregateId, TDomainEvent>
        where TDomainEvent : notnull, IDomainEvent<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification{TAggregateId, TEvent}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        /// <param name="domainEvent">The target domain event.</param>
        protected Notification(TAggregateId aggregateId, TDomainEvent domainEvent)
            : base(aggregateId)
        {
            DomainEvent = domainEvent;
        }

        ///<inheritdoc/>
        [JsonIgnore]
        public TDomainEvent DomainEvent { get; }
    }
}
