
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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents an event entity to be written.
    /// </summary>
    public abstract class StoreEntity : Entity, IDisposable
    {
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
        /// Gets the exception.
        /// </summary>
        public string? Exception { get; private set; }

        /// <summary>
        /// Gets the exception full type name.
        /// </summary>
        public string? ExceptionTypeFullName { get; private set; }

        /// <summary>
        /// Adds the specified exception to the event store.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public void AddException(Exception exception)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));

            Exception = exception.ToString();
            ExceptionTypeFullName = exception.GetType().AssemblyQualifiedName!;
        }

        /// <summary>
        /// Remove the underlying exception.
        /// </summary>
        public void RemoveException()
        {
            Exception = default;
            ExceptionTypeFullName = default;
        }

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A <typeparamref name="T"/> representation of the JSON value or null.</returns>
        /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
        /// There is remaining data in the span beyond a single JSON value.</exception>
        /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
        /// for returnType or its serializable members.</exception>
        public T? ToObject<T>(JsonSerializerOptions? options = default)
            where T : class => EventData.ToObject<T>(options);

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the specified type.
        /// </summary>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A returnType representation of the JSON value or null if exception.</returns>
        /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
        /// There is remaining data in the span beyond a single JSON value.</exception>
        /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
        /// for returnType or its serializable members.</exception>
        public object? ToObject(Type returnType, JsonSerializerOptions? options = default)
            => EventData.ToObject(returnType, options);

        /// <summary>
        /// Deserializes the content of <see cref="EventData"/> to the <see cref="EventTypeFullName"/> type.
        /// </summary>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A returnType representation of the JSON value or null if exception.</returns>
        /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
        /// There is remaining data in the span beyond a single JSON value.</exception>
        /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
        /// for returnType or its serializable members.</exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="FileLoadException"></exception>
        /// <exception cref="BadImageFormatException"></exception>
        public object? ToObject(JsonSerializerOptions? options = default)
            => Type.GetType(EventTypeFullName, true) is { } returnType ? EventData.ToObject(returnType, options) : default;

        /// <summary>
        /// Constructs a new instance of <see cref="StoreEntity"/> with the specified properties.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="aggregateTypeName">The type name of the aggregate.</param>
        /// <param name="eventTypeFullName">The full type name of the event.</param>
        /// <param name="eventTypeName">The type name of the event.</param>
        /// <param name="eventData">The string representation of the event content.</param>
        /// <param name="exception"></param>
        /// <param name="exceptionTypeFullName"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventTypeFullName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventData"/> is null.</exception>
        [JsonConstructor]
        protected StoreEntity(
            string aggregateId,
            string aggregateTypeName,
            string eventTypeFullName,
            string eventTypeName,
            JsonDocument eventData,
            string? exception = default,
            string? exceptionTypeFullName = default)
        {
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            AggregateTypeName = aggregateTypeName ?? throw new ArgumentNullException(nameof(aggregateTypeName));
            EventTypeFullName = eventTypeFullName ?? throw new ArgumentNullException(nameof(eventTypeFullName));
            EventTypeName = eventTypeName ?? throw new ArgumentNullException(nameof(eventTypeName));
            EventData = eventData ?? throw new ArgumentNullException(nameof(eventData));
            Exception = exception?.ToString();
            ExceptionTypeFullName = exceptionTypeFullName;
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
