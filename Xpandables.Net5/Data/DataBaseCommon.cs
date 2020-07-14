
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
using System.Data.Common;
using System.Threading.Tasks;

using Xpandables.Net5.Data.Executables;

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Provides with a default implementation of <see cref="DataBaseCommon"/>.
    /// You must derive from this class in order to extend its behaviors.
    /// </summary>
    public class DataBaseCommon
    {
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;
        private readonly IDataProviderFactoryProvider _dataProviderFactoryProvider;
        private readonly DataConnection _dataConnection;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseCommon"/> with the specified settings.
        /// </summary>
        /// <param name="dataProviderFactoryProvider">The settings to be used.</param>
        /// <param name="dataConnection">The data base settings.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataProviderFactoryProvider"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dataConnection"/> is not valid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dataConnection"/> provider is not valid.</exception>
        public DataBaseCommon(IDataProviderFactoryProvider dataProviderFactoryProvider, DataConnection dataConnection)
        {
            _dataProviderFactoryProvider = dataProviderFactoryProvider ?? throw new ArgumentNullException(nameof(dataProviderFactoryProvider));
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));

            if (!_dataConnection.IsValid(out var exception)) throw new ArgumentException("Database connection not valid.", exception);

            _dbProviderFactory = new Lazy<DbProviderFactory>(
                () => dataProviderFactoryProvider?.GetProviderFactory(_dataConnection.GetProviderType())
                ?? throw new InvalidOperationException($"Unable to find the specified data base provider.{_dataConnection.ProviderName}"));
        }

        /// <summary>
        /// Executes a command/query with the specified executable <typeparamref name="TExecutable" /> type
        /// and returns a result of <typeparamref name="TResult" /> type using default execution options.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="DataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="DataExecutable{T}" /> interface.</typeparam>
        /// <param name="commandQuery">The query or store procedure name.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandQuery" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<TResult> ExecuteAsync<TResult, TExecutable>(string commandQuery, CommandType commandType, params object[] parameters)
            where TExecutable : DataExecutable<TResult>
            => await ExecuteAsync<TResult, TExecutable>(new DataOptionsBuilder().BuildDefault(), commandQuery, commandType, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes a command/query with the specified executable <typeparamref name="TExecutable" /> type
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="DataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="DataExecutable{T}" /> interface.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="commandQuery">The query or store procedure name.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandQuery" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public virtual async Task<TResult> ExecuteAsync<TResult, TExecutable>(
            DataOptions options, string commandQuery, CommandType commandType, params object[] parameters)
            where TExecutable : DataExecutable<TResult>
        {
            var executable = _dataProviderFactoryProvider.GetService(typeof(TExecutable)) as TExecutable
                ?? throw new ArgumentNullException($"{typeof(TExecutable).Name} implementation not registered. Use services.AddTransient<TExecutable>().");

            return await ExecuteAsync(executable, options, commandQuery, commandType, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Provides with resources for executable and runs that instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of executable.</typeparam>
        /// <param name="executable">The executable instance.</param>
        /// <param name="options">The execution options.</param>
        /// <param name="commandQuery">The query or command to apply.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">The parameters to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="executable"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        protected virtual async Task<TResult> ExecuteAsync<TResult>(
            DataExecutable<TResult> executable, DataOptions options, string commandQuery, CommandType commandType, params object[] parameters)
        {
            var transaction = default(DbTransaction);
            try
            {
                using var connection = BuildConnection(_dbProviderFactory.Value, _dataConnection.GetConnectionString());
                using var command = connection.CreateCommand();
                using var adapter = _dbProviderFactory.Value.CreateDataAdapter();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync().ConfigureAwait(false);

                if (options.IsTransactionEnabled)
                {
                    transaction = connection.BeginTransaction(options.IsolationLevel);
                    command.Transaction = transaction;
                }

                var accessors = new DataComponent(command, adapter, commandType);
                var arguments = new DataArgument(options, commandQuery, parameters);

                return await executable.ExecuteAsync(accessors, arguments).ConfigureAwait(false);
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
        protected virtual DbConnection BuildConnection(DbProviderFactory dbProviderFactory, string connectionString)
        {
            if (dbProviderFactory is null) throw new ArgumentNullException(nameof(dbProviderFactory));
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));

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
        protected virtual void SpeedSqlServerResult(DbConnection connection)
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
