
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
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

using Xpandables.Net.Entities;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a store entity to be written.
    /// </summary>
    public abstract class StoreEntity : Entity
    {
        /// <summary>
        /// Gets the .Net Framework content type full name.
        /// </summary>
        public string TypeFullName { get; }

        /// <summary>
        /// Gets the .Net Framework content type name.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Determines whether or not the data is JSON.
        /// </summary>
        public bool IsJson { get; }

        /// <summary>
        /// Gets the byte representation of the type.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="StoreEntity"/> from the specified data.
        /// </summary>
        /// <param name="typeFullName">the full type name of the content.</param>
        /// <param name="typeName">The type name of the content.</param>
        /// <param name="isJson">is JSON content or not.</param>
        /// <param name="data">The content as array of bytes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFullName"/> or <paramref name="data"/> is null.</exception>
        [JsonConstructor]
        protected StoreEntity(string typeFullName, string typeName, bool isJson, byte[] data)
        {
            TypeFullName = typeFullName ?? throw new ArgumentNullException(nameof(typeFullName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            IsJson = isJson;
            Data = data ?? throw new ArgumentNullException(nameof(data));
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
                .Take(16)
                .ToList()
                .ForEach(e => stringBuilder.Append(e));

            return stringBuilder.ToString().ToUpperInvariant();
        }
    }
}
