
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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Correlation;
using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data.Mappers
{
    /// <summary>
    /// Provides with a method to map a source to entities.
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Maps the data source to the specified type.
        /// </summary>
        /// <typeparam name="TData">The type of expected result.</typeparam>
        /// <param name="source">The data source to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public List<TData> Map<TData>(object source, DataOptions options)
            where TData : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            Entities.Clear();

            var exceptions = new ConcurrentQueue<Exception>();
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = options.CancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount * 4
            };

            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                switch (options.ThreadOptions)
                {
                    case ThreadOption.Normal:
                        foreach (var row in GetTargetCollection(source))
                            DataMapperRow.Map(row, typeof(TData), options);
                        break;
                    case ThreadOption.SpeedUp:
                        Partitioner
                            .Create(GetTargetCollection(source))
                            .AsParallel()
                            .WithCancellation(parallelOptions.CancellationToken)
                            .WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism)
                            .ForAll(row => DataMapperRow.Map(row, typeof(TData), options));
                        break;
                    case ThreadOption.Expensive:
                        var array = GetTargetCollection(source).ToArray();
                        Parallel.For(
                            0,
                            array.Length,
                            parallelOptions,
                            index => DataMapperRow.Map(array[index], typeof(TData), options));
                        break;
                }
            }
            catch (Exception exception)
            {
                exceptions.Enqueue(exception);
            }

            if (!exceptions.IsEmpty)
                throw new AggregateException(exceptions);

            return Entities.Select(entity => (TData)entity.Value.Entity!).ToList();
        }

        /// <summary>
        /// Gets the data mapper row.
        /// </summary>
        IDataMapperRow DataMapperRow { get; }

        /// <summary>
        /// Gets the collection of built entities.
        /// </summary>
        CorrelationCollection<string, IDataEntity> Entities { get; }

        private static IEnumerable<object> GetTargetCollection(object dataSource)
            => dataSource switch
            {
                DataTable dataTable => dataTable.AsEnumerable(),
                IEnumerable<IDataRecord> enumerable => enumerable,
                IEnumerable<DataRow> dataRows => dataRows.AsEnumerable(),
                _ => throw new InvalidOperationException($"Invalid type collection : '{dataSource.GetType().Name}'")
            };
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
