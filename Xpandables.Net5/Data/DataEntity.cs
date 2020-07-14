
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

using Xpandables.Net5.Creators;
using Xpandables.Net5.Cryptography;
using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Implementation of a mapping associated with a class.
    /// </summary>
    public class DataEntity
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
        public DataEntity(Type type, IEnumerable<DataProperty> properties)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            IsNullable = type.IsNullable();
            IsEnumerable = type.IsEnumerable();
        }

        /// <summary>
        /// Defines a custom identity builder for the entity.
        /// </summary>
        /// <param name="identityBuilder">The identity builder to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="identityBuilder"/> is null.</exception>
        public DataEntity AddIdentityBuilder(DataIdentityBuilder identityBuilder)
        {
            IdentityBuilder = identityBuilder ?? throw new ArgumentNullException(nameof(identityBuilder));
            return this;
        }

        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        public object? Entity { get; protected set; }

        /// <summary>
        /// Gets the type full name of the instance.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        public IEnumerable<DataProperty> Properties { get; }

        /// <summary>
        /// Gets the identity that is unique for the entity.
        /// </summary>
        public string Identity { get; protected set; } = string.Empty;

        /// <summary>
        /// Determine whether or not the underlying object is already signed.
        /// </summary>
        public bool IsIdentified => !string.IsNullOrWhiteSpace(Identity);

        /// <summary>
        /// Determine whether the property is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Determine whether the property is a collection.
        /// </summary>
        public bool IsEnumerable { get; }

        /// <summary>
        /// Contains a custom identity builder.
        /// </summary>
        protected DataIdentityBuilder IdentityBuilder { get; set; }
            = entity =>
            {
                if (entity.IsIdentified) return entity.Identity;

                string value = _stringGenerator.Generate(32, Key);
                if (entity.Properties.Any(property => property.IsIdentity))
                {
                    if (entity.Entity is null)
                        throw new InvalidOperationException($"Unable to build identity for an empty entity : {entity.Type.Name}");

                    value = entity.Properties
                        .Where(property => property.IsIdentity)
                        .Select(property =>
                            entity.Entity
                                .GetType()
                                .GetProperty(property.PropertyName)
                                ?.GetValue(entity.Entity, null)
                                ?.ToString())
                        .StringJoin(';')
                        .Trim();
                }

                if (string.IsNullOrWhiteSpace(value))
                    value = _stringGenerator.Generate(32, Key);

                entity.Identity = _stringCryptography.Encrypt(value, Key).Value;
                return entity.Identity;
            };

        /// <summary>
        /// Builds the identity using the current entity instance.
        /// The properties must be already assigned.
        /// </summary>
        internal void BuildIdentity() => IdentityBuilder(this);

        /// <summary>
        /// Returns the entity type.
        /// <para>If the entity type is nullable, returns <see langword="Nullable.GetUnderlyingType(Type)"/>.</para>
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public virtual Type GetEntityType()
        {
            var targetType = Type;
            if (IsNullable) targetType = Nullable.GetUnderlyingType(targetType);
            if (IsEnumerable) targetType = targetType!.GetGenericArguments()[0];

            return targetType!;
        }

        /// <summary>
        /// Sets an instance value to the entity.
        /// </summary>
        /// <param name="instance">The instance to be set.</param>
        internal void SetEntity(object instance) => Entity = instance;

        /// <summary>
        /// Creates an instance of the exactly value to <see cref="Type"/>.
        /// </summary>
        public virtual object CreateEntity(IInstanceCreator instanceCreator)
            => instanceCreator?.Create(GetEntityType()) ?? throw new ArgumentNullException(nameof(instanceCreator));
    }

    /// <summary>
    /// Implementation of a mapping associated with a specific type class.
    /// </summary>
    /// <typeparam name="T">Type of the class.</typeparam>
    public class DataEntity<T> : DataEntity
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataEntity{T}"/> that contains a type and a collection of properties.
        /// </summary>
        /// <param name="type">the type of the entity.</param>
        /// <param name="properties">The collection of properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public DataEntity(
            Type type,
            IEnumerable<DataProperty<T>> properties)
            : base(type, properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        public new T? Entity => base.Entity as T;

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        public new IEnumerable<DataProperty<T>> Properties { get; }

        /// <summary>
        /// Creates an instance of the exactly value to <typeparamref name="T"/> type.
        /// </summary>
        public virtual new T CreateEntity(IInstanceCreator instanceCreator) => (T)base.CreateEntity(instanceCreator);
    }
}
