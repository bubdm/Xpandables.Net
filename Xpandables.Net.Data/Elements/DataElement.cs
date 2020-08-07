
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

using Xpandables.Net.Extensions;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Base implementation of a mapping with a class element.
    /// </summary>
    public abstract class DataElement : IDataElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataElement"/> class with the target element type.
        /// </summary>
        /// <param name="type">The type of the data element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        protected DataElement(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsNullable = type.IsNullable();
            IsEnumerable = type.IsEnumerable();
            IsPrimitive = type.IsPrimitive || type.FullName!.Equals("System.String");
        }

        /// <summary>
        /// Gets the type of the target element.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Determine whether the element is a value type|string or reference type.
        /// </summary>
        public bool IsPrimitive { get; }

        /// <summary>
        /// Determine whether the target element is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Determine whether the target element is a collection.
        /// </summary>
        public bool IsEnumerable { get; }
    }   
}
