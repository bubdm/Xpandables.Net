
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Correlation;
using Xpandables.Net.Data.Elements;
using Xpandables.Net.Extensions;

namespace Xpandables.Net.Data.Mappers
{
    /// <summary>
    /// Provides with a method to map a source to entities.
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Asynchronously maps the data source to the specified type and returns a collection of that type.
        /// </summary>
        /// <typeparam name="TEntity">The type of expected result.</typeparam>
        /// <param name="source">The data source to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public async IAsyncEnumerable<TEntity> MapAsync<TEntity>(
            IAsyncEnumerable<IDataRecord> source, DataOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TEntity : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            Entities.Clear();

            await source
              .ParallelForEachAsync(
                  record => DataMapperRow.Map<TEntity>(record, options),
                  Environment.ProcessorCount * 4,
                  TaskScheduler.FromCurrentSynchronizationContext(),
                  cancellationToken)
              .ConfigureAwait(false);

            foreach (var data in Entities.Select(entity => (TEntity)entity.Value.Entity!))
                yield return data;
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
