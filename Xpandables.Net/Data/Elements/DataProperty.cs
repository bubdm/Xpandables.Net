
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
using System.Collections;
using System.Data;

using Xpandables.Net.Creators;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Provides with custom information for a mapping associated with a property.
    /// </summary>
    public sealed class DataProperty : DataElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataProperty"/> with all arguments using cache.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="dataName">The data name from the source.</param>
        /// <param name="dataPrefix">The data prefix from the source.</param>
        /// <param name="isIdentity">Whether the property is an identity key or not.</param>
        /// <param name="getter">The property getter delegate.</param>
        /// <param name="setter">The property setter delegate.</param>
        /// <param name="converter">The property converter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="getter"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="setter"/> is null.</exception>
        public DataProperty(
            Type type,
            string propertyName,
            string dataName,
            string? dataPrefix,
            bool isIdentity,
            Delegate getter,
            Delegate setter,
            DataPropertyConverter? converter)
            : base(type)
        {
            DataPrefix = dataPrefix;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            IsIdentity = isIdentity;

            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            Setter = setter ?? throw new ArgumentNullException(nameof(setter));

            Converter = converter ??= (_, source) => source;
        }

        /// <summary>
        /// Gets the target property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the name of the column in the data source.
        /// </summary>
        public string DataName { get; private set; }

        /// <summary>
        /// Gets the prefix of the column in the data source.
        /// </summary>
        public string? DataPrefix { get; private set; }

        /// <summary>
        /// Gets the full name of the column in the data source.
        /// </summary>
        public string DataFullName => $"{DataPrefix}{DataName}";

        /// <summary>
        /// Gets the value whether the property is used for uniquely identify the data source.
        /// </summary>
        public bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets the setter delegate for the property.
        /// </summary>
        public Delegate Setter { get; }

        /// <summary>
        /// Gets the getter delegate for the property.
        /// </summary>
        public Delegate Getter { get; }

        /// <summary>
        /// Gets the delegate used to convert the data row value to the expected type of property.
        /// The default behavior just returns the value.
        /// </summary>
        public DataPropertyConverter Converter { get; }

        /// <summary>
        /// Sets the target element with the data record value.
        /// </summary>
        /// <param name="dataRecord">The data record row to be used.</param>
        /// <param name="target">The target instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        public void SetData(IDataRecord dataRecord, object target, IInstanceCreator instanceCreator)
        {
            _ = dataRecord ?? throw new ArgumentNullException(nameof(dataRecord));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            if (dataRecord.Contains(DataFullName))
            {
                var index = dataRecord.GetOrdinal(DataFullName);
                if (!dataRecord.IsDBNull(index))
                    SetElement(dataRecord.GetValue(index), target, instanceCreator);
            }
        }

        /// <summary>
        /// Sets the target element with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="target">The target element instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        public override void SetElement(object? value, object target, IInstanceCreator instanceCreator)
        {
            value = value is DBNull ? null : value;
            value = Converter(this, value);

            try
            {
                if (IsEnumerable)
                {
                    if (Getter.DynamicInvoke(target) is null)
                        Setter.DynamicInvoke(target, CreateStronglyElement(instanceCreator));

                    if (Getter.DynamicInvoke(target) is { } list)
                        ((IList)list).Add(value);
                }
                else
                {
                    Setter.DynamicInvoke(target, value);
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Mapping the property '{target.GetType().Name}.{PropertyName}' of type '{Type.Name}' failed. Data row name '{DataFullName}', Data row value : '{value}'",
                    exception);
            }
        }
    }
}
