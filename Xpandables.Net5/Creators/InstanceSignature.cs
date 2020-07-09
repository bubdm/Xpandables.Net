
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
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net5.Creators;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Provides an helper to get the property signature.
    /// </summary>
    class InstanceSignature : IEquatable<InstanceSignature>
    {
        public InstanceProperty[] properties;
        public int hashCode;

        /// <summary>
        /// Returns a new instance of <see cref="InstanceSignature"/> with the list of properties.
        /// </summary>
        /// <param name="properties">The properties to be used to sign</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null or empty.</exception>
        public InstanceSignature(IEnumerable<InstanceProperty> properties)
        {
            if (properties?.Any() != true) throw new ArgumentNullException(nameof(properties));

            this.properties = properties.ToArray();
            hashCode = 0;
            foreach (var p in properties)
                hashCode ^= p.Name.GetHashCode(StringComparison.InvariantCulture) ^ p.Type.GetHashCode();
        }

        public override int GetHashCode() => hashCode;

        public override bool Equals(object? obj) => obj is InstanceSignature signature && Equals(signature);

        public bool Equals(InstanceSignature? other)
        {
            if (other is null) return false;
            if (properties.Length != other.properties.Length) return false;
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name != other.properties[i].Name ||
                    properties[i].Type != other.properties[i].Type) return false;
            }
            return true;
        }
    }
}
