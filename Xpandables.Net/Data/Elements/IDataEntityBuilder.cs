
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
using System.Linq.Expressions;
using System.Reflection;

using Xpandables.Net.Correlation;
using Xpandables.Net.Creators;
using Xpandables.Net.Data.Attributes;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Types;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Defines a method to build instance of <see cref="DataEntity"/>.
    /// </summary>
    public interface IDataEntityBuilder
    {
        private readonly static MethodInfo builder =
                typeof(IDataEntityBuilder).GetMethod(
                    "Builder",
                    new Type[] { typeof(Type), typeof(IDataOptions), typeof(IDataPropertyBuilder), typeof(IInstanceCreator) })!;

        /// <summary>
        /// Builds an implementation of <see cref="DataEntity"/>.
        /// </summary>
        /// <param name="type">The type of the class.</param>
        /// <param name="options"></param>
        /// <returns>An instance of <see cref="DataEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to build an instance. See inner exception.</exception>
        public DataEntity Build(Type type, IDataOptions options) => GetEntityBuilder(type, options);

        /// <summary>
        /// Gets the builder cache.
        /// </summary>
        CorrelationCollection<string, Lazy<Delegate>> Cache { get; }

        /// <summary>
        /// Gets the property builder.
        /// </summary>
        IDataPropertyBuilder PropertyBuilder { get; }

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        IInstanceCreator InstanceCreator { get; }

        /// <summary>
        /// Builds the key used for caching delegate.
        /// </summary>
        /// <param name="type">The type of the entity.</param>
        private static string BuildKey(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            var key = type!.FullName;
            if (type.IsEnumerable()) key += "'1";

            return key!;
        }

        /// <summary>
        /// Builds the <see cref="DataEntity"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="options"></param>
        private DataEntity GetEntityBuilder(Type type, IDataOptions options)
        {
            var builder = GetEntityBuilderDelegate<Func<Type, IDataOptions, IDataPropertyBuilder, IInstanceCreator, DataEntity>>(type);
            return builder.Invoke(type, options, PropertyBuilder, InstanceCreator);
        }

        /// <summary>
        /// Provides with the <see cref="DataEntity"/> builder delegate from type.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate to return.</typeparam>
        /// <param name="type">The type of the entity.</param>
        private TDelegate GetEntityBuilderDelegate<TDelegate>(Type type)
            where TDelegate : Delegate
        {
            var key = BuildKey(type);
            var valueFound = Cache.GetOrAdd(key, _ => new Lazy<Delegate>(() => BuildEntityDelegate<TDelegate>()));
            return (TDelegate)valueFound.Value;
        }

        /// <summary>
        /// Constructs the delegate for an entity type.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate.</typeparam>
        public static TDelegate BuildEntityDelegate<TDelegate>()
            where TDelegate : Delegate
        {
            var typeExpression = Expression.Parameter(typeof(Type));
            var optionExpression = Expression.Parameter(typeof(IDataOptions));
            var propertyExpression = Expression.Parameter(typeof(IDataPropertyBuilder));
            var creatorExpression = Expression.Parameter(typeof(IInstanceCreator));

            var parameters = new ParameterExpression[] { typeExpression, optionExpression, propertyExpression, creatorExpression };

            var methodCall = Expression.Call(builder, parameters);
            return Expression.Lambda<TDelegate>(methodCall, parameters).Compile();
        }

        /// <summary>
        /// Builds the <see cref="DataEntity"/> with provided arguments.
        /// </summary>
        /// <param name="source">The type of entity.</param>
        /// <param name="options">The execute options.</param>
        /// <param name="propertyBuilder">the property builder.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        public static DataEntity Builder(Type source, IDataOptions options, IDataPropertyBuilder propertyBuilder, IInstanceCreator instanceCreator)
        {
            var identities = GetIdentityProperties(source);
            var type = GetEntityType(source);

            var sourceProperties = type.GetProperties()
                .Where(propertyInfo => propertyInfo.GetGetMethod(true) != null && propertyInfo.GetSetMethod(true) != null)
                .ToArray();

            var properties = options.IsConditionalMappingEnabled switch
            {
                true => sourceProperties
                    .Select(propertyInfo => propertyBuilder.Build(
                        new DataPropertySource(propertyInfo, options, identities)))
                    .Where(options.ConditionalMapping!),
                _ => sourceProperties
                    .Where(property =>
                      (!options.ContainsNotMappedNames
                          || !options.NotMappedNames.TryGetValue(source, out var values)
                          || !values.Contains(property.Name))
                          && property.GetCustomAttribute<DataNotMappedAttribute>(true) is null)
                    .Select(propertyInfo => propertyBuilder.Build(
                        new DataPropertySource(propertyInfo, options, identities)))
            };

            DataEntity dataEntity = new DataEntity(source, properties);
            dataEntity.SetElement(default, dataEntity.CreateElement(instanceCreator), instanceCreator);

            return dataEntity;
        }

        /// <summary>
        /// Gets the entity from the specified type.
        /// </summary>
        /// <param name="source">The type to act on.</param>
        private static Type GetEntityType(Type source)
        {
            var type = source ?? throw new ArgumentNullException(nameof(source));

            if (type.IsEnumerable())
                type = type.GetGenericArguments()[0];
            else if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type) ?? type;

            return type;
        }

        /// <summary>
        /// Gets the identities properties from the specified type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        private static string[] GetIdentityProperties(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            var keys = type!.GetCustomAttribute<DataKeysAttribute>()?.Keys ?? Array.Empty<string>();
            return keys;
        }
    }

    /// <summary>
    /// Provides with a method to build implementations of <see cref="DataEntity"/>.
    /// </summary>
    public sealed class DataEntityBuilder : IDataEntityBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataEntityBuilder"/> class.
        /// </summary>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        /// <param name="cache">the dedicated cache.</param>
        public DataEntityBuilder(
            IDataPropertyBuilder propertyBuilder,
            IInstanceCreator instanceCreator,
            CorrelationCollection<string, Lazy<Delegate>> cache)
        {
            PropertyBuilder = propertyBuilder ?? throw new ArgumentNullException(nameof(propertyBuilder));
            InstanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Gets the builder cache.
        /// </summary>
        public CorrelationCollection<string, Lazy<Delegate>> Cache { get; }

        /// <summary>
        /// Gets the property builder.
        /// </summary>
        public IDataPropertyBuilder PropertyBuilder { get; }

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        public IInstanceCreator InstanceCreator { get; }
    }
}
