
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
    public static partial class DataBaseExtensions
    {
        /// <summary>
        /// Asynchronously executes the stored procedure by its name using options and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                dataOptions, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the stored procedure by its name using connection and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                dataConnection, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the stored procedure by its name and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteProcedureTableAsync(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the stored procedure as transactional using the options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionProcedureAsync(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                dataOptions, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the stored procedure as transactional using the connection and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionProcedureAsync(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                dataConnection, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the stored procedure as transactional and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionProcedureAsync(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the specified query as transactional using the options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionQueryAsync(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                dataOptions, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the specified query as transactional using the connection and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<int>> ExecuteTransactionQueryAsync(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<int, DataExecutableTransaction>(
                dataConnection, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the query against the database using options and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                dataOptions, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the query against the database using connection and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                dataConnection, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes the query against the database and returns an optional data table result.
        /// </summary>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<DataTable>> ExecuteQueryTableAsync(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<DataTable, DataExecutableTable>(
                 commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a stored procedure against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleProcedureAsync<TResult>(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                dataOptions, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a stored procedure against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleProcedureAsync<TResult>(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                dataConnection, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a stored procedure against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleProcedureAsync<TResult>(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a query against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleQueryAsync<TResult>(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                dataOptions, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a query against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleQueryAsync<TResult>(
            this IDataBase dataBase,
            IDataConnectionOptions dataConnection,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                dataConnection, commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);

        /// <summary>
        /// Asynchronously executes a query against the database that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query/stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async Task<Optional<TResult>> ExecuteSingleQueryAsync<TResult>(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            => await dataBase.ExecuteAsync<TResult, DataExecutableSingle<TResult>>(
                commandText, CommandType.Text, parameters)
                .ConfigureAwait(false);
    }
}
