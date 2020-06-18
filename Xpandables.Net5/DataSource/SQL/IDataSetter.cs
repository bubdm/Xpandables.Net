
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
using System.Data;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Allows an application author to set an entity property with a data reader / data row value.
    /// </summary>
    public interface IDataSetter
    {
        /// <summary>
        /// Sets an entity property with the data record value.
        /// </summary>
        /// <param name="dataRecord">The data record row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        void Set(IDataRecord dataRecord, object entity, IInstanceCreator instanceCreator);

        /// <summary>
        /// Sets an entity property with the data row value.
        /// </summary>
        /// <param name="dataRow">The data row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        void Set(DataRow dataRow, object entity, IInstanceCreator instanceCreator);

        /// <summary>
        /// Sets the entity property with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        void Set(object value, object entity, IInstanceCreator instanceCreator);
    }

    /// <summary>
    /// Allows an application author to set a specific entity property with a data reader / data row value.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IDataBaseMapperPropertySetter<T> : IDataSetter
        where T : class
    {
        /// <summary>
        /// Sets an entity property with the data record value.
        /// </summary>
        /// <param name="dataRecord">The data record row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        void Set(IDataRecord dataRecord, T entity, IInstanceCreator instanceCreator);

        /// <summary>
        /// Sets an entity property with the data row value.
        /// </summary>
        /// <param name="dataRow">The data row to be used.</param>
        /// <param name="entity">The entity instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        void Set(DataRow dataRow, T entity, IInstanceCreator instanceCreator);
    }
}
