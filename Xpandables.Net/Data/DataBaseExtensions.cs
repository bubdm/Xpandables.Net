
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
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with extension methods for <see cref="DataBase"/>.
    /// </summary>
    public static class DataBaseExtensions
    {
        /// <summary>
        /// Determines whether or not the underlying data reader contains the specified column name.
        /// If so, returns <see langword="true"/> otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="source">The source of data reader to act on.</param>
        /// <param name="columName">The column to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="columName"/> is null.</exception>
        public static bool Contains(this IDataRecord source, string columName)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(columName)) throw new ArgumentNullException(nameof(columName));

            for (int index = 0; index < source.FieldCount; index++)
            {
                if (source.GetName(index).Equals(columName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether or not the connection is an MSSQL Connection.
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
        public static async IAsyncEnumerable<TResult> ExecuteMappedProcedureAsync<TResult>(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the stored procedure by its name using options and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure as transactional using the options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionProcedureAsync(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specified query as transactional using the options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionQueryAsync(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes the specify query against the database using options and returns a collection of results of
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
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueryAsync<TResult>(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                options, commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Executes the query against the database using options and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes a stored procedure against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleProcedureAsync<TResult>(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                options, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Executes a query against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleQueryAsync<TResult>(
            this IDataBase dataBase,
            DataOptions options,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                options, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);
    }
}
