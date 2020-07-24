
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
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Data.Executables;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with a default implementation of <see cref="IDataBase"/>.
    /// </summary>
    public sealed class DataBase : IDataBase
    {
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;
        private readonly IDataFactoryProvider _datarFactoryProvider;
        private readonly IDataConnectionProvider _connectionProvider;

        private readonly DataConnection _dataConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFactoryProvider"></param>
        /// <param name="connectionProvider"></param>
        public DataBase(IDataFactoryProvider dataFactoryProvider, IDataConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _datarFactoryProvider = dataFactoryProvider ?? throw new ArgumentNullException(nameof(dataFactoryProvider));


            _dataConnection = _connectionProvider.GetDataConnection() ?? throw new ArgumentNullException(nameof(connectionProvider));
            if (!_dataConnection.IsValid(out var exception)) throw new ArgumentException("Database connection not valid.", exception);

            _dbProviderFactory = new Lazy<DbProviderFactory>(
                () => dataFactoryProvider.GetProviderFactory(_dataConnection.GetProviderType())
                ?? throw new InvalidOperationException($"Unable to find the specified data base provider.{_dataConnection.ProviderName}"));
        }

        /// <summary>
        /// Executes a command/query with the specified executable <typeparamref name="TExecutable" /> type
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="IDataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="IDataExecutable{T}" /> interface.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query or store procedure name.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<TResult> ExecuteAsync<TResult, TExecutable>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            CancellationToken cancellationToken,
            params object[] parameters) where TExecutable : IDataExecutable<TResult>
        {
            var executable = _datarFactoryProvider.GetService<TExecutable>()
                ?? throw new InvalidOperationException(
                    "Exception encountered while attempting to execute command.",
                    new ArgumentException($"{typeof(TExecutable).Name} implementation not registered. Use services.AddTransient<TExecutable>()."));

            return await ExecuteAsync(executable, options, commandText, commandType, cancellationToken, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Provides with resources for executable and runs that instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of executable.</typeparam>
        /// <param name="executable">The executable instance.</param>
        /// <param name="options">The execution options.</param>
        /// <param name="commandText">The query or command to apply.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="parameters">The parameters to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="executable"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        private async Task<TResult> ExecuteAsync<TResult>(
            IDataExecutable<TResult> executable,
            DataOptions options,
            string commandText,
            CommandType commandType,
            CancellationToken cancellationToken,
            params object[] parameters)
        {
            var transaction = default(DbTransaction);
            try
            {
                using var connection = BuildConnection(_dbProviderFactory.Value, _dataConnection.GetConnectionString());
                using var command = connection.CreateCommand();
                using var adapter = _dbProviderFactory.Value.CreateDataAdapter();

                command.CommandType = commandType;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync().ConfigureAwait(false);

                if (options.IsTransactionEnabled)
                {
                    transaction = connection.BeginTransaction(options.IsolationLevel);
                    command.Transaction = transaction;
                }

                var component = new DataExecutableContext.DataComponent(command, adapter);
                var arguments = new DataExecutableContext.DataArgument(options, commandText, parameters);
                var context = new DataExecutableContext(arguments, component);

                return await executable.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
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
        /// Provides with a database connection using the provider and the connection string.
        /// </summary>
        /// <param name="dbProviderFactory">The database provider factory.</param>
        /// <param name="connectionString">The connection string to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbProviderFactory"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionString"/> is null.</exception>
        private static DbConnection BuildConnection(DbProviderFactory dbProviderFactory, string connectionString)
        {
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            SpeedSqlServerResult(dbConnection);
            return dbConnection;
        }

        /// <summary>
        /// Speeds the connection result for SQL server only.
        /// </summary>
        /// <param name="connection">the connection to speed.</param>
        private static void SpeedSqlServerResult(DbConnection connection)
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
                cmd.ExecuteNonQuery();
            }
        }
    }
}
