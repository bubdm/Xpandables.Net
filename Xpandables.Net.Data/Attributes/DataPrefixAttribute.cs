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
    /// Defines the prefix of the property/field on the target data source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataPrefixAttribute : Attribute
    {
        /// <summary>
        /// Defines the prefix of the property/field to be used on a data source.
        /// </summary>
        /// <param name="prefix">The prefix value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="prefix"/> is null.</exception>
        public DataPrefixAttribute(string prefix) => Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));

        /// <summary>
        /// Gets the value of the prefix string used to map the data source column with.
        /// </summary>
        public string Prefix { get; }
    }
}
