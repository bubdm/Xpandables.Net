
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Provides with a method to build implementations of <see cref="DataEntity"/> and <see cref="DataEntity{T}"/>.
    /// </summary>
    public sealed class DataEntityBuilder
    {
        private readonly DataPropertyBuilder _propertyBuilder;
        private readonly ConcurrentDictionary<string, Lazy<Delegate>> _cacheDictionary;
        private readonly IInstanceCreator _instanceCreator;

        private readonly static MethodInfo _builder =
            typeof(DataEntityBuilder).GetMethod(
                "DoBuilder",
                new Type[] { typeof(Type), typeof(DataOptions), typeof(DataPropertyBuilder), typeof(IInstanceCreator) })!;

        private readonly static MethodInfo _builderGeneric =
            typeof(DataEntityBuilder).GetMethod(
                "DoBuilder",
                new Type[] { typeof(DataOptions), typeof(DataPropertyBuilder), typeof(IInstanceCreator) })!;

        /// <summary>
        /// Initializes a new instance of <see cref="DataEntityBuilder"/>.
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="instanceCreator"></param>
        public DataEntityBuilder(DataPropertyBuilder propertyBuilder, IInstanceCreator instanceCreator)
        {
            _propertyBuilder = propertyBuilder ?? throw new ArgumentNullException(nameof(propertyBuilder));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _cacheDictionary = new ConcurrentDictionary<string, Lazy<Delegate>>();
        }

        /// <summary>
        /// Builds an implementation of <see cref="DataEntity{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>An implementation of <see cref="DataEntity{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">Unable to build an instance. See inner exception.</exception>
        public DataEntity<T> Build<T>(DataOptions options)
            where T : class, new() => GetEntityBuilder<T>(options);

        /// <summary>
        /// Builds an implementation of <see cref="DataEntity"/>.
        /// </summary>
        /// <param name="type">The type of the class.</param>
        /// <param name="options"></param>
        /// <returns>An instance of <see cref="DataEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to build an instance. See inner exception.</exception>
        public DataEntity Build(Type type, DataOptions options) => GetEntityBuilder(type, options);

        /// <summary>
        /// Builds the <see cref="DataEntity"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="options"></param>
        private DataEntity GetEntityBuilder(Type type, DataOptions options)
        {
            var builder = GetEntityBuilderDelegate<Func<Type, DataOptions, DataPropertyBuilder, IInstanceCreator, DataEntity>>(type);
            return builder.Invoke(type, options, _propertyBuilder, _instanceCreator);
        }

        /// <summary>
        /// Builds the <see cref="DataEntity{T}"/> from the generic type.
        /// </summary>
        /// <typeparam name="T">the type of the entity to act on.</typeparam>
        private DataEntity<T> GetEntityBuilder<T>(DataOptions options)
            where T : class, new()
        {
            var builder = GetEntityBuilderDelegate<Func<DataOptions, DataPropertyBuilder, IInstanceCreator, DataEntity<T>>>(typeof(T), true);
            return builder.Invoke(options, _propertyBuilder, _instanceCreator);
        }

        /// <summary>
        /// Builds the key used for caching delegate.
        /// </summary>
        /// <param name="type">The type of the entity.</param>
        /// <param name="isGeneric">Determines whether the key is generic or not.</param>
        private string KeyBuilder(Type type, bool isGeneric)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            var key = type!.FullName;
            if (isGeneric) key += "'1";

            return key!;
        }

        /// <summary>
        /// Provides with the <see cref="DataEntity"/> builder delegate from type.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate to return.</typeparam>
        /// <param name="type">The type of the entity.</param>
        /// <param name="isGeneric">Whether or not it's a generic process.</param>
        private TDelegate GetEntityBuilderDelegate<TDelegate>(Type type, bool isGeneric = false)
            where TDelegate : Delegate
        {
            var key = KeyBuilder(type, isGeneric);
            var valueFound = _cacheDictionary.GetOrAdd(
                key, _ => new Lazy<Delegate>(() => !isGeneric ? GetTypeBuilderDelegate<TDelegate>() : GetGenericBuilderDelegate<TDelegate>(type)));

            return (TDelegate)valueFound.Value;
        }

        /// <summary>
        /// Constructs the delegate for an entity type.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate.</typeparam>
        private TDelegate GetTypeBuilderDelegate<TDelegate>()
            where TDelegate : Delegate
        {
            var typeExpression = Expression.Parameter(typeof(Type));
            var optionExpression = Expression.Parameter(typeof(DataOptions));
            var propertyExpression = Expression.Parameter(typeof(DataPropertyBuilder));
            var creatorExpression = Expression.Parameter(typeof(IInstanceCreator));

            var parameters = new ParameterExpression[] { typeExpression, optionExpression, propertyExpression, creatorExpression };

            var methodCall = Expression.Call(_builder, parameters);
            return Expression.Lambda<TDelegate>(methodCall, parameters).Compile();
        }

        /// <summary>
        /// Constructs the delegate for generic type.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="type">The type to act on.</param>
        private TDelegate GetGenericBuilderDelegate<TDelegate>(Type type)
            where TDelegate : Delegate
        {
            var optionExpression = Expression.Parameter(typeof(DataOptions));
            var propertyExpression = Expression.Parameter(typeof(DataPropertyBuilder));
            var creatorExpression = Expression.Parameter(typeof(IInstanceCreator));

            var method = _builderGeneric.MakeGenericMethod(type);
            var parameters = new ParameterExpression[] { optionExpression, propertyExpression, creatorExpression };

            var methodCall = Expression.Call(method, parameters);
            return Expression.Lambda<TDelegate>(methodCall, parameters).Compile();
        }

        /// <summary>
        /// Builds the <see cref="DataEntity"/> with provided arguments.
        /// </summary>
        /// <param name="type">The type of entity.</param>
        /// <param name="options">The execute options.</param>
        /// <param name="propertyBuilder">the property builder.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        public static DataEntity DoBuilder(Type type, DataOptions options, DataPropertyBuilder propertyBuilder, IInstanceCreator instanceCreator)
        {
            var parameters = BuildParameters(type);
            var sourceProperties = parameters.Type.GetProperties()
                .Where(propertyInfo => propertyInfo.GetGetMethod() != null && propertyInfo.GetSetMethod() != null)
                .ToArray();
            var properties = default(IEnumerable<DataProperty>);

            if (options.IsMappableEnabled)
            {
                properties = sourceProperties
                  .Select(propertyInfo => propertyBuilder.Build(
                      new DataPropertySource(propertyInfo, options, parameters.IdentityProperties)))
                  .Where(options.IsMappable!);
            }
            else
            {
                properties = sourceProperties
                  .Where(property =>
                    (!options.ContainsExceptNames
                        || !options.ExceptNames.TryGetValue(type, out var values)
                        || !values.Contains(property.Name))
                        && property.GetCustomAttribute<DataExceptAttribute>(true) is null)
                  .Select(propertyInfo => propertyBuilder.Build(
                      new DataPropertySource(propertyInfo, options, parameters.IdentityProperties)));
            }

            var dataMapperEntity = new DataEntity(type, properties);
            dataMapperEntity.SetEntity(dataMapperEntity.CreateEntity(instanceCreator));

            if (options.IdentityBuilder != null)
                dataMapperEntity.AddIdentityBuilder(options.IdentityBuilder);

            return dataMapperEntity;
        }

        /// <summary>
        /// Builds the <see cref="DataEntity{T}"/> with provided arguments.
        /// </summary>
        /// <typeparam name="T">the generic type of entity.</typeparam>
        /// <param name="options">The execute options.</param>
        /// <param name="propertyBuilder">the property builder.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        public static DataEntity<T> DoBuilder<T>(DataOptions options, DataPropertyBuilder propertyBuilder, IInstanceCreator instanceCreator)
            where T : class, new()
        {
            var parameters = BuildParameters(typeof(T));
            var sourceProperties = parameters.Type.GetProperties()
                .Where(propertyInfo => propertyInfo.GetGetMethod() != null && propertyInfo.GetSetMethod() != null)
                .ToArray();
            var properties = default(IEnumerable<DataProperty<T>>);

            if (options.IsMappableEnabled)
            {
                properties = sourceProperties
                  .Select(propertyInfo => propertyBuilder.Build<T>(
                      new DataPropertySource(propertyInfo, options, parameters.IdentityProperties)))
                  .Where(options.IsMappableGeneric<T>()!);
            }
            else
            {
                properties = sourceProperties
                  .Where(property =>
                    (!options.ContainsExceptNames
                        || !options.ExceptNames.TryGetValue(typeof(T), out var values)
                        || !values.Contains(property.Name))
&& property.GetCustomAttribute<DataExceptAttribute>(true) is null)
                  .Select(propertyInfo => propertyBuilder.Build<T>(
                      new DataPropertySource(propertyInfo, options, parameters.IdentityProperties)));
            }

            var dataMapperEntity = new DataEntity<T>(typeof(T), properties);
            dataMapperEntity.SetEntity(dataMapperEntity.CreateEntity(instanceCreator));

            if (options.IdentityBuilder != null)
                dataMapperEntity.AddIdentityBuilder(options.IdentityBuilder);

            return dataMapperEntity;
        }

        /// <summary>
        /// Builds parameters for the specified type.
        /// </summary>
        /// <param name="source">The type to act on.</param>
        private static (string[] IdentityProperties, Type Type) BuildParameters(Type source)
        {
            var type = source ?? throw new ArgumentNullException(nameof(source));

            if (type.IsEnumerable())
                type = type.GetGenericArguments()[0];
            else if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type);

            var keys = type!.GetCustomAttribute<DataKeysAttribute>()?.Keys ?? Array.Empty<string>();

            return (keys, type!);
        }
    }
}
