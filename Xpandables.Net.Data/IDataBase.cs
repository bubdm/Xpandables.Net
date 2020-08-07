
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Data.Providers;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with a method to execute command to a database using an implementation of <see cref="IDataExecutable{TResult}"/> interface.
    /// </summary>
    [Guid("B6D4EDC7-3548-442F-B5CA-7050A24DB32D")]
    [ComImport()]
    [CoClass(typeof(DataBase))]
    public interface IDataBase
    {
        internal IDataFactoryProvider DataFactoryProvider { get; }
        internal IDataExecutableProvider DataExecutableProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataConnection">The connection to be used You can use the <see cref="DataConnectionBuilder"/> to build a new instance.</param>
        /// <returns></returns>
        public IDataBaseConnection UseConnection(IDataConnection dataConnection)
            => new IDataBaseConnection(dataConnection, DataFactoryProvider, DataExecutableProvider);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataConnectionProvider"></param>
        /// <returns></returns>
        public IDataBaseConnection UseConnection(IDataConnectionProvider dataConnectionProvider)
            => new IDataBaseConnection(dataConnectionProvider.GetDataConnection(), DataFactoryProvider, DataExecutableProvider);

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TExecutable" /> type to the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="IDataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="IDataExecutable{T}" /> interface.</typeparam>
        /// <param name="dataConnection">The connection to be used You can use the <see cref="DataConnectionBuilder"/> to build a new instance.</param>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public virtual async Task<Optional<TResult>> ExecuteAsync<TResult, TExecutable>(
            IDataConnection dataConnection,
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TExecutable : class, IDataExecutable<TResult>
                => await new IDataBaseConnection(dataConnection, DataFactoryProvider, DataExecutableProvider)
                    .ExecuteAsync<TResult, TExecutable>(options, commandText, commandType, parameters)
                    .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TExecutableMapped" /> type to the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutableMapped" /> type must implement <see cref="IDataExecutable{TResult}" /> interface.
        /// The default implementation just returns an empty result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutableMapped">The type of the executable. The class must implement <see cref="IDataExecutable{TResult}" /> interface.</typeparam>
        /// <param name="dataConnection">The connection to be used You can use the <see cref="DataConnectionBuilder"/> to build a new instance.</param>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public virtual async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TExecutableMapped>(
            IDataConnection dataConnection,
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TExecutableMapped : class, IDataExecutableMapper<TResult>
        {
            await foreach (var result in new IDataBaseConnection(dataConnection, DataFactoryProvider, DataExecutableProvider)
                .ExecuteMappedAsync<TResult, TExecutableMapped>(options, commandText, commandType, parameters))
                yield return result;
        }
    }

    /// <summary>
    /// Provides with a default implementation of <see cref="IDataBase"/>.
    /// </summary>
    public sealed class DataBase : IDataBase
    {
        private readonly IDataFactoryProvider _dataFactoryProvider;
        private readonly IDataExecutableProvider _dataExecutableProvider;

        IDataExecutableProvider IDataBase.DataExecutableProvider => _dataExecutableProvider;
        IDataFactoryProvider IDataBase.DataFactoryProvider => _dataFactoryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFactoryProvider"></param>
        /// <param name="dataExecutableProvider"></param>
        public DataBase(IDataFactoryProvider dataFactoryProvider, IDataExecutableProvider dataExecutableProvider)
        {
            _dataFactoryProvider = dataFactoryProvider ?? throw new ArgumentNullException(nameof(dataFactoryProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
        }
    }
}
