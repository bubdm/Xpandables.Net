
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

using Xpandables.Net.Creators;
using Xpandables.Net.Types;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Base implementation of a mapping with a class element.
    /// </summary>
    public abstract class DataElement
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

        /// <summary>
        /// Returns the element type.
        /// <para>If the element type is nullable, returns <see langword="Nullable.GetUnderlyingType(Type)"/>.</para>
        /// If the element type is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public Type GetElementType()
        {
            var targetType = Type;
            if (IsNullable) targetType = Nullable.GetUnderlyingType(targetType);
            if (IsEnumerable) targetType = targetType!.GetGenericArguments()[0];

            return targetType!;
        }

        /// <summary>
        /// Returns the element type without looking for enumerable concern.
        /// If the element is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public Type GetElementStronglyType()
        {
            var targetType = Type;
            return IsNullable ? Nullable.GetUnderlyingType(targetType)! : targetType;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Type"/>. The type must contains a parameterless constructor.
        /// </summary>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <returns>A new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to create an instance. See inner exception.</exception>
        public object CreateElement(IInstanceCreator instanceCreator) => CreateElementInstance(GetElementType(), instanceCreator);

        /// <summary>
        /// Creates an instance of the exactly value to <see cref="Type"/>.
        /// </summary>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <returns>An new instance of <see cref="System.Type"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to create an instance. See inner exception.</exception>
        public object CreateStronglyElement(IInstanceCreator instanceCreator) => CreateElementInstance(GetElementStronglyType(), instanceCreator);

        /// <summary>
        /// Creates an instance of the specified type using the instance creator.
        /// </summary>
        /// <param name="targetType">The type to be instantiated</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <returns>A new instance of the target type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to create an instance. See inner exception.</exception>
        public static object CreateElementInstance(Type targetType, IInstanceCreator instanceCreator)
        {
            _ = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            var exception = default(Exception);
            instanceCreator.OnException = ex => exception = ex.SourceException;

            return instanceCreator?.Create(targetType)
                ?? throw new InvalidOperationException($"Unable to create an instance of '{targetType.Name}' type. See inner exception", exception);
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
        public abstract void SetElement(object? value, object target, IInstanceCreator instanceCreator);
    }
}
