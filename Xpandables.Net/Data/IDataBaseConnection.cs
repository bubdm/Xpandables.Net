
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
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Data.Providers;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with methods to execute command against a database using the defined <see cref="IDataConnection"/>.
    /// </summary>
    public interface IDataBaseConnection : IDataBase
    {
        internal IDataConnection DataConnection { get; }
        internal Lazy<DbProviderFactory> DbProviderFactory { get; }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TExecutable" /> type to the database
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="IDataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="IDataExecutable{T}" /> interface.</typeparam>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<Optional<TResult>> ExecuteAsync<TResult, TExecutable>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TExecutable : class, IDataExecutable<TResult>
        {
            var executable = DataExecutableProvider.GetExecutable<TExecutable>()
                ?? throw new ArgumentException($"{typeof(TExecutable).Name} not found !");

            var transaction = default(DbTransaction);
            try
            {
                using var connection = await BuildDbConnectionAsync(DbProviderFactory.Value, DataConnection).ConfigureAwait(false);
                using var command = connection.CreateCommand();
                using var adapter = DbProviderFactory.Value.CreateDataAdapter();

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
                        throw new InvalidOperationException(
                            "Exception encountered while attempting to roll back the transaction.",
                            new AggregateException(new[] { exception, sqlException }));
                    }
                }

                throw new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
            }
            finally
            {
                if (options.IsTransactionEnabled)
                    transaction?.Dispose();
            }
        }

        /// <summary>
        /// Asynchronously executes a command/query with the specified executable <typeparamref name="TExecutableMapped" /> type to the database
        /// and returns an asynchronous result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutableMapped" /> type must implement <see cref="IDataExecutable{TResult}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutableMapped">The type of the executable. The class must implement <see cref="IDataExecutable{TResult}" /> interface.</typeparam>
        /// <param name="options">The database options. You can use the <see cref="DataOptionsBuilder"/> to build a new instance.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">A collection of parameter objects for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync<TResult, TExecutableMapped>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TExecutableMapped : class, IDataExecutableMapper<TResult>
        {
            var executableMapped = DataExecutableProvider.GetExecutableMapped<TExecutableMapped>()
                ?? throw new ArgumentException($"{typeof(TExecutableMapped).Name} not found !");

            var transaction = default(DbTransaction);
            var context = default(DataExecutableContext);
            try
            {
                using var connection = await BuildDbConnectionAsync(DbProviderFactory.Value, DataConnection).ConfigureAwait(false);
                using var command = connection.CreateCommand();
                using var adapter = DbProviderFactory.Value.CreateDataAdapter();

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
                context = new DataExecutableContext(arguments, component);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Exception encountered while attempting to execute command.", exception);
            }

            await foreach (var result in AsyncExtensions.TryCatchAsyncEnumerable(() => executableMapped.ExecuteMappedAsync(context, options.CancellationToken).ConfigureAwait(false), out var exceptionDispatchInfo))
            {
                if (exceptionDispatchInfo is { })
                {
                    if (options.IsTransactionEnabled)
                    {
                        try
                        {
                            transaction?.Rollback();
                        }
                        catch (Exception sqlException)
                        {
                            throw new InvalidOperationException(
                                "Exception encountered while attempting to roll back the transaction.",
                                new AggregateException(new[] { exceptionDispatchInfo.SourceException, sqlException }));
                        }
                        finally
                        {
                            if (options.IsTransactionEnabled)
                                transaction?.Dispose();
                        }
                    }

                    throw new InvalidOperationException("Exception encountered while attempting to execute command.", exceptionDispatchInfo.SourceException);
                }

                yield return result;
            }
        }

        /// <summary>
        /// Provides with a database connection using the provider and the connection string.
        /// </summary>
        /// <param name="dbProviderFactory">The database provider factory.</param>
        /// <param name="dataConnection">The data connection to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbProviderFactory"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        private static async Task<DbConnection> BuildDbConnectionAsync(DbProviderFactory dbProviderFactory, IDataConnection dataConnection)
        {
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = dataConnection.GetConnectionString();
            await dbConnection.OpenAsync().ConfigureAwait(false);
            await SpeedSqlServerResultAsync(dbConnection).ConfigureAwait(false);
            return dbConnection;
        }

        /// <summary>
        /// Speeds the connection result for SQL server only.
        /// </summary>
        /// <param name="connection">the connection to speed.</param>
        private static async Task SpeedSqlServerResultAsync(DbConnection connection)
        {
            if (connection.GetType().Name.Equals("SqlConnection", StringComparison.OrdinalIgnoreCase))
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"
                    SET ANSI_NULLS ON
                    SET ANSI_PADDING ON
                    SET ANSI_WARNINGS ON
                    SET ARITHABORT ON
                    SET CONCAT_NULL_YIELDS_NULL ON
                    SET QUOTED_IDENTIFIER ON
                    SET NUMERIC_ROUNDABORT OFF";
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Default implementation of <see cref="IDataBaseConnection"/>.
    /// </summary>
    public sealed class DataBaseConnection : IDataBaseConnection
    {
        private readonly IDataConnection _dataConnection;
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;
        private readonly IDataExecutableProvider _dataExecutableProvider;
        private readonly IDataFactoryProvider _dataFactoryProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseConnection"/> with specified properties.
        /// </summary>
        /// <param name="dataConnection">The data connection to be sued.</param>
        /// <param name="dataFactoryProvider">The data factory provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataFactoryProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        internal DataBaseConnection(IDataConnection dataConnection, IDataFactoryProvider dataFactoryProvider, IDataExecutableProvider dataExecutableProvider)
        {
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _dataFactoryProvider = dataFactoryProvider ?? throw new ArgumentNullException(nameof(dataFactoryProvider));
            _dbProviderFactory = new Lazy<DbProviderFactory>(
                () => dataFactoryProvider.GetProviderFactory(dataConnection.ProviderType)
                ?? throw new InvalidOperationException($"Unable to find the specified data base provider.{_dataConnection.ProviderType.DisplayName}"));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
        }

        IDataConnection IDataBaseConnection.DataConnection => _dataConnection;
        IDataExecutableProvider IDataBase.DataExecutableProvider => _dataExecutableProvider;
        IDataFactoryProvider IDataBase.DataFactoryProvider => _dataFactoryProvider;
        Lazy<DbProviderFactory> IDataBaseConnection.DbProviderFactory => _dbProviderFactory;
    }
}
