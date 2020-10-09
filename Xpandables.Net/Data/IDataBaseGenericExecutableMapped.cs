
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

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data
{
    public partial interface IDataBase
    {
        /// <summary>
        /// Asynchronously executes a command/query with the specified connection, options and executable <typeparamref name="TDataExecutableMapped" /> type against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutableMapped" /> type inherits from <see cref="DataExecutableMapper{TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapped">The type of the executable. The class inherits from <see cref="DataExecutableBuilder{TResult}" />.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TDataExecutableMapped>(
            IDataConnectionOptions dataConnection,
            IDataOptions dataOptions,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutableMapped : DataExecutableMapper<TResult>
            where TResult : class, new()
        {
            var executableMapped = DataExecutableProvider.GetDataExecutableMapper<TResult, TDataExecutableMapped>()
                       ?? throw new ArgumentException($"{typeof(TDataExecutableMapped).Name} not found !");

            await foreach (var result in ExecuteMappedAsync(dataConnection, dataOptions, executableMapped, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified options and executable <typeparamref name="TDataExecutableMapped" /> type against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutableMapped" /> type inherits from <see cref="DataExecutableMapper{TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapped">The type of the executable. The class inherits from <see cref="DataExecutableBuilder{TResult}" />.</typeparam>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TDataExecutableMapped>(
            IDataOptions dataOptions,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutableMapped : DataExecutableMapper<TResult>
            where TResult : class, new()
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");

            await foreach (var result in ExecuteMappedAsync<TResult, TDataExecutableMapped>(DataConnection, dataOptions, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified connection and executable <typeparamref name="TDataExecutableMapped" /> type against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutableMapped" /> type inherits from <see cref="DataExecutableMapper{TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapped">The type of the executable. The class inherits from <see cref="DataExecutableBuilder{TResult}" />.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TDataExecutableMapped>(
            IDataConnectionOptions dataConnection,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutableMapped : DataExecutableMapper<TResult>
            where TResult : class, new()
        {
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            await foreach (var result in ExecuteMappedAsync<TResult, TDataExecutableMapped>(dataConnection, DataOptions, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TDataExecutableMapped" /> type against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutableMapped" /> type inherits from <see cref="DataExecutableMapper{TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapped">The type of the executable. The class inherits from <see cref="DataExecutableBuilder{TResult}" />.</typeparam>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TDataExecutableMapped>(
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutableMapped : DataExecutableMapper<TResult>
            where TResult : class, new()
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            await foreach (var result in ExecuteMappedAsync<TResult, TDataExecutableMapped>(DataConnection, DataOptions, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable mapper against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutableMapper">The data executable mapper instance to be used.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult>(
            IDataOptions dataOptions,
            DataExecutableMapper<TResult> dataExecutableMapper,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TResult : class, new()
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");

            await foreach (var result in ExecuteMappedAsync(DataConnection, dataOptions, dataExecutableMapper, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable mapper against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutableMapper">The data executable mapper instance to be used.</param>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult>(
            IDataConnectionOptions dataConnection,
            DataExecutableMapper<TResult> dataExecutableMapper,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TResult : class, new()
        {
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            await foreach (var result in ExecuteMappedAsync(dataConnection, DataOptions, dataExecutableMapper, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable mapper against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutableMapper">The data executable mapper instance to be used.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult>(
            DataExecutableMapper<TResult> dataExecutableMapper,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TResult : class, new()
        {
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");

            await foreach (var result in ExecuteMappedAsync(DataConnection, DataOptions, dataExecutableMapper, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }
    }
}
