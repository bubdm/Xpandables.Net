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
    /// <para>The specified type must match the <see cref="DataPropertyConverter"/> delegate signature.</para>
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
        ///     [DataConverter(typeof(DecimalPropertyConverter), nameof(DecimalPropertyConverter.Convert))]
        ///     public decimal Amount {get; set;}
        /// }
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        /// <param name="ownerType">The type that contains the converter method.</param>
        /// <param name="methodName">The name of the method to be used. The method should be static.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="ownerType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="methodName"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to get the delegate converter. See inner exception.</exception>
        public DataConverterAttribute(Type ownerType, string methodName)
        {
            _ = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
            _ = methodName ?? throw new ArgumentNullException(nameof(methodName));

            try
            {
                Converter = (DataPropertyConverter)Delegate.CreateDelegate(typeof(DataPropertyConverter), ownerType.GetMethod(methodName));
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Unable to get the delegate converter : {ownerType.Name}.{methodName}", exception);
            }
        }

        /// <summary>
        /// Gets the fully delegate to use as a converter for the object this attribute is bound to.
        /// </summary>
        public DataPropertyConverter Converter { get; }
    }
}
