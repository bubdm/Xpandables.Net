
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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents an event to be written.
    /// </summary>
    public class EntityEvent : Entity
    {
        /// <summary>
        /// Gets the event id.
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Gets the aggregate related id.
        /// </summary>
        public Guid AggregateId { get; }

        /// <summary>
        /// Gets the .Net Framework content type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Determines whether or not the data is JSON.
        /// </summary>
        public bool IsJson { get; }

        /// <summary>
        /// Gets the byte representation of the type.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="EntityEvent"/> with its properties.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="aggregateId">The related aggregate identifier.</param>
        /// <param name="type">The .Net assembly content type.</param>
        /// <param name="isJson">Determines whether or not the data is JSON.</param>
        /// <param name="data">The byte representation of the event.</param>
        public EntityEvent(Guid eventId, Guid aggregateId, string type, bool isJson, byte[] data)
        {
            EventId = eventId;
            AggregateId = aggregateId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsJson = isJson;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
