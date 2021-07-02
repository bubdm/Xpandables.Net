
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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xpandables.Net.Entities;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents an event entity to be written.
    /// </summary>
    public abstract class EventStoreEntity : Entity, IDisposable
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventStoreEntity"/> from the event of aggregate.
        /// The method will serialize the specified argument to <see cref="JsonDocument"/>.
        /// </summary>
        /// <typeparam name="TAggregateId">The type of the target aggregate id.</typeparam>
        /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
        /// <typeparam name="TEventStoreEntity"></typeparam>
        /// <param name="event">The event to act on.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <returns>A new instance of <see cref="EventStoreEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Serialization of the event failed. See inner exception</exception>
        public static TEventStoreEntity From<TAggregateId, TAggregate, TEventStoreEntity>(
            IEvent @event,
            JsonSerializerOptions? serializerOptions = default,
            JsonDocumentOptions documentOptions = default)
            where TAggregate : class, IAggregate<TAggregateId>
            where TAggregateId : class, IAggregateId
            where TEventStoreEntity : EventStoreEntity, new()
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = typeof(TAggregate).GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            string eventString;
            JsonDocument eventData;

            try
            {
                eventString = JsonSerializer.Serialize(@event, @event.GetType(), serializerOptions);
                eventData = JsonDocument.Parse(eventString, documentOptions);
            }
            catch (Exception exception) when (exception is ArgumentException or NotSupportedException or JsonException)
            {
                throw new InvalidOperationException("Serialization of the event failed. See inner exception.", exception);
            }

            return new()
            {
                AggregateId = aggregateId,
                AggregateTypeName = aggregateTypeName,
                EventTypeFullName = eventTypeFullName,
                EventTypeName = eventTypeName,
                EventData = eventData
            };
        }

        /// <summary>
        /// Gets the string representation of the aggregate related identifier.
        /// </summary>
        [ConcurrencyCheck]
        public string AggregateId { get; internal init; }

        /// <summary>
        /// Gets the string representation of the aggregate type.
        /// </summary>
        public string AggregateTypeName { get; internal init; }

        /// <summary>
        /// Gets the string representation of the .Net Framework event type full name.
        /// </summary>
        public string EventTypeFullName { get; internal init; }

        /// <summary>
        /// Gets the string representation of the .Net Framework event type name
        /// (Without name space).
        /// </summary>
        public string EventTypeName { get; internal init; }

        /// <summary>
        /// Gets the string representation of the content of the event as <see cref="JsonDocument"/>.
        /// </summary>
        public JsonDocument EventData { get; internal init; }

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A <typeparamref name="T"/> representation of the JSON value or null.</returns>
        /// <exception cref="InvalidOperationException">The deserialization failed. See inner exception.</exception>
        public T? ToObject<T>(JsonSerializerOptions? options = default)
            where T : class => EventData.ToObject<T>(options);

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the specified type.
        /// </summary>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A returnType representation of the JSON value or null if exception.</returns>
        /// <exception cref="InvalidOperationException">The deserialization failed. See inner exception.</exception>
        public object? ToObject(Type returnType, JsonSerializerOptions? options = default)
            => EventData.ToObject(returnType, options);

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the <see cref="EventTypeFullName"/> type.
        /// </summary>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A returnType representation of the JSON value or null if exception.</returns>
        /// <exception cref="InvalidOperationException">The deserialization failed. See inner exception.</exception>
        public object? ToObject(JsonSerializerOptions? options = default)
            => GetEventDataType() is { } returnType ? EventData.ToObject(returnType, options) : default;

        /// <summary>
        /// Returns the type that matches the <see cref="EventTypeFullName"/>.
        /// </summary>
        /// <param name="throwOnError"><see langword="true"/> to throw an exception if the type cannot be found; <see langword="false"/> to return null. 
        /// Specifying false also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
        /// <returns>The type with the <see cref="EventTypeFullName"/> name. If the type is not found, the <paramref name="throwOnError"/> parameter 
        /// specifies whether null is returned or an exception is thrown.
        /// In some cases, an exception is thrown regardless of the value of <paramref name="throwOnError"/>.
        /// .</returns>
        /// <exception cref="InvalidOperationException">Unable to find the type. See inner exception.</exception>
        public Type? GetEventDataType(bool throwOnError = true)
        {
            try
            {
                return Type.GetType(EventTypeFullName, throwOnError);
            }
            catch (Exception exception) when(exception is TargetInvocationException or TypeLoadException or ArgumentException or FileNotFoundException or FileLoadException or BadImageFormatException)
            {
                throw new InvalidOperationException($"Unable to find the '{EventTypeFullName}' type. See inner exception.", exception);
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="EventStoreEntity"/> with the specified properties.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="aggregateTypeName">The type name of the aggregate.</param>
        /// <param name="eventTypeFullName">The full type name of the event.</param>
        /// <param name="eventTypeName">The type name of the event.</param>
        /// <param name="eventData">The string representation of the event content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventTypeFullName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventData"/> is null.</exception>
        [JsonConstructor]
        public EventStoreEntity(
            string aggregateId,
            string aggregateTypeName,
            string eventTypeFullName,
            string eventTypeName,
            JsonDocument eventData)
        {
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            AggregateTypeName = aggregateTypeName ?? throw new ArgumentNullException(nameof(aggregateTypeName));
            EventTypeFullName = eventTypeFullName ?? throw new ArgumentNullException(nameof(eventTypeFullName));
            EventTypeName = eventTypeName ?? throw new ArgumentNullException(nameof(eventTypeName));
            EventData = eventData ?? throw new ArgumentNullException(nameof(eventData));
        }

        internal EventStoreEntity()
        {
            AggregateId = default!;
            AggregateTypeName = default!;
            EventTypeFullName = default!;
            EventTypeName = default!;
            EventData = default!;
        }

        ///<inheritdoc/>
        protected override string KeyGenerator()
        {
            var stringBuilder = new StringBuilder();

            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(_ => Guid.NewGuid())
                .Take(32)
                .ToList()
                .ForEach(e => stringBuilder.Append(e));

            return stringBuilder.ToString().ToUpperInvariant();
        }

        ///<inheritdoc/>
        public void Dispose()
        {
            EventData?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
