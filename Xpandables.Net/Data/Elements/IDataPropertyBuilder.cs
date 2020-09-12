
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
using System.Linq;
using System.Reflection;

using Xpandables.Net.Data.Attributes;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Defines a method to build instances of <see cref="DataProperty"/>.
    /// </summary>
    public interface IDataPropertyBuilder
    {
        /// <summary>
        /// Builds an instance of <see cref="DataProperty"/>.
        /// </summary>
        /// <param name="source">The property info source.</param>
        /// <returns>An implementation of <see cref="DataProperty"/>.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="source"/> is null.</exception>
        public DataProperty Build(DataPropertySource source)
        {
            var properties = BuildPropertyParameters(source);

            return new DataProperty(
                properties.Type,
                properties.PropertyName,
                properties.DataName,
                properties.DataPrefix,
                properties.IsIdentity,
                properties.Getter,
                properties.Setter,
                properties.Converter);
        }    

        private static (Type Type, string PropertyName, string DataName, string? DataPrefix, bool IsIdentity, Delegate Getter, Delegate Setter, DataPropertyConverter? Converter) BuildPropertyParameters(DataPropertySource builderSource)
        {
            var property = builderSource.PropertyInfo;
            var type = property.PropertyType;
            var propertyName = property.Name;

            var ownerAttr = property.ReflectedType?.GetCustomAttribute<DataPrefixAttribute>();
            var propertyAttr = property.GetCustomAttribute<DataPrefixAttribute>();
            var nameAttr = property.GetCustomAttribute<DataNameAttribute>();

            var dataName = nameAttr?.Name ?? property.Name;
            var dataPrefix = propertyAttr?.Prefix ?? ownerAttr?.Prefix;

            var isIdentity = builderSource.IdentyProperties.Any(k => k == propertyName);

            var getter = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(new Type[] { property.ReflectedType!, type }), null, property.GetMethod!);
            var setter = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(new Type[] { property.ReflectedType!, type }), null, property.SetMethod!);

            var converterAttr = property.GetCustomAttribute<DataConverterAttribute>();
            var converter = converterAttr?.Converter;

            if (builderSource.Options.Converters.TryGetValue(type, out var foundConverter))
                converter = foundConverter;
            if (builderSource.Options.MappedNames.TryGetValue(builderSource.PropertyInfo.ReflectedType!, out var mapperName)
                && mapperName.TryGetValue(propertyName, out var foundDataName))
            {
                dataName = foundDataName;
                dataPrefix = null;
            }

            return (type, propertyName, dataName, dataPrefix, isIdentity, getter, setter, converter);
        }
    }

    /// <summary>
    /// Provides with a method to build instances of <see cref="DataProperty"/>.
    /// </summary>
    public sealed class DataPropertyBuilder : IDataPropertyBuilder { }
}
