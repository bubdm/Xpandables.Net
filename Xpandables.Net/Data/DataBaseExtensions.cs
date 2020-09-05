
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with extension methods for <see cref="IDataBaseConnection"/>.
    /// </summary>
    public static class DataBaseExtensions
    {
        /// <summary>
        /// Executes a command/query with the specified executable <typeparamref name="TExecutable" /> type
        /// and returns a result of <typeparamref name="TResult" /> type using default execution options.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="IDataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="IDataExecutable{T}" /> interface.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query or store procedure name.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteAsync<TResult, TExecutable>(
            this IDataBaseConnection dataBase,
            string commandText,
            CommandType commandType,
            params object[] parameters)
            where TExecutable : class, IDataExecutable<TResult>
            => await dataBase.ExecuteAsync<TResult, TExecutable>(new DataOptionsBuilder().BuildDefault(), commandText, commandType, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified query as transactional query using the parameters and options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionAsync(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified query as transactional query using the parameters and default option
        /// and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionAsync(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            => await dataBase.ExecuteTransactionAsync(new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified command to the database using parameters and options and returns a data table.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified query to the database using parameters and default options
        /// and returns a data table.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            => await dataBase.ExecuteQueryTableAsync(new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified stored procedure to the database using parameters and options and returns a data table.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified stored procedure to the database using parameters and default options
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            => await dataBase.ExecuteProcedureTableAsync(new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using default options
        /// and returns an asynchronous collection of results of the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProceduresAsync<TResult>(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedProceduresAsync<TResult>(
                new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the stored procedure by its name using options and returns an asynchronous collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProceduresAsync<TResult>(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapper<TResult>>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the specify query to the database using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueriesAsync<TResult>(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapper<TResult>>(
                options, commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the specify query to the database using default options
        /// and returns a collection of results of the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueriesAsync<TResult>(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedQueriesAsync<TResult>(
                new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the stored procedure by its name using options and returns the number of affected rows.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteProcedureAsync(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableProcedure>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using default options
        /// and returns the number of affected rows.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteProcedureAsync(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            => await dataBase.ExecuteProcedureAsync(new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes a query that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleAsync<TResult>(
            this IDataBaseConnection dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes a query that returns a single value of specific type using default options.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleAsync<TResult>(
            this IDataBaseConnection dataBase, string commandText, params object[] parameters)
            => await dataBase.ExecuteSingleAsync<TResult>(
                new DataOptionsBuilder().BuildDefault(), commandText, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Determines whether or not the target data record contains the specified column name.
        /// </summary>
        /// <param name="dataRecord">The data record to act on.</param>
        /// <param name="columnName">The column name to find.</param>
        /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dataRecord"/>  is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="columnName"/>  is null.</exception>
        public static bool Contains(this IDataRecord dataRecord, string columnName)
        {
            _ = dataRecord ?? throw new ArgumentNullException(nameof(dataRecord));
            _ = columnName ?? throw new ArgumentNullException(nameof(columnName));

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                if (dataRecord.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not the connection is an SQL Connection.
        /// </summary>
        /// <param name="connection">The connection to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connection"/> is null.</exception>
        public static bool IsSqlConnection(this IDbConnection connection)
        {
            if (connection is null) throw new ArgumentNullException(nameof(connection));
            return connection.GetType().Name.Contains("SqlConnection", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses query that uses old format.
        /// </summary>
        /// <param name="query">The query to be formatted.</param>
        /// <returns>A parsed query.</returns>
        public static string ParseSql(this string query)
        {
            string[] parts = query.Split('?');
            if (parts.Length > 0)
            {
                var output = new StringBuilder();
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    output.Append(parts[i]);
                    output.Append("@P").Append(i);
                }

                output.Append(parts[^1]);
                query = output.ToString();
            }

            return query;
        }
    }
}
