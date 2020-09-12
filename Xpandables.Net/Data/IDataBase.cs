
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
    public interface IDataBase
    {
        internal IDataConnectionContextProvider DataConnectionContextProvider { get; }
        internal IDataExecutableProvider DataExecutableProvider { get; }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutable">The data executable instance to be used.</param>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult>(
            DataExecutable<TResult> dataExecutable,
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
        {
            var executable = dataExecutable ?? throw new ArgumentNullException(nameof(dataExecutable));
            var transaction = default(DbTransaction);
            try
            {
                var dataConnectionContext = await DataConnectionContextProvider.GetDataConnectionContextAsync(options.Connection).ConfigureAwait(false);

                using var connection = dataConnectionContext.DbConnection;
                using var command = connection.CreateCommand();
                using var adapter = dataConnectionContext.DbProviderFactory.CreateDataAdapter();

                command.CommandType = commandType;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync().ConfigureAwait(false);

                if (options.IsTransactionEnabled)
                {
                    transaction = await connection.BeginTransactionAsync(options.IsolationLevel, options.CancellationToken).ConfigureAwait(false);
                    command.Transaction = transaction;
                }

                var component = new DataExecutableContext.DataComponent(command, adapter);
                var arguments = new DataExecutableContext.DataArgument(options, commandText, parameters);
                var context = new DataExecutableContext(arguments, component);

                return await executable.ExecuteAsync(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (options.IsTransactionEnabled)
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

                        if (options.IsOnExceptionDefined)
                            options.OnExceptionHandled(invalidSqlException);
                        else
                            throw invalidSqlException;
                    }
                }

                var invalidException = new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
                if (options.IsOnExceptionDefined)
                    options.OnExceptionHandled(invalidException);
                else
                    throw invalidException;

                return Optional<TResult>.Empty();
            }
            finally
            {
                if (options.IsTransactionEnabled)
                    transaction?.Dispose();
            }
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TDataExecutable" /> type against the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutable" /> type inherits from <see cref="DataExecutable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable. The class inherits from <see cref="DataExecutable{T}" />.</typeparam>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TDataExecutable>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutable : DataExecutable<TResult>
        {
            var executable = DataExecutableProvider.GetDataExecutable<TResult, TDataExecutable>()
                ?? throw new ArgumentException($"{typeof(TDataExecutable).Name} not found !");

            return await ExecuteAsync(executable, options, commandText, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataExecutableMapper">the data executable mapper.</param>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult>(
            DataExecutableMapper<TResult> dataExecutableMapper,
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TResult : class, new()
        {
            var executableMapped = dataExecutableMapper ?? throw new ArgumentNullException(nameof(dataExecutableMapper));
            var transaction = default(DbTransaction);

            DbConnection connectionSource;
            DbCommand commandSource;
            DbDataAdapter adapterSource;

            try
            {
                var dataConnectionContext = await DataConnectionContextProvider.GetDataConnectionContextAsync(options.Connection).ConfigureAwait(false);

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

            if (options.IsTransactionEnabled)
            {
                transaction = await connection.BeginTransactionAsync(options.IsolationLevel, options.CancellationToken).ConfigureAwait(false);
                command.Transaction = transaction;
            }

            var component = new DataExecutableContext.DataComponent(command, adapter);
            var arguments = new DataExecutableContext.DataArgument(options, commandText, parameters);
            var context = new DataExecutableContext(arguments, component);

            var enumerableAsync = executableMapped.ExecuteMappedAsync(context);
            await using var enumeratorAsync = enumerableAsync.GetAsyncEnumerator(options.CancellationToken);

            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await enumeratorAsync.MoveNextAsync();
                }
                catch (Exception exception)
                {
                    if (options.IsTransactionEnabled)
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

                            if (options.IsOnExceptionDefined)
                                options.OnExceptionHandled(invalidSqlException);
                            else
                                throw invalidSqlException;

                            yield break;
                        }
                        finally
                        {
                            if (options.IsTransactionEnabled)
                                transaction?.Dispose();
                        }
                    }

                    var invalidException = new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
                    if (options.IsOnExceptionDefined)
                        options.OnExceptionHandled(invalidException);
                    else
                        throw invalidException;
                }

                if (resultExist)
                    yield return enumeratorAsync.Current;
            }
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TDataExecutableMapped" /> type against the database
        /// and asynchronously returns result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TDataExecutableMapped" /> type inherits from <see cref="DataExecutable{TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapped">The type of the executable. The class inherits from <see cref="DataExecutable{TResult}" /> interface.</typeparam>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TDataExecutableMapped>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TDataExecutableMapped : DataExecutableMapper<TResult>
            where TResult : class, new()
        {
            var executableMapped = DataExecutableProvider.GetDataExecutableMapper<TResult, TDataExecutableMapped>()
                       ?? throw new ArgumentException($"{typeof(TDataExecutableMapped).Name} not found !");

            await foreach (var result in ExecuteMappedAsync(executableMapped, options, commandText, commandType, parameters).ConfigureAwait(false))
                yield return result;
        }
    }

    /// <summary>
    /// Provides with a default implementation of <see cref="IDataBase"/>.
    /// </summary>
    public sealed class DataBase : IDataBase
    {
        private readonly IDataConnectionContextProvider _dataConnectionContextProvider;
        private readonly IDataExecutableProvider _dataExecutableProvider;

        IDataExecutableProvider IDataBase.DataExecutableProvider => _dataExecutableProvider;
        IDataConnectionContextProvider IDataBase.DataConnectionContextProvider => _dataConnectionContextProvider;

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
    }
}
