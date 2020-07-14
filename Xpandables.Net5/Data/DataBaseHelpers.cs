
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

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Provides with helper methods for data base.
    /// </summary>
    public static class DataBaseHelpers
    {
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

        /// <summary>
        /// Executes the specified query as transactional query using the parameters and default option
        /// and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<int> ExecuteTransactionAsync(this DataBase dataBase, string sqlQuery, params object[] parameters)
            => await dataBase.ExecuteTransactionAsync(new DataOptionsBuilder().BuildDefault(), sqlQuery, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specified query to the database using parameters and default options
        /// and returns a data table.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<DataTable> ExecuteQueryTableAsync(this DataBase dataBase, string sqlQuery, params object[] parameters)
            => await dataBase.ExecuteQueryTableAsync(new DataOptionsBuilder().BuildDefault(), sqlQuery, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specified stored procedure to the database using parameters and default options
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="storedProc">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProc"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<DataTable> ExecuteProcedureTableAsync(this DataBase dataBase, string storedProc, params object[] parameters)
            => await dataBase.ExecuteProcedureTableAsync(new DataOptionsBuilder().BuildDefault(), storedProc, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specify query to the database using default options
        /// and returns a result of the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dataBase"></param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<T?> ExecuteQueryAsync<T>(this DataBase dataBase, string sqlQuery, params object[] parameters)
            where T : class, new()
            => await dataBase.ExecuteQueryAsync<T>(new DataOptionsBuilder().BuildDefault(), sqlQuery, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specify query to the database using default options
        /// and returns a collection of results of the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dataBase"></param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<List<T>> ExecuteQueriesAsync<T>(this DataBase dataBase, string sqlQuery, params object[] parameters)
            where T : class, new()
            => await dataBase.ExecuteQueriesAsync<T>(new DataOptionsBuilder().BuildDefault(), sqlQuery, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using default options
        /// and returns the number of affected rows.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="storedProc">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProc"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<int> ExecuteProcedureAsync(this DataBase dataBase, string storedProc, params object[] parameters)
            => await dataBase.ExecuteProcedureAsync(new DataOptionsBuilder().BuildDefault(), storedProc, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using default options
        /// and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dataBase"></param>
        /// <param name="storedProc">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProc"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<List<T>> ExecuteProceduresAsync<T>(this DataBase dataBase, string storedProc, params object[] parameters)
            where T : class, new()
            => await dataBase.ExecuteProceduresAsync<T>(new DataOptionsBuilder().BuildDefault(), storedProc, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes a query that returns a single value of specific type using default options.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dataBase"></param>
        /// <param name="sqlQuery">The query definition.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<T> ExecuteSingleAsync<T>(this DataBase dataBase, string sqlQuery, params object[] parameters)
            => await dataBase.ExecuteSingleAsync<T>(new DataOptionsBuilder().BuildDefault(), sqlQuery, parameters).ConfigureAwait(false);
    }
}
