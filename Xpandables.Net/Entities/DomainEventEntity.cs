
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
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents a domain event to be written.
    /// </summary>
    public class DomainEventEntity : Entity
    {
        ///<inheritdoc/>
        [JsonConstructor]
        protected DomainEventEntity(Guid eventId, Guid aggregateId, string type, long version, bool isJson, byte[] data)
        {
            EventId = eventId;
            AggregateId = aggregateId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Version = version;
            IsJson = isJson;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets the event id.
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Gets the aggregate related id.
        /// </summary>
        [ConcurrencyCheck]
        public Guid AggregateId { get; }

        /// <summary>
        /// Gets the .Net Framework content type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [ConcurrencyCheck]
        public long Version { get; }

        /// <summary>
        /// Determines whether or not the data is JSON.
        /// </summary>
        public bool IsJson { get; }

        /// <summary>
        /// Gets the byte representation of the type.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="DomainEventEntity"/> with its properties.
        /// </summary>
        /// <param name="event">The event to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public DomainEventEntity(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            EventId = @event.Guid;
            AggregateId = @event.AggregateId;
            Type = @event.GetType().AssemblyQualifiedName!;
            Version = @event.Version;
            IsJson = true;
            Data = Serialize(@event);
        }

        /// <summary>
        /// Serializes the event to a JSON string using the <see cref="System.Text.Json"/>.
        /// You can override this method to customize its behavior.
        /// </summary>
        /// <returns>A JSON string.</returns>
        protected virtual byte[] Serialize(IDomainEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, @event.GetType()));
        }

        /// <summary>
        /// Deserializes the current message to the expected type or null using the <see cref="System.Text.Json"/>.
        /// </summary>
        /// <returns>An instance of the target event type or null.</returns>
        public virtual IDomainEvent? Deserialize()
            => JsonSerializer.Deserialize(Encoding.UTF8.GetString(Data), System.Type.GetType(Type)!) as IDomainEvent;
    }
}
