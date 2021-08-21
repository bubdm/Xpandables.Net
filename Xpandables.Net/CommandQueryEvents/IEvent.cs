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

namespace Xpandables.Net;

/// <summary>
/// Defines a marker interface to be used to mark an object to act as an event : Domain event or Integration event.
/// The events can be raised using the differed approach described by "Jimmy Bogard"
/// </summary>
public interface IEvent : ICommandQueryEvent
{
    /// <summary>
    /// Gets the identifier of the target aggregate.
    /// </summary>
    AggregateId AggregateId { get; }

    /// <summary>
    /// Gets the event identifier.
    /// </summary>
    Guid Guid { get; }
}

/// <summary>
/// Represents a helper class that allows implementation of <see cref="IEvent"/> interface. 
/// </summary>
[Serializable]
public abstract class Event : CommandQueryEvent, IEvent
{
    /// <summary>
    /// Constructs a new instance of <see cref="Event"/> with its target aggregate id.
    /// </summary>
    /// <param name="aggregateId">The target aggregate Id</param>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
    protected Event(AggregateId aggregateId) : base() => AggregateId = aggregateId;

    /// <summary>
    /// Constructs a new instance of <see cref="Event"/> with its target aggregate id and event identifier.
    /// </summary>
    /// <param name="aggregateId">The target aggregate Id</param>
    /// <param name="guid">The event identifier.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
    [JsonConstructor]
    protected Event(AggregateId aggregateId, Guid guid)
        : base()
    {
        AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
        Guid = guid;
    }

    ///<inheritdoc/>
    public AggregateId AggregateId { get; protected set; }

    ///<inheritdoc/>
    public Guid Guid { get; } = Guid.NewGuid();
}
