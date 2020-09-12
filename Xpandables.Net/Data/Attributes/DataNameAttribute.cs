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

namespace Xpandables.Net.Data.Attributes
{
    /// <summary>
    /// Specifies the name of the bound field on the target data source for the decorated property.
    /// It's useful when the decorated property/field name does not match the data source field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataNameAttribute : Attribute
    {
        /// <summary>
        /// Defines the name of the property/field to be used on a data source.
        /// </summary>
        /// <param name="name">The name value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        public DataNameAttribute(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Gets the value of the name string used to map the data source column with.
        /// </summary>
        public string Name { get; }
    }
}
