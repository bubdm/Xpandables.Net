
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

using Xpandables.Net5.Creators;

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Maps data to entities.
    /// </summary>
    public sealed class DataMapper
    {
        private readonly DataMapperRow _dataMapperRow;
        private readonly ConcurrentDictionary<string, DataEntity> _entities = new ConcurrentDictionary<string, DataEntity>();

        /// <summary>
        /// Initializes a new instance of <see cref="DataMapper"/>.
        /// </summary>
        public DataMapper(IInstanceCreator instanceCreator, DataEntityBuilder entityBuilder)
        {
            _dataMapperRow = new DataMapperRow(instanceCreator, entityBuilder, _entities);
        }

        /// <summary>
        /// Maps the data table to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of expected result..</typeparam>
        /// <param name="source">The data table to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public List<T> Map<T>(DataTable source, DataOptions options)
            where T : class, new()
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (options is null) throw new ArgumentNullException(nameof(options));

            return DoMap<T>(source, options, _entities);
        }

        /// <summary>
        /// Maps the data reader to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="source">The data reader to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public List<T> Map<T>(IEnumerable<IDataRecord> source, DataOptions options)
            where T : class, new()
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (options is null) throw new ArgumentNullException(nameof(options));

            return DoMap<T>(source, options, _entities);
        }

        private List<T> DoMap<T>(object source, DataOptions options, ConcurrentDictionary<string, DataEntity> entities)
                 where T : class, new()
        {
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
                        ThreadNormal(source, typeof(T), options);
                        break;
                    case ThreadOption.SpeedUp:
                        ThreadSpeedUp(source, typeof(T), options, parallelOptions);
                        break;
                    case ThreadOption.Expensive:
                        ThreadExpensive(source, typeof(T), options, parallelOptions);
                        break;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
            {
                exceptions.Enqueue(exception);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            if (!exceptions.IsEmpty)
                throw new AggregateException(exceptions);

            return entities.Select(entity => (T)entity.Value.Entity!).ToList();
        }

        private void ThreadNormal(object source, Type entityType, DataOptions options)
        {
            switch (source)
            {
                case DataTable dataTable:
                    foreach (var row in dataTable.AsEnumerable())
                        _dataMapperRow.Map(row, entityType, options);
                    break;
                case IEnumerable<IDataRecord> dataRecords:
                    foreach (var row in dataRecords)
                        _dataMapperRow.Map(row, entityType, options);
                    break;
            }
        }

        private void ThreadSpeedUp(object source, Type entityType, DataOptions options, ParallelOptions parallelOptions)
        {
            switch (source)
            {
                case DataTable dataTable:
                    Partitioner
                        .Create(dataTable.AsEnumerable())
                        .AsParallel()
                        .WithCancellation(parallelOptions.CancellationToken)
                        .WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism)
                        .ForAll(row => _dataMapperRow.Map(row, entityType, options));
                    break;
                case IEnumerable<IDataRecord> dataRecords:
                    foreach (var row in dataRecords)
                        _dataMapperRow.Map(row, entityType, options);
                    break;
            }
        }

        private void ThreadExpensive(object source, Type entityType, DataOptions options, ParallelOptions parallelOptions)
        {
            switch (source)
            {
                case DataTable dataTable:
                    Parallel.For(
                        0,
                        dataTable.Rows.Count,
                        parallelOptions,
                        index => _dataMapperRow.Map(dataTable.Rows[index], entityType, options));

                    break;
                case IEnumerable<IDataRecord> dataRecords:
                    foreach (var row in dataRecords)
                        _dataMapperRow.Map(row, entityType, options);
                    break;
            }
        }
    }
}
