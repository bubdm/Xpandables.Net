
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using Xpandables.Net.Data.Attributes;
using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Allows application author to build <see cref="DataOptions"/>.
    /// </summary>
    public sealed class DataOptionsBuilder
    {
        private IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;
        private ThreadOption _threadOptions;
        private ReaderOption _readerOption;
        private CancellationToken _cancellationToken = CancellationToken.None;
        private bool _isTransactionEnabled;
        private Func<IDataProperty, bool>? _isMappable;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> _dataNames
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();
        private readonly ConcurrentDictionary<Type, HashSet<string>> _exceptNames = new ConcurrentDictionary<Type, HashSet<string>>();
        private readonly ConcurrentDictionary<Type, DataPropertyConverter> _converters = new ConcurrentDictionary<Type, DataPropertyConverter>();
        private DataIdentityBuilder? _identityBuilder;
        private bool _isIdentityRetrieved;

        /// <summary>
        /// Returns a new instance of <see cref="DataOptions"/> using registered information.
        /// </summary>
        public DataOptions Build()
            => new DataOptions(
                _isTransactionEnabled,
                _isolationLevel,
                _isMappable,
                _dataNames,
                _exceptNames,
                _converters,
                _threadOptions,
                _readerOption,
                _identityBuilder,
                _isIdentityRetrieved,
                _cancellationToken);

        /// <summary>
        /// Returns a the default instance of <see cref="DataOptions"/>.
        /// </summary>
        public DataOptions BuildDefault()
            => new DataOptions(
                false,
                IsolationLevel.ReadCommitted,
                default,
                _dataNames,
                _exceptNames,
                _converters,
                ThreadOption.Normal,
                ReaderOption.DataAdapter,
                default,
                false,
                _cancellationToken);

        /// <summary>
        /// Enables use of transaction. The transaction will be closed just after the current execution.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to be used. The default value used by the server is <see cref="IsolationLevel.ReadCommitted"/> </param>
        public DataOptionsBuilder AddTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _isTransactionEnabled = true;
            _isolationLevel = isolationLevel;
            return this;
        }

        /// <summary>
        /// This is the highest level of mapping.
        /// Adds a delegate that determines whether or not a property should be mapped.
        /// The delegate will received an instance of the processing property and should return <see langword="true"/> if the property should be
        /// mapped, otherwise <see langword="false"/>. This action should be used for complex mapping and be aware of the performance impact.
        /// <para>The definition here takes priority over all attributes <see cref="DataNotMappedAttribute"/>
        /// and other <see cref="AddNotMappedName{T}(Expression{Func{T, string}})"/>.</para>
        /// </summary>
        /// <param name="mappable">The delegate that determine if a property is used or not.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="mappable"/> is null.</exception>
        public DataOptionsBuilder AddMappable(Func<IDataProperty, bool> mappable)
        {
            _isMappable = mappable ?? throw new ArgumentNullException(nameof(mappable));
            return this;
        }

        /// <summary>
        /// Bounds the specified model property name to the column data row specific name.
        /// Behaves like the <see cref="DataNameAttribute"/> attribute but takes priority over this attribute
        /// and <see cref="DataPrefixAttribute"/> attribute.
        /// Does not works for nested type, you have to provide mapper name for nested properties.
        /// You can add many mappers for various properties.
        /// </summary>
        /// <typeparam name="T">The type of the target model.</typeparam>
        /// <param name="propertySelector">The model property selector. We advise use of <see langword="nameof(model.PropertyName)"/>
        /// The property name must match a real property name.</param>
        /// <param name="sourceColumnName">The name of the column in the data row.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertySelector"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceColumnName"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">The property can no be bound to the data row property.</exception>
        public DataOptionsBuilder AddDataName<T>(Expression<Func<T, string>> propertySelector, string sourceColumnName)
            where T : class
        {
            if (propertySelector is null) throw new ArgumentNullException(nameof(propertySelector));
            if (string.IsNullOrWhiteSpace(sourceColumnName)) throw new ArgumentNullException(nameof(sourceColumnName));

            var property = propertySelector.Body is ConstantExpression constantExpression
                ? constantExpression.Value!.ToString() : GetMemberNameFromExpression(propertySelector);

            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException($"The parameter {nameof(propertySelector)} is not a valid expression.");

            if (typeof(T).GetProperty(property) is null)
                throw new ArgumentException($"The parameter {nameof(propertySelector)} does not exist in the {typeof(T).Name} type.");

            var mapping = _dataNames.GetOrAdd(
                typeof(T),
                _ => new ConcurrentDictionary<string, string>(new[] { new KeyValuePair<string, string>(property, sourceColumnName) }));

            mapping.AddOrUpdate(property, sourceColumnName, (_, __) => sourceColumnName);

            return this;
        }

        /// <summary>
        /// Specifies that selected property of the model must not be bound to the result.
        /// Behaves like the <see cref="DataNotMappedAttribute"/> attribute but takes priority over this attribute.
        /// You can add many mappers for various properties.
        /// </summary>
        /// <typeparam name="T">The type of the target model.</typeparam>
        /// <param name="propertySelector">The model property selector. We advise use of <see langword="nameof(model.PropertyName)"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertySelector"/> is null.</exception>
        /// <exception cref="ArgumentException">The property can no be found in the target type.</exception>
        /// <exception cref="ArgumentException">The property already exist in the filter of target type.</exception>
        public DataOptionsBuilder AddNotMappedName<T>(Expression<Func<T, string>> propertySelector)
            where T : class
        {
            if (propertySelector is null) throw new ArgumentNullException(nameof(propertySelector));

            var property = propertySelector.Body is ConstantExpression constantExpression
                ? constantExpression.Value!.ToString() : GetMemberNameFromExpression(propertySelector);

            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException($"The parameter {nameof(propertySelector)} is not a valid expression ({nameof(ConstantExpression)} or {nameof(MemberExpression)})");

            if (typeof(T).GetProperty(property) is null)
                throw new ArgumentException($"The parameter {nameof(propertySelector)} does not exist in the {typeof(T).Name} type.");

            var filtering = _exceptNames.GetOrAdd(typeof(T), _ => new HashSet<string>());

            if (!filtering.Add(property))
                throw new ArgumentException($"{property} already exist in the filter for {typeof(T).Name}");

            return this;
        }

        /// <summary>
        /// Specifies that selected properties of the model must not be bound to the result.
        /// Behaves like the <see cref="DataNotMappedAttribute"/> attribute but takes priority over this attribute.
        /// Does not works for nested type, you have to provide not mapped for nested types.
        /// You can add many mappers for various types.
        /// </summary>
        /// <typeparam name="T">The type of the target model.</typeparam>
        /// <param name="propertySelectors">The collection of model properties selector. We advise use of <see langword="nameof(model.PropertyName)"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertySelectors"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">The property already exist in the filter of target type.</exception>
        public DataOptionsBuilder AddNotMappedNames<T>(params Expression<Func<T, string>>[] propertySelectors)
            where T : class
        {
            if (propertySelectors?.Any() != true) throw new ArgumentNullException($"{nameof(propertySelectors)} is null or empty");

            foreach (var propertySelector in propertySelectors)
                AddNotMappedName(propertySelector);

            return this;
        }

        /// <summary>
        /// Specifies that collection of property names of the model must not be bound to the result.
        /// Behaves like the <see cref="DataNotMappedAttribute"/> attribute but takes priority over this attribute.
        /// Does not works for nested type, you have to provide not mapped for nested types.
        /// You can add many mappers for various types.
        /// </summary>
        /// <typeparam name="T">The type of the target model.</typeparam>
        /// <param name="propertyNames">A collection of string property names that shouldn't be bound to the result.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="propertyNames"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">The model does not contains the specified property.</exception>
        /// <exception cref="ArgumentException">The property already exist in the filter of target type.</exception>
        public DataOptionsBuilder AddNotMappedNames<T>(params string[] propertyNames)
            where T : class
        {
            if (propertyNames?.Any() != true) throw new ArgumentNullException($"{nameof(propertyNames)} is null or empty");

            var properties = typeof(T).GetProperties();
            foreach (var property in propertyNames)
            {
                if (Array.Find(properties, p => p.Name.Equals(property, StringComparison.OrdinalIgnoreCase)) is null)
                    throw new ArgumentException($"{typeof(T).Name} does not contains '{property}' name.");

                var filtering = _exceptNames.GetOrAdd(typeof(T), _ => new HashSet<string>());

                if (!filtering.Add(property))
                    throw new ArgumentException($"{property} already exist in the filter for {typeof(T).Name}");
            }

            return this;
        }

        /// <summary>
        /// Adds a delegate to be used to build entity identity.
        /// The value is used to uniquely identify an entity instance.
        /// The delegate will received an instance of the target entity.
        /// The definition here takes priority over the <see cref="DataKeysAttribute"/> attributes.
        /// </summary>
        /// <param name="identityBuilder">The delegate to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="identityBuilder"/> is null.</exception>
        public DataOptionsBuilder AddIdentityBuilder(DataIdentityBuilder identityBuilder)
        {
            _identityBuilder = identityBuilder ?? throw new ArgumentNullException(nameof(identityBuilder));
            return this;
        }

        /// <summary>
        /// Adds a delegate to be used for converting data row value to the target type.
        /// The delegate will receive an instance of the target property and the value from the data row.
        /// You can add many converters for various type.
        /// <para>The <typeparamref name="TType"/> should be a primitive type, <see cref="DateTime"/> or a <see cref="string"/> type.</para>
        /// The definition here takes priority over the mapper attributes.
        /// </summary>
        /// <typeparam name="TType">The type of the property the converter should be applied to its value.</typeparam>
        /// <param name="converter">The delegate to be used to convert a data row value for the target <typeparamref name="TType"/> type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public DataOptionsBuilder AddConverter<TType>(DataPropertyConverter converter)
        {
            if (!typeof(TType).IsPrimitive
                && typeof(TType) != typeof(DateTime)
                && typeof(TType) != typeof(DateTime?)
                && typeof(TType) != typeof(string))
            {
                throw new ArgumentException($"Primitives type or string type expected.");
            }

            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            _converters.AddOrUpdate(typeof(TType), converter, (_, __) => converter);
            return this;
        }

        /// <summary>
        /// Defines the execution thread options.
        /// </summary>
        /// <param name="threadOptions">the thread options to be used.</param>
        public DataOptionsBuilder AddThreadOptions(ThreadOption threadOptions)
        {
            _threadOptions = threadOptions;
            return this;
        }

        /// <summary>
        /// Defines the data type option to retrieve data from database.
        /// Only for 'select' methods.
        /// </summary>
        /// <param name="readerOptions">the thread options to be used.</param>
        public DataOptionsBuilder AddReaderOptions(ReaderOption readerOptions)
        {
            _readerOption = readerOptions;
            return this;
        }

        /// <summary>
        /// Defines the cancellation token for the execution.
        /// The default used value is <see cref="CancellationToken.None"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to be used.</param>
        public DataOptionsBuilder AddCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Defines that the current execution should retrieve the last identity from the query.
        /// Be aware of the fact that the query must be an insertion, otherwise you will face an exception.
        /// </summary>
        public DataOptionsBuilder UseRetrievedIdentity()
        {
            _isIdentityRetrieved = true;
            return this;
        }

        /// <summary>
        /// Returns the member name from the expression if found, otherwise returns null.
        /// </summary>
        /// <typeparam name="T">The type of the model class.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member name.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        private static string? GetMemberNameFromExpression<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
            where T : class
        {
            if (propertyExpression is null) throw new ArgumentNullException(nameof(propertyExpression));

            return (propertyExpression.Body as MemberExpression
                ?? ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression)
                ?.Member.Name;
        }
    }
}
