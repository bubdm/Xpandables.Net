
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
    /// Represents an aggregate snapshot to be written.
    /// </summary>
    [Serializable]
    public class SnapShotEntity : StoreEntity
    {
        ///<inheritdoc/>
        [JsonConstructor]
        public SnapShotEntity(string aggregateId, string type, long version, bool isJson, byte[] data)
            : base(type, isJson, data)
        {
            AggregateId = aggregateId;
            Version = version;
        }

        /// <summary>
        /// Gets the string representation of the aggregate related identifier.
        /// </summary>
        [ConcurrencyCheck]
        public string AggregateId { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public long Version { get; }
    }
}
