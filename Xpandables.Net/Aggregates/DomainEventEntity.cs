
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

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a domain event to be written.
    /// </summary>
    public class DomainEventEntity : StoreEntity
    {
        ///<inheritdoc/>
        [JsonConstructor]
        public DomainEventEntity(Guid eventId, string aggregateId, string typeFullName, string typeName, long version, bool isJson, byte[] data)
            : base(typeFullName, typeName, isJson, data)
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
        /// Gets the string representation of the aggregate related identifier.
        /// </summary>
        [ConcurrencyCheck]
        public string AggregateId { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [ConcurrencyCheck]
        public long Version { get; }
    }
}
