
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents an email event entity to be written.
    /// </summary>
    [Table(nameof(EmailEvents))]
    public sealed class EmailEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// For internal use.
        /// </summary>
        public EmailEventStoreEntity() { }

        /// <summary>
        /// Constructs a new instance of <see cref="EmailEventStoreEntity"/> with the specified properties.
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
        public EmailEventStoreEntity(
            string aggregateId,
            string aggregateTypeName,
            string eventTypeFullName,
            string eventTypeName,
            JsonDocument eventData) 
            : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData)
        {
        }
    }
}
