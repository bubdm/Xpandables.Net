
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
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net.Entities;

/// <summary>
/// Represents an event entity to be written.
/// </summary>
public interface IStoreEntity : IEntity, IDisposable
{
    /// <summary>
    /// Gets the string representation of the aggregate related identifier.
    /// </summary>
    string AggregateId { get; }

    /// <summary>
    /// Gets the string representation of the aggregate type.
    /// </summary>
    string AggregateTypeName { get; }

    /// <summary>
    /// Gets the string representation of the .Net Framework event type full name.
    /// </summary>
    string EventTypeFullName { get; }

    /// <summary>
    /// Gets the string representation of the .Net Framework event type name
    /// (Without name space).
    /// </summary>
    string EventTypeName { get; }

    /// <summary>
    /// Gets the event representation as <see cref="JsonDocument"/>.
    /// </summary>
    object Event { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    string? Exception { get; }

    /// <summary>
    /// Gets the exception full type name.
    /// </summary>
    string? ExceptionTypeFullName { get; }

    /// <summary>
    /// Adds the specified exception to the event store.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
    void AddException(Exception exception);

    /// <summary>
    /// Remove the underlying exception.
    /// </summary>
    void RemoveException();
}

/// <summary>
/// Represents a generic event entity to be written.
/// </summary>
/// <typeparam name="TEvent">The type the event.</typeparam>
public interface IStoreEntity<TEvent> : IStoreEntity
    where TEvent : class, IEvent
{
    /// <summary>
    /// Gets the event representation as <see cref="JsonDocument"/>.
    /// </summary>
    new TEvent Event { get; }

    object IStoreEntity.Event => Event;
}

/// <summary>
/// Represents an event entity to be written.
/// </summary>
public abstract class StoreEntity : Entity, IStoreEntity, IDisposable
{
    ///<inheritdoc/>
    public object Event { get; internal init; }

    ///<inheritdoc/>
    [ConcurrencyCheck]
    public string AggregateId { get; internal init; }

    ///<inheritdoc/>
    public string AggregateTypeName { get; internal init; }

    ///<inheritdoc/>
    public string EventTypeFullName { get; internal init; }

    ///<inheritdoc/>
    public string EventTypeName { get; internal init; }

    ///<inheritdoc/>
    public string? Exception { get; internal set; }

    ///<inheritdoc/>
    public string? ExceptionTypeFullName { get; internal set; }

    ///<inheritdoc/>
    public void AddException(Exception exception)
    {
        _ = exception ?? throw new ArgumentNullException(nameof(exception));

        Exception = exception.ToString();
        ExceptionTypeFullName = exception.GetType().AssemblyQualifiedName!;
    }

    ///<inheritdoc/>
    public void RemoveException()
    {
        Exception = default;
        ExceptionTypeFullName = default;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="StoreEntity"/> with the specified properties.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateTypeName">The aggregate type name.</param>
    /// <param name="eventTypeFullName">The event type full name.</param>
    /// <param name="eventTypeName">The event type name.</param>
    /// <param name="event">The event instance.</param>
    /// <param name="exceptionTypeFullName">The exception full type name.</param>
    /// <param name="exception">The exception instance.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="aggregateTypeName"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="eventTypeFullName"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="eventTypeName"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
    [JsonConstructor]
    protected StoreEntity(
        string aggregateId,
        string aggregateTypeName,
        string eventTypeFullName,
        string eventTypeName,
        object @event,
        string? exception = default,
        string? exceptionTypeFullName = default)
    {
        AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
        AggregateTypeName = aggregateTypeName ?? throw new ArgumentNullException(nameof(aggregateTypeName));
        EventTypeFullName = eventTypeFullName ?? throw new ArgumentNullException(nameof(eventTypeFullName));
        EventTypeName = eventTypeName ?? throw new ArgumentNullException(nameof(eventTypeName));
        Event = @event ?? throw new ArgumentNullException(nameof(@event));
        Exception = exception?.ToString();
        ExceptionTypeFullName = exceptionTypeFullName;
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        (Event as IDisposable)?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
///  Represents a generic event entity to be written.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public abstract class StoreEntity<TEvent> : StoreEntity, IStoreEntity<TEvent>
    where TEvent : class, IEvent
{
    ///<inheritdoc/>
    public new TEvent Event { get; internal init; }

    /// <summary>
    /// Constructs a new instance of <see cref="StoreEntity{TEvent}"/>.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateTypeName">The aggregate type name.</param>
    /// <param name="eventTypeFullName">The event type full name.</param>
    /// <param name="eventTypeName">The event type name.</param>
    /// <param name="event">The event instance.</param>
    /// <param name="exceptionTypeFullName">The exception full type name.</param>
    /// <param name="exception">The exception instance.</param>
    protected StoreEntity(
        string aggregateId,
        string aggregateTypeName,
        string eventTypeFullName,
        string eventTypeName,
        TEvent @event,
        string? exception = null,
        string? exceptionTypeFullName = null)
        : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, @event, exception, exceptionTypeFullName)
    {
        Event = @event ?? throw new ArgumentNullException(nameof(@event));
    }
}
