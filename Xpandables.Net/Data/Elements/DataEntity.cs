﻿
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
using Xpandables.Net.Helpers;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Implementation of a mapping associated with a class.
    /// </summary>
    public class DataEntity : DataElement, IDataEntity
    {
        private const string Key = "ABCDEFG0123456789";
        private static readonly IStringGenerator _stringGenerator = new StringGenerator();
        private static readonly IStringCryptography _stringCryptography = new StringCryptography(_stringGenerator);

        /// <summary>
        /// Initializes a new instance of <see cref="DataEntity"/> that contains a type and a collection of properties.
        /// </summary>
        /// <param name="type">the type of the entity.</param>
        /// <param name="properties">The collection of properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public DataEntity(Type type, IEnumerable<IDataProperty> properties)
            : base(type)
            => Properties = properties ?? throw new ArgumentNullException(nameof(properties));

        /// <summary>
        /// Sets the parent of the current entity.
        /// </summary>
        /// <param name="entity">The entity parent.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public IDataEntity SetParent(object entity) => this.With(e => e.ParentEntity = entity);

        /// <summary>
        /// Builds the identity using the current entity instance.
        /// The entity must be already assigned.
        /// </summary>
        public void BuildIdentity()
        {
            if (((IDataEntity)this).IsIdentified) return;

            string value = _stringGenerator.Generate(32, Key);
            if (Properties.Any(property => property.IsIdentity))
            {
                if (Entity is null)
                    throw new InvalidOperationException($"Unable to build identity for an empty entity : {Type.Name}");

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
        /// Sets the target entity with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="target">The target element instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        public override void SetElement(object? value, object target, IInstanceCreator instanceCreator) => Entity = target;

        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        public object? Entity { get; protected set; }

        /// <summary>
        /// Gets the parent entity if exist.
        /// </summary>
        public object? ParentEntity { get; protected set; }

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        public IEnumerable<IDataProperty> Properties { get; }

        /// <summary>
        /// Gets the identity that is unique for the entity.
        /// </summary>
        public string? Identity { get; protected set; }
    }
}
