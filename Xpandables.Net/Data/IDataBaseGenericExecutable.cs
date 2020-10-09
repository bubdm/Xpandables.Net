
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
using System.Data;
using System.Threading.Tasks;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    public partial interface IDataBase
    {
        /// <summary>
        /// Asynchronously executes a command/query with the specified connection, options and executable <typeparamref name="TDataExecutable" /> type against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutable" /> type inherits from <see cref="DataExecutable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable. The class inherits from <see cref="DataExecutable{T}" />.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TDataExecutable>(
            IDataConnectionOptions dataConnection,
            IDataOptions dataOptions,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutable : DataExecutable<TResult>
        {
            var executable = DataExecutableProvider.GetDataExecutable<TResult, TDataExecutable>()
                ?? throw new ArgumentException($"{typeof(TDataExecutable).Name} not found !");

            return await ExecuteAsync(dataConnection, dataOptions, executable, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified options and executable <typeparamref name="TDataExecutable" /> type against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutable" /> type inherits from <see cref="DataExecutable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable. The class inherits from <see cref="DataExecutable{T}" />.</typeparam>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TDataExecutable>(
            IDataOptions dataOptions,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutable : DataExecutable<TResult>
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");

            return await ExecuteAsync<TResult, TDataExecutable>(DataConnection, dataOptions, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified connection and executable <typeparamref name="TDataExecutable" /> type against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutable" /> type inherits from <see cref="DataExecutable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable. The class inherits from <see cref="DataExecutable{T}" />.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TDataExecutable>(
            IDataConnectionOptions dataConnection,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutable : DataExecutable<TResult>
        {
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            return await ExecuteAsync<TResult, TDataExecutable>(dataConnection, DataOptions, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TDataExecutable" /> type against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutable" /> type inherits from <see cref="DataExecutable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable. The class inherits from <see cref="DataExecutable{T}" />.</typeparam>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TDataExecutable>(
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutable : DataExecutable<TResult>
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            return await ExecuteAsync<TResult, TDataExecutable>(DataConnection, DataOptions, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified options and executable against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutable">The data executable instance to be used.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutable" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult>(
            IDataOptions dataOptions,
            DataExecutable<TResult> dataExecutable,
            string commandText,
            CommandType commandType,
            params object[] parameters)
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");

            return await ExecuteAsync(DataConnection, dataOptions, dataExecutable, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified connection and executable against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutable">The data executable instance to be used.</param>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutable" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult>(
            IDataConnectionOptions dataConnection,
            DataExecutable<TResult> dataExecutable,
            string commandText,
            CommandType commandType,
            params object[] parameters)
        {
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            return await ExecuteAsync(dataConnection, DataOptions, dataExecutable, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutable">The data executable instance to be used.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutable" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult>(
            DataExecutable<TResult> dataExecutable,
            string commandText,
            CommandType commandType,
            params object[] parameters)
        {
            _ = DataConnection ?? throw new ArgumentNullException(nameof(IDataConnectionOptions), "You must initialize the database instance with a default data connection.");
            _ = DataOptions ?? throw new ArgumentNullException(nameof(IDataOptions), "You must initialize the database instance with a default data options.");

            return await ExecuteAsync(DataConnection, DataOptions, dataExecutable, commandText, commandType, parameters).ConfigureAwait(false);
        }
    }
}
