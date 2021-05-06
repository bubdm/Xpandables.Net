
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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xpandables.Net.Database;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents a snapshot to be written.
    /// </summary>
    [Serializable]
    public class SnapShotEntity : Entity
    {
        ///<inheritdoc/>
        [JsonConstructor]
        public SnapShotEntity(Guid aggregateId, string type, long version, bool isJson, byte[] data)
        {
            AggregateId = aggregateId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Version = version;
            IsJson = isJson;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets the snapShot content.
        /// </summary>
        public byte[] Data { get; }

        ///<inheritdoc/>
        [ConcurrencyCheck]
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
        /// Gets the version.
        /// </summary>
        public long Version { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="SnapShotEntity"/> with its properties.
        /// </summary>
        /// <param name="snapShot">The snapShot to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="snapShot"/> is null.</exception>
        public SnapShotEntity(ISnapShot snapShot)
        {
            _ = snapShot ?? throw new ArgumentNullException(nameof(snapShot));

            AggregateId = snapShot.AggregateId;
            Type = snapShot.GetType().AssemblyQualifiedName!;
            Version = snapShot.Version;
            IsJson = true;
            Data = Serialize(snapShot);
        }

        /// <summary>
        /// Serializes the snapShot to a JSON string using the <see cref="System.Text.Json"/>.
        /// You can override this method to customize its behavior.
        /// </summary>
        /// <returns>A JSON string.</returns>
        protected virtual byte[] Serialize(ISnapShot snapShot)
        {
            _ = snapShot ?? throw new ArgumentNullException(nameof(snapShot));
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapShot, snapShot.GetType()));
        }

        /// <summary>
        /// Deserializes the current content to the expected type or null using the <see cref="System.Text.Json"/>.
        /// </summary>
        /// <returns>An instance of the target snapShot type or null.</returns>
        public virtual ISnapShot Deserialize()
            => (ISnapShot)JsonSerializer.Deserialize(Encoding.UTF8.GetString(Data), System.Type.GetType(Type)!)!;
    }
}
