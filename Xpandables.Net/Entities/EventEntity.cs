
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
using System.Text.Json.Serialization;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents a domain event to be written.
    /// </summary>
    public class EventEntity : StoreEntity<IDomainEvent>
    {
        ///<inheritdoc/>
        [JsonConstructor]
        public EventEntity(Guid eventId, Guid aggregateId, string type, long version, bool isJson, byte[] data)
            : base(type, isJson, data)
        {
            EventId = eventId;
            AggregateId = aggregateId;
            Version = version;
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
        /// Gets the version.
        /// </summary>
        [ConcurrencyCheck]
        public long Version { get; }
    }
}
