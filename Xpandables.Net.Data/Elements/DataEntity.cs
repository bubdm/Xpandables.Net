
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

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Implementation of a mapping associated with a class.
    /// </summary>
    public class DataEntity : DataElement, IDataEntity
    {
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
     
        private object? entity;
        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        object? IDataEntity.Entity { get => entity; set => entity = value; }

        private IDataProperty? parentProperty;
        /// <summary>
        /// Gets the property on parent entity if exist.
        /// </summary>
        IDataProperty? IDataEntity.ParentProperty { get => parentProperty; set => parentProperty = value; }

        private object? parentEntity;
        /// <summary>
        /// Gets the parent entity if exist.
        /// </summary>
        object? IDataEntity.ParentEntity { get => parentEntity; set => parentEntity = value; }

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        public IEnumerable<IDataProperty> Properties { get; }

        private string? identity;
        /// <summary>
        /// Gets the identity that is unique for the entity.
        /// </summary>
        string? IDataEntity.Identity { get => identity; set => identity = value; }
    }
}
