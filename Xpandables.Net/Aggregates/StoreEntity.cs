
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
using System.Text.Json.Serialization;

using Xpandables.Net.Entities;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a store entity to be written.
    /// </summary>
    public abstract class StoreEntity<TEntity> : Entity
        where TEntity : class
    {
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
        /// Constructs anew instance of <see cref="NotificationEntity"/> from the specified event.
        /// </summary>
        /// <param name="type">the type of the content.</param>
        /// <param name="isJson">is JSON content or not.</param>
        /// <param name="data">The content as array of bytes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> or <paramref name="data"/> is null.</exception>
        [JsonConstructor]
        protected StoreEntity(string type, bool isJson, byte[] data)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsJson = isJson;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
