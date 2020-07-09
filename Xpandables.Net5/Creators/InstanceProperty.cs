
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

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// The class for a dynamic property.
    /// </summary>
    public sealed class InstanceProperty
    {
        /// <summary>
        /// Returns a new instance of <see cref="InstanceProperty"/> with the specified parameters.
        /// </summary>
        /// <param name="name">The property name to be used</param>
        /// <param name="type">The property type to be used</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public InstanceProperty(string name, Type type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Gets the property Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the property Type.
        /// </summary>
        public Type Type { get; }
    }
}
