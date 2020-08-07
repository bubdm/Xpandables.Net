
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
    /// Defines an entity property.
    /// </summary>
    public interface IDataProperty : IDataElement
    {
        /// <summary>
        /// Gets the target property name.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the name of the column in the data source.
        /// </summary>
        string DataName { get; }

        /// <summary>
        /// Gets the prefix of the column in the data source.
        /// </summary>
        string? DataPrefix { get; }

        /// <summary>
        /// Gets the full name of the column in the data source.
        /// </summary>
        public string DataFullName => $"{DataPrefix}{DataName}";

        /// <summary>
        /// Determine whether the property is used for uniquely identify the data source.
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// Gets the setter delegate for the property.
        /// </summary>
        Delegate Setter { get; }

        /// <summary>
        /// Gets the getter delegate for the property.
        /// </summary>
        Delegate Getter { get; }

        /// <summary>
        /// Gets the delegate used to convert the data row value to the expected type of property.
        /// The default behavior will return the value.
        /// </summary>
        DataPropertyConverter Converter { get; }

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
        public new void SetElement(object? value, object target, IInstanceCreator instanceCreator)
        {
            value = value is DBNull ? null : value;
            value = Converter(this, value);

            try
            {
                if (IsEnumerable)
                {
                    if (Getter.DynamicInvoke(target) is null)
                        Setter.DynamicInvoke(target, CreateStronglyElement(instanceCreator));

                    ((IList)Getter.DynamicInvoke(target)).Add(value);
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
