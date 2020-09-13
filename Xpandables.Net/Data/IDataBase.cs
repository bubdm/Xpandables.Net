
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
using System.Data.Common;
using System.Threading.Tasks;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with methods to execute command against a database using an implementation of <see cref="DataExecutable{TResult}"/> or <see cref="DataExecutableMapper{TResult}"/>.
    /// </summary>
    public partial interface IDataBase
    {
        internal IDataConnectionContextProvider DataConnectionContextProvider { get; }
        internal IDataExecutableProvider DataExecutableProvider { get; }

        /// <summary>
        /// May contains the data connection used from initialization.
        /// </summary>
        IDataConnection? DataConnection { get; }

        /// <summary>
        /// May contains the data options used from initialization.
        /// </summary>
        IDataOptions? DataOptions { get; }

        /// <summary>
        /// Asynchronously executes a command/query with the specified connection, options and executable against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionBuilder"/> to build a new instance.</param>
        /// <param name="dataExecutable">The data executable instance to be used.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutable" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult>(
            IDataConnection dataConnection,
            IDataOptions dataOptions,
            DataExecutable<TResult> dataExecutable,
            string commandText,
            CommandType commandType,
            params object[] parameters)
        {
            _ = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _ = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
            _ = dataExecutable ?? throw new ArgumentNullException(nameof(dataExecutable));

            var transaction = default(DbTransaction);
            try
            {
                var dataConnectionContext = await DataConnectionContextProvider.GetDataConnectionContextAsync(dataConnection).ConfigureAwait(false);

                using var connection = dataConnectionContext.DbConnection;
                using var command = connection.CreateCommand();
                using var adapter = dataConnectionContext.DbProviderFactory.CreateDataAdapter();

                command.CommandType = commandType;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync().ConfigureAwait(false);

                if (dataOptions.IsTransactionEnabled)
                {
                    transaction = await connection.BeginTransactionAsync(dataOptions.IsolationLevel, dataOptions.CancellationToken).ConfigureAwait(false);
                    command.Transaction = transaction;
                }

                var component = new DataExecutableContext.DataComponent(command, adapter);
                var arguments = new DataExecutableContext.DataArgument(dataOptions, commandText, parameters);
                var context = new DataExecutableContext(arguments, component);

                return await dataExecutable.ExecuteAsync(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (dataOptions.IsTransactionEnabled)
                {
                    try
                    {
                        await transaction!.RollbackAsync(dataOptions.CancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception sqlException)
                    {
                        var invalidSqlException = new InvalidOperationException(
                            "Exception encountered while attempting to roll back the transaction.",
                            new AggregateException(new[] { exception, sqlException }));

                        if (dataOptions.IsOnExceptionDefined)
                            dataOptions.OnExceptionHandled(invalidSqlException);
                        else
                            throw invalidSqlException;
                    }
                }

                var invalidException = new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
                if (dataOptions.IsOnExceptionDefined)
                    dataOptions.OnExceptionHandled(invalidException);
                else
                    throw invalidException;

                return Optional<TResult>.Empty();
            }
            finally
            {
                if (dataOptions.IsTransactionEnabled)
                    await transaction!.DisposeAsync();
            }
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified connection, options and executable mapper against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataConnection">The data connection. You can use the <see cref="DataConnectionBuilder"/> to build a new instance.</param>
        /// <param name="dataOptions">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="dataExecutableMapper">the data executable mapper.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableMapper" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult>(
            IDataConnection dataConnection,
            IDataOptions dataOptions,
            DataExecutableMapper<TResult> dataExecutableMapper,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TResult : class, new()
        {
            _ = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _ = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
            var executableMapped = dataExecutableMapper ?? throw new ArgumentNullException(nameof(dataExecutableMapper));

            var transaction = default(DbTransaction);

            DbConnection connectionSource;
            DbCommand commandSource;
            DbDataAdapter adapterSource;

            try
            {
                var dataConnectionContext = await DataConnectionContextProvider.GetDataConnectionContextAsync(dataConnection).ConfigureAwait(false);

                connectionSource = dataConnectionContext.DbConnection;
                commandSource = connectionSource.CreateCommand();
                adapterSource = dataConnectionContext.DbProviderFactory.CreateDataAdapter();

                commandSource.CommandType = commandType;
                if (connectionSource.State != ConnectionState.Open)
                    await connectionSource.OpenAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
            }

            using var connection = connectionSource;
            using var command = commandSource;
            using var adapter = adapterSource;

            if (dataOptions.IsTransactionEnabled)
            {
                transaction = await connection.BeginTransactionAsync(dataOptions.IsolationLevel, dataOptions.CancellationToken).ConfigureAwait(false);
                command.Transaction = transaction;
            }

            var component = new DataExecutableContext.DataComponent(command, adapter);
            var arguments = new DataExecutableContext.DataArgument(dataOptions, commandText, parameters);
            var context = new DataExecutableContext(arguments, component);

            var enumerableAsync = executableMapped.ExecuteMappedAsync(context);
            await using var enumeratorAsync = enumerableAsync.GetAsyncEnumerator(dataOptions.CancellationToken);

            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await enumeratorAsync.MoveNextAsync();
                }
                catch (Exception exception)
                {
                    if (dataOptions.IsTransactionEnabled)
                    {
                        try
                        {
                            transaction?.Rollback();
                        }
                        catch (Exception sqlException)
                        {
                            var invalidSqlException = new InvalidOperationException(
                                            "Exception encountered while attempting to roll back the transaction.",
                                            new AggregateException(new[] { exception, sqlException }));

                            if (dataOptions.IsOnExceptionDefined)
                                dataOptions.OnExceptionHandled(invalidSqlException);
                            else
                                throw invalidSqlException;

                            yield break;
                        }
                        finally
                        {
                            if (dataOptions.IsTransactionEnabled)
                                transaction?.Dispose();
                        }
                    }

                    var invalidException = new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
                    if (dataOptions.IsOnExceptionDefined)
                        dataOptions.OnExceptionHandled(invalidException);
                    else
                        throw invalidException;
                }

                if (resultExist)
                    yield return enumeratorAsync.Current;
            }
        }
    }

    /// <summary>
    /// Provides with a default implementation of <see cref="IDataBase"/>.
    /// </summary>
    public sealed class DataBase : IDataBase
    {
        private readonly IDataConnectionContextProvider _dataConnectionContextProvider;
        private readonly IDataExecutableProvider _dataExecutableProvider;
        private readonly IDataConnection? _dataConnection;
        private readonly IDataOptions? _dataOptions;

        IDataExecutableProvider IDataBase.DataExecutableProvider => _dataExecutableProvider;
        IDataConnectionContextProvider IDataBase.DataConnectionContextProvider => _dataConnectionContextProvider;
        IDataConnection? IDataBase.DataConnection => _dataConnection;
        IDataOptions? IDataBase.DataOptions => _dataOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/>.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data connection.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataConnection">The default data connection to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataConnection dataConnection)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data options.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataOptions">The default data options to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataOptions dataOptions)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataOptions = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data options and connection.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataConnection">The default data connection to be used.</param>
        /// <param name="dataOptions">The default data options to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataConnection dataConnection, IDataOptions dataOptions)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _dataOptions = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
        }
    }
}
