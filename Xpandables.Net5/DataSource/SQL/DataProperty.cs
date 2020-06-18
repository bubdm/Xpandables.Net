
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
using System.Collections;
using System.Data;
using System.Reflection;

namespace System.Design.SQL
{
    /// <summary>
    /// Provides with custom information for a mapping associated with a property.
    /// </summary>
    public class DataProperty : IDataSetter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataProperty"/> with all arguments using cache.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="dataName">The data name from the source.</param>
        /// <param name="dataPrefix">The data prefix from the source.</param>
        /// <param name="isIdentity">Whether the property is an identity key or not.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo"/> is null.</exception>
        public DataProperty(
            string propertyName,
            string dataName,
            string? dataPrefix,
            bool isIdentity,
            PropertyInfo propertyInfo)
        {
            DataPrefix = dataPrefix;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            IsIdentity = isIdentity;
            Type = propertyInfo?.PropertyType ?? throw new ArgumentNullException(nameof(propertyInfo));

            Getter = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(new Type[] { propertyInfo.ReflectedType!, Type }), null, propertyInfo.GetMethod!);
            Setter = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(new Type[] { propertyInfo.ReflectedType!, Type }), null, propertyInfo.SetMethod!);

            IsPrimitive = Type.IsPrimitive || Type.FullName!.Equals("System.String");
            IsNullable = Type.IsNullable();
            IsEnumerable = Type.IsEnumerable();
        }

        /// <summary>
        /// Gets the target property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the name of the column in the data source.
        /// </summary>
        public string DataName { get; protected set; }

        /// <summary>
        /// Gets the prefix of the column in the data source.
        /// </summary>
        public string? DataPrefix { get; protected set; }

        /// <summary>
        /// Gets the full name of the column in the data source.
        /// </summary>
        public string DataFullName => $"{DataPrefix}{DataName}";

        /// <summary>
        /// Determine whether the property is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Determine whether the property is a value type|string or reference type.
        /// </summary>
        public bool IsPrimitive { get; }

        /// <summary>
        /// Determine whether the property is used for uniquely identify the data source.
        /// </summary>
        public bool IsIdentity { get; protected set; }

        /// <summary>
        /// Determine whether the property is a collection.
        /// </summary>
        public bool IsEnumerable { get; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the setter delegate for the property.
        /// </summary>
        public Delegate Setter { get; }

        /// <summary>
        /// Gets the getter delegate for the property.
        /// </summary>
        public Delegate Getter { get; }

        /// <summary>
        /// Gets the delegate used to convert the data row value to the expected type of property.
        /// The default behavior just returns the value.
        /// </summary>
        public DataPropertyConverter Converter { get; private set; } = (_, source) => source;

        /// <summary>
        /// Adds a converter for the underlying property. Useful when the data row can contain an unexpected value.
        /// </summary>
        /// <param name="converter">The converter to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="converter"/> is null.</exception>
        public DataProperty AddConverter(DataPropertyConverter converter)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            return this;
        }

        /// <summary>
        /// Defines a custom <see cref="DataName"/> for the underlying property.
        /// </summary>
        /// <param name="dataName">The new data name to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        public DataProperty SetDataName(string dataName)
        {
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            DataPrefix = null;
            return this;
        }

        /// <summary>
        /// Returns the property type.
        /// <para>If the entity type is nullable, returns <see langword="Nullable.GetUnderlyingType(Type)"/>.</para>
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public Type GetPropertyType()
        {
            var targetType = Type;
            if (IsNullable) targetType = Nullable.GetUnderlyingType(targetType);
            if (IsEnumerable) targetType = targetType!.GetGenericArguments()[0];

            return targetType!;
        }

        /// <summary>
        /// Returns the property type.
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public virtual Type GetPropertyStronglyType()
        {
            var targetType = Type;
            return IsNullable ? Nullable.GetUnderlyingType(targetType)! : targetType;
        }

        /// <summary>
        /// Creates an instance of the <see cref="System.Type"/>. The type must contains a parameterless constructor.
        /// </summary>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <returns>A new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public virtual object CreateProperty(IInstanceCreator instanceCreator)
            => instanceCreator?.Create(GetPropertyType()) ?? throw new ArgumentNullException(nameof(instanceCreator));

        /// <summary>
        /// Creates an instance of the exactly value to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <returns>An new instance of <see cref="System.Type"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public virtual object CreateStronglyTypedProperty(IInstanceCreator instanceCreator)
            => instanceCreator?.Create(GetPropertyStronglyType()) ?? throw new ArgumentNullException(nameof(instanceCreator));

        /// <summary>
        /// Sets the data row to the property.
        /// </summary>
        /// <param name="dataRow">The data row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public void Set(DataRow dataRow, object entity, IInstanceCreator instanceCreator)
        {
            if (dataRow is null) throw new ArgumentNullException(nameof(dataRow));
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            if (dataRow.Table.Columns.Contains(DataFullName))
            {
                Set(dataRow[DataFullName], entity, instanceCreator);
            }
        }

        /// <summary>
        /// Sets the data record to the property.
        /// </summary>
        /// <param name="dataRecord">The data record row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public void Set(IDataRecord dataRecord, object entity, IInstanceCreator instanceCreator)
        {
            if (dataRecord is null) throw new ArgumentNullException(nameof(dataRecord));
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            if (dataRecord.Contains(DataFullName))
            {
                var index = dataRecord.GetOrdinal(DataFullName);
                if (!dataRecord.IsDBNull(index))
                    Set(dataRecord.GetValue(index), entity, instanceCreator);
            }
        }

        /// <summary>
        /// Sets the entity property with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator" /> is null.</exception>
        public void Set(object value, object entity, IInstanceCreator instanceCreator)
        {
            var dataValue = Converter(this, value);
            if (dataValue is null || dataValue is DBNull)
                dataValue = null;
            var entityValue = entity;

            try
            {
                if (IsEnumerable)
                {
                    if (Getter.DynamicInvoke(entityValue) is null)
                        Setter.DynamicInvoke(entityValue, CreateStronglyTypedProperty(instanceCreator));

                    ((IList)Getter.DynamicInvoke(entityValue)!).Add(dataValue);
                }
                else
                {
                    Setter.DynamicInvoke(entityValue, dataValue);
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Mapping the property '{entityValue.GetType().Name}.{PropertyName}' of type '{Type.Name}' failed. Data row name '{DataFullName}',  Data row value : '{dataValue}'",
                    exception);
            }
        }
    }

    /// <summary>
    /// Implementation of a mapping associated with a property and a specify type class.
    /// </summary>
    /// <typeparam name="T">Type of the class.</typeparam>
    public class DataProperty<T> : DataProperty, IDataBaseMapperPropertySetter<T>
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataProperty{T}"/> with all arguments.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="dataName">The data name from the source.</param>
        /// <param name="dataPrefix">The data prefix from the source.</param>
        /// <param name="isIdentity">Whether the property is an identity key or not.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo"/> is null.</exception>
        public DataProperty(
            string propertyName,
            string dataName,
            string? dataPrefix,
            bool isIdentity,
            PropertyInfo propertyInfo)
            : base(propertyName, dataName, dataPrefix, isIdentity, propertyInfo) { }

        /// <summary>
        /// Sets the data row to the property.
        /// </summary>
        /// <param name="dataRow">The data row to act on.</param>
        /// <param name="entity">the entity to save to.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public void Set(DataRow dataRow, T entity, IInstanceCreator instanceCreator) => base.Set(dataRow, entity, instanceCreator);

        /// <summary>
        /// Sets the data reader to the property.
        /// </summary>
        /// <param name="dataRecord">The data reader to act on.</param>
        /// <param name="entity">the entity to save to.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public void Set(IDataRecord dataRecord, T entity, IInstanceCreator instanceCreator) => base.Set(dataRecord, entity, instanceCreator);

        /// <summary>
        /// Creates an instance of the exactly value to <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An new instance of <typeparamref name="T"/>.</returns>
        public new T CreateProperty(IInstanceCreator instanceCreator) => (T)base.CreateProperty(instanceCreator);

        /// <summary>
        /// Creates an instance of the exactly value to <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An new instance of <typeparamref name="T"/>.</returns>
        public new T CreateStronglyTypedProperty(IInstanceCreator instanceCreator) => (T)base.CreateStronglyTypedProperty(instanceCreator);
    }
}
