
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
using System.Text.Json.Serialization;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.NotificationEvents
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as a notification event.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    public interface INotificationEvent<TAggregateId> : IEvent<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    { }

    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as notification holding a specific <see cref="IDomainEvent{TAggregateId}"/>.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    public interface INotificationEvent<TAggregateId, out TDomainEvent> : INotificationEvent<TAggregateId>
        where TDomainEvent : notnull, IDomainEvent<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the target domain event.
        /// </summary>
        [JsonIgnore]
        TDomainEvent DomainEvent { get; }
    }
}
