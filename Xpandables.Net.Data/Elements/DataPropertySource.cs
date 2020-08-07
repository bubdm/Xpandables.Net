
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Provides with information to build a property.
    /// </summary>
    public sealed class DataPropertySource
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataPropertySource"/>.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="options"></param>
        /// <param name="identyProperties"></param>
        public DataPropertySource(PropertyInfo propertyInfo, DataOptions options, string[] identyProperties)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IdentyProperties = identyProperties?.Any() == true
                ? new ReadOnlyCollection<string>(identyProperties)
                : new ReadOnlyCollection<string>(Array.Empty<string>());
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the current execution options.
        /// </summary>
        public DataOptions Options { get; }

        /// <summary>
        /// Gets a collection of properties defined as identifiers.
        /// </summary>
        public ReadOnlyCollection<string> IdentyProperties { get; }
    }
}
