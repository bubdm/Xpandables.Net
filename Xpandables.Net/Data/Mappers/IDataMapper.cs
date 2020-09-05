
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
using System.Data;
using System.Threading.Tasks;

using Xpandables.Net.Correlation;
using Xpandables.Net.Data.Elements;
using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data.Mappers
{
    /// <summary>
    /// Provides with methods to map a source to entities.
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Asynchronously maps the data source to the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of expected result.</typeparam>
        /// <param name="source">The data source to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public async Task MapAsync<TEntity>(IAsyncEnumerable<IDataRecord> source, DataOptions options)
            where TEntity : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            Entities.Clear();

            await foreach (var row in source)
                DataMapperRow.Map<TEntity>(row, options);
        }

        /// <summary>
        /// Maps the record to the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of expected result.</typeparam>
        /// <param name="record">The data source to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="record"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public void Map<TEntity>(IDataRecord record, DataOptions options)
            where TEntity : class, new()
        {
            _ = record ?? throw new ArgumentNullException(nameof(record));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            DataMapperRow.Map<TEntity>(record, options);
        }

        /// <summary>
        /// Gets the data mapper row.
        /// </summary>
        IDataMapperRow DataMapperRow { get; }

        /// <summary>
        /// Gets the collection of built entities.
        /// </summary>
        CorrelationCollection<string, IDataEntity> Entities { get; }
    }

    /// <summary>
    /// Implementation of <see cref="IDataMapper"/>.
    /// </summary>
    public sealed class DataMapper : IDataMapper
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapper"/> class.
        /// </summary>
        /// <param name="dataMapperRow">The data mapper for row.</param>
        /// <param name="entities">The shared entities collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataMapperRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null.</exception>
        public DataMapper(IDataMapperRow dataMapperRow, CorrelationCollection<string, IDataEntity> entities)
        {
            DataMapperRow = dataMapperRow ?? throw new ArgumentNullException(nameof(dataMapperRow));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        /// <summary>
        /// Gets the data mapper row.
        /// </summary>
        public IDataMapperRow DataMapperRow { get; }

        /// <summary>
        /// Gets the collection of built entities.
        /// </summary>
        public CorrelationCollection<string, IDataEntity> Entities { get; }
    }
}
