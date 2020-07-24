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

using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data.Attributes
{
    /// <summary>
    /// Specifies what type to use as a converter for the property this attribute is bound to.
    /// <para>The specified type must implement the <see cref="IDataConverter"/> interface.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataConverterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataConverterAttribute"/> with the type converter to be used
        /// for the decorated property.
        /// </summary>
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <description>The example of decorating a property.</description>
        /// <code>
        /// The mapper will use the specified type to provide a custom converter from the data row value
        /// to the target type.
        ///
        /// public class Foo
        /// {
        ///     [DataConverter(typeof(DecimalPropertyConverter))]
        ///     public decimal Amount {get; set;}
        /// }
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// <param name="type">The type converter. The type must implement the <see cref="IDataConverter"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="type"/> does not implement <see cref="IDataConverter"/>.</exception>
        public DataConverterAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(IDataConverter).IsAssignableFrom(type))
                throw new ArgumentException($"{type.FullName} does not implement {nameof(IDataConverter)} interface.");

            var converterObj = type.GetProperty(nameof(IDataConverter.PropertyConverter), System.Reflection.BindingFlags.Public).GetValue(null);
            Converter = (DataPropertyConverter)converterObj;
        }

        /// <summary>
        /// Gets the fully qualified type name of the System.Type to use as a converter for
        /// the object this attribute is bound to.
        /// </summary>
        public DataPropertyConverter Converter { get; }
    }
}
