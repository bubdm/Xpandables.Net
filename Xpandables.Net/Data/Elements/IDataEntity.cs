
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
    /// Definition of a mapping associated with a class.
    /// </summary>
    public interface IDataEntity : IDataElement
    {
        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        object? Entity { get; }

        /// <summary>
        /// Gets the property on parent entity if exist.
        /// </summary>
        IDataProperty? ParentProperty { get; }

        /// <summary>
        /// Gets the parent entity if exist.
        /// </summary>
        object? ParentEntity { get; }

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
        string? Identity { get; }

        /// <summary>
        /// Determine whether or not the underlying object is already signed.
        /// </summary>
        public bool IsIdentified => !string.IsNullOrWhiteSpace(Identity);

        /// <summary>
        /// Builds the identity using the current entity instance.
        /// The entity must be already assigned.
        /// </summary>
        void BuildIdentity();

        /// <summary>
        /// Sets the parent property of the current entity.
        /// </summary>
        /// <param name="entity">The parent entity.</param>
        /// <param name="parentProperty">The property parent.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="parentProperty"/> is null.</exception>
        IDataEntity SetParent(object entity, IDataProperty parentProperty);
    }
}
