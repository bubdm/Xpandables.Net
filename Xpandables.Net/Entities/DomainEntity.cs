
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

namespace Xpandables.Net.Entities;

/// <summary>
/// Represents the domain event store entity.
/// </summary>
public class DomainEntity : StoreEntity
{
    /// <summary>
    /// Initializes a new instance of <see cref="DomainEntity"/>.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateTypeName">The aggregate type name.</param>
    /// <param name="eventTypeFullName">The event type full name.</param>
    /// <param name="eventTypeName">The event type name.</param>
    /// <param name="event">The event instance.</param>
    [JsonConstructor]
    public DomainEntity(
        string aggregateId,
        string aggregateTypeName,
        string eventTypeFullName,
        string eventTypeName,
        object @event)
        : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, @event)
    {
    }
}

/// <summary>
/// Represents the generic domain event store entity.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class DomainEntity<TEvent> : DomainEntity, IStoreEntity<TEvent>
    where TEvent : class, IEvent
{
    /// <summary>
    /// Initializes a new instance of <see cref="DomainEntity{TEvent}"/>.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateTypeName">The aggregate type name.</param>
    /// <param name="eventTypeFullName">The event type full name.</param>
    /// <param name="eventTypeName">The event type name.</param>
    /// <param name="event">The event instance.</param>
    public DomainEntity(
        string aggregateId,
        string aggregateTypeName,
        string eventTypeFullName,
        string eventTypeName,
        TEvent @event)
        : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, @event)
    {
        Event = @event;
    }

    ///<inheritdoc/>
    public new TEvent Event { get; }
}
