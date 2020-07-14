
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Provides with a method to build implementations of
    /// <see cref="DataProperty"/> and <see cref="DataProperty{T}"/>.
    /// </summary>
    public class DataPropertyBuilder
    {
        /// <summary>
        /// Builds an implementation of <see cref="DataProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="propertySource">The property info source.</param>
        /// <returns>An implementation of <see cref="DataProperty{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="propertySource"/> is null.</exception>
        public virtual DataProperty<T> Build<T>(DataPropertySource propertySource)
            where T : class, new()
        {
            var (DataPrefix, PropertyName, DataName, IsIdentity) = BuildParameters(propertySource);

            return DoBuild(
                () => new DataProperty<T>(PropertyName, DataName, DataPrefix, IsIdentity, propertySource.PropertyInfo),
                propertySource);
        }

        /// <summary>
        /// Builds an implementation of <see cref="DataProperty"/>.
        /// </summary>
        /// <param name="propertySource">The property info source.</param>
        /// <returns>An implementation of <see cref="DataProperty"/>.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="propertySource"/> is null.</exception>
        public virtual DataProperty Build(DataPropertySource propertySource)
        {
            var (DataPrefix, PropertyName, DataName, IsIdentity) = BuildParameters(propertySource);

            return DoBuild(
                () => new DataProperty(PropertyName, DataName, DataPrefix, IsIdentity, propertySource.PropertyInfo),
                propertySource);
        }

        private static T DoBuild<T>(Func<T> builder, DataPropertySource source)
            where T : DataProperty
        {
            var property = builder();

            if (source.Options.Converters.TryGetValue(property.Type, out var converter))
                property.AddConverter(converter);

            if (source.Options.DataNames.TryGetValue(source.PropertyInfo.ReflectedType!, out var mapperName)
                && mapperName.TryGetValue(property.PropertyName, out var dataName))
            {
                property.SetDataName(dataName);
            }

            return property;
        }

        private static (string? DataPrefix, string PropertyName, string DataName, bool IsIdentity)
            BuildParameters(DataPropertySource source)
        {
            var property = source.PropertyInfo ?? throw new ArgumentNullException(nameof(source));
            var keys = source.IdentyProperties ?? new ReadOnlyCollection<string>(Array.Empty<string>());

            var ownerAttr = property.ReflectedType?.GetCustomAttribute<DataPrefixAttribute>();
            var propertyAttr = property.GetCustomAttribute<DataPrefixAttribute>();
            var nameAttr = property.GetCustomAttribute<DataNameAttribute>();

            var dataPrefix = propertyAttr?.Prefix ?? ownerAttr?.Prefix;
            var dataName = nameAttr?.Name ?? property.Name;
            var isIdentity = keys.Any(k => k == property.Name);

            return (dataPrefix, property.Name, dataName, isIdentity);
        }
    }
}
