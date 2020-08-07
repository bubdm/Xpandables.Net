
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
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net.Creators;
using Xpandables.Net.Cryptography;
using Xpandables.Net.Extensions;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Definition of a mapping associated with a class.
    /// </summary>
    public interface IDataEntity : IDataElement
    {
        private const string Key = "ABCDEFG0123456789";
        private static readonly IStringGenerator _stringGenerator = new StringGenerator();
        private static readonly IStringCryptography _stringCryptography = new StringCryptography(_stringGenerator);

        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        object? Entity { get; protected set; }

        /// <summary>
        /// Gets the property on parent entity if exist.
        /// </summary>
        IDataProperty? ParentProperty { get; protected set; }

        /// <summary>
        /// Gets the parent entity if exist.
        /// </summary>
        object? ParentEntity { get; protected set; }

        /// <summary>
        /// Determines whether the current entity is a nested entity.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsNestedEntity => ParentEntity is { };

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        IEnumerable<IDataProperty> Properties { get; }

        /// <summary>
        /// Gets the identity that is unique for the entity.
        /// </summary>
        string? Identity { get; protected set; }

        /// <summary>
        /// Determine whether or not the underlying object is already signed.
        /// </summary>
        public bool IsIdentified => !string.IsNullOrWhiteSpace(Identity);

        /// <summary>
        /// Builds the identity using the current entity instance.
        /// The entity must be already assigned.
        /// </summary>
        public void BuildIdentity()
        {
            if (IsIdentified) return;

            string value = _stringGenerator.Generate(32, Key);
            if (Properties.Any(property => property.IsIdentity))
            {
                if (Entity is null)
                    throw new InvalidOperationException($"Unable to build identity from an empty entity : {Type.Name}");

                value = Properties
                    .Where(property => property.IsIdentity)
                    .Select(property =>
                            Entity
                            .GetType()
                            .GetProperty(property.PropertyName)
                            ?.GetValue(Entity, null)
                            ?.ToString())
                    .StringJoin(';')
                    .Trim();
            }

            if (string.IsNullOrWhiteSpace(value))
                value = _stringGenerator.Generate(32, Key);

            Identity = _stringCryptography.Encrypt(value, Key).Value;
        }

        /// <summary>
        /// Sets the parent property of the current entity.
        /// </summary>
        /// <param name="parentEntity">The parent entity.</param>
        /// <param name="parentProperty">The property parent.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="parentProperty"/> is null.</exception>
        public IDataEntity SetParent(object parentEntity, IDataProperty parentProperty)
        {
            ParentEntity = parentEntity;
            ParentProperty = parentProperty;

            return this;
        }

        /// <summary>
        /// Sets the target entity with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="target">The target element instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        public new void SetElement(object? value, object target, IInstanceCreator instanceCreator) => Entity = target;
    }
}
