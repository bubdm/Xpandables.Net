
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
using System.Data;
using System.Threading;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Determines the way to retrieve data from the database.
    /// </summary>
    public enum ReaderOption
    {
        /// <summary>
        /// Reads data using the data adapter type. This is the default value.
        /// </summary>
        DataAdapter,

        /// <summary>
        /// Reads data using the data reader type.
        /// </summary>
        DataReader
    }

    /// <summary>
    /// Determines the algorithm to be applied.
    /// </summary>
    public enum ThreadOptions
    {
        /// <summary>
        /// A normal for each...
        /// </summary>
        Normal,

        /// <summary>
        /// Use of partitioner with parallel execution. Not available when used with <see cref="ReaderOption.DataReader"/>.
        /// </summary>
        SpeedUp,

        /// <summary>
        /// Use of parallel. Not available when used with <see cref="ReaderOption.DataReader"/>.
        /// </summary>
        Expensive
    }

    /// <summary>
    /// Contains execution behaviors. Use <see cref="DataOptionsBuilder"/> to build options.
    /// </summary>
    public sealed class DataOptions
    {
        internal DataOptions(bool isTransactionEnabled, IsolationLevel isolationLevel, Func<DataProperty, bool>? isMappable, ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> dataNames, ConcurrentDictionary<Type, HashSet<string>> exceptNames, ConcurrentDictionary<Type, DataPropertyConverter> converters, ThreadOptions threadOptions, ReaderOption readerOptions, DataIdentityBuilder? identityBuilder, bool isIdentityRetrieved, CancellationToken cancellationToken)
        {
            IsTransactionEnabled = isTransactionEnabled;
            IsolationLevel = isolationLevel;
            IsMappable = isMappable;
            DataNames = dataNames ?? throw new ArgumentNullException(nameof(dataNames));
            ExceptNames = exceptNames ?? throw new ArgumentNullException(nameof(exceptNames));
            Converters = converters ?? throw new ArgumentNullException(nameof(converters));
            ThreadOptions = threadOptions;
            ReaderOptions = readerOptions;
            CancellationToken = cancellationToken;
            IdentityBuilder = identityBuilder;
            IsIdentityRetrieved = isIdentityRetrieved;
        }

        /// <summary>
        /// Determines whether or not to use transaction. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool IsTransactionEnabled { get; }

        /// <summary>
        /// Determines the isolation level for transaction. Only used if <see cref="IsTransactionEnabled"/> is <see langword="true"/>.
        /// </summary>
        public IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Defines the delegate that determines whether or not a property should be mapped.
        /// Its default behavior return <see langword="true"/>.
        /// </summary>
        public Func<DataProperty, bool>? IsMappable { get; }

        /// <summary>
        /// Determines whether or not the conditional mapping has been defined. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool IsMappableEnabled => IsMappable is { };

        /// <summary>
        /// Returns the generic type of conditional mapper.
        /// </summary>
        /// <typeparam name="T">The type of property.</typeparam>
        public Func<DataProperty<T>, bool>? IsMappableGeneric<T>()
            where T : class, new() => IsMappable;

        /// <summary>
        /// Contains a collection of manual names mapping.
        /// </summary>
        public ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> DataNames { get; }
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();

        /// <summary>
        /// Contains a collection of manual names from types that should not to be mapped.
        /// </summary>
        public ConcurrentDictionary<Type, HashSet<string>> ExceptNames { get; }
            = new ConcurrentDictionary<Type, HashSet<string>>();

        /// <summary>
        /// Contains a collection of converters.
        /// </summary>
        public ConcurrentDictionary<Type, DataPropertyConverter> Converters { get; }
            = new ConcurrentDictionary<Type, DataPropertyConverter>();

        /// <summary>
        /// Defines the thread execution options. The default value is <see cref="ThreadOptions.Normal"/>.
        /// </summary>
        public ThreadOptions ThreadOptions { get; }

        /// <summary>
        /// Defines the data type option to use to retrieve data from database. The default value is <see cref="ReaderOption.DataAdapter"/>.
        /// </summary>
        public ReaderOption ReaderOptions { get; }

        /// <summary>
        /// Contains the cancellation token to be used.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; } = CancellationToken.None;

        /// <summary>
        /// Determines whether or not the filtered delegate has been defined. The default value is <see langword="false"/>
        /// </summary>
        public bool ContainsExceptNames => ExceptNames.Count > 0;

        /// <summary>
        /// Contains the entity identity builder delegate.
        /// </summary>
        public DataIdentityBuilder? IdentityBuilder { get; }

        /// <summary>
        /// Determines whether or not to retrieve the newly created identity from sql command.
        /// </summary>
        public bool IsIdentityRetrieved { get; }
    }
}
