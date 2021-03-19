
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;
using System.Linq;
using System.Reflection;

namespace Xpandables.Net.Database
{
    /// <summary>
    ///  Provides with methods used to extend <see cref="IDataContext"/>.
    /// </summary>
    public static class DataContextHelpers
    {
        /// <summary>
        /// Represents a <see cref="DbSet{TEntity}"/> that can be used to query and save instances of <typeparamref name="TEntity"/>.
        /// LINQ queries against a <see cref="DbSet{TEntity}"/> will be translated into queries against the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being operated on by this set.</typeparam>
        /// <param name="dataContext">The target instance of db context.</param>
        /// <returns>An instance of <see cref="DbSet{TEntity}"/> for the specific type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public static DbSet<TEntity> Set<TEntity>(this IDataContext dataContext)
            where TEntity : class
        {
            _ = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            return (DbSet<TEntity>)dataContext.InternalDbSet<TEntity>();
        }

        /// <summary>
        /// Represents a <see cref="DbSet{TEntity}"/> that can be used to query and save instances of <typeparamref name="TEntity"/>.
        /// LINQ queries against a <see cref="DbSet{TEntity}"/> will be translated into queries against the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being operated on by this set.</typeparam>
        /// <param name="entityAccessor">the target instance of entity accessor.</param>
        /// <returns>An instance of <see cref="DbSet{TEntity}"/> for the specific type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entityAccessor"/> is null.</exception>
        public static DbSet<TEntity> Set<TEntity>(this IEntityAccessor<TEntity> entityAccessor)
            where TEntity : class
        {
            _ = entityAccessor ?? throw new ArgumentNullException(nameof(entityAccessor));
            return (DbSet<TEntity>)entityAccessor.DataContext.InternalDbSet<TEntity>();
        }

        /// <summary>
        /// Specifies the converter to be used for the generic property type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The generic target <typeparamref name="T"/> to act on.</typeparam>
        /// <param name="modelBuilder">The current model builder instance.</param>
        /// <param name="valueConverter">The converter to be applied.</param>
        /// <returns>The model builder with converter application.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueConverter"/> is null.</exception>
        public static ModelBuilder UseEnumerationValueConverterForType<T>(
            this ModelBuilder modelBuilder,
            ValueConverter<T, string> valueConverter)
            where T : EnumerationType
            => modelBuilder.UseEnumerationValueConverterForType(typeof(T), valueConverter);

        /// <summary>
        /// Specifies the converter to be used for the property type (<see cref="EnumerationType"/>).
        /// </summary>
        /// <param name="modelBuilder">The current model builder instance.</param>
        /// <param name="type">The target type to act on.</param>
        /// <param name="valueConverter">The converter to be applied.</param>
        /// <returns>The model builder with converter application.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueConverter"/> is null.</exception>
        public static ModelBuilder UseEnumerationValueConverterForType(
            this ModelBuilder modelBuilder,
            Type type,
            ValueConverter valueConverter)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

            var isTypeEnumerationFunc = new Func<IMutableEntityType, bool>(IsTypeEnumeration);
            var isPropertyTypeFunc = new Func<PropertyInfo, bool>(IsPropertyType);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(isTypeEnumerationFunc))
            {
                foreach (var property in entityType.ClrType.GetProperties().Where(isPropertyTypeFunc))
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(valueConverter);
            }

            return modelBuilder;

            static bool IsTypeEnumeration(IMutableEntityType entityType)
            {
                return !entityType.ClrType.IsSubclassOf(typeof(EnumerationType));
            }

            bool IsPropertyType(PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType == type;
            }
        }
    }
}
