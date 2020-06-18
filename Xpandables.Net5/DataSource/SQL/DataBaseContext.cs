
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
using System.Collections.Generic;
using System.Data;
using System.Design.SQL.Executables;
using System.Linq;
using System.Threading.Tasks;

namespace System.Design.SQL
{
    /// <summary>
    /// Provides methods to executes command to data base.
    /// </summary>
    public sealed class DataBaseContext : DataBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseContext" /> with the specified settings.
        /// </summary>
        /// <param name="dataProviderFactoryProvider">The settings to be used.</param>
        /// <param name="dataConnection">The data base settings.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataProviderFactoryProvider" /> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dataConnection" /> is not valid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dataConnection" /> provider is not valid.</exception>
        public DataBaseContext(IDataProviderFactoryProvider dataProviderFactoryProvider, DataConnection dataConnection)
            : base(dataProviderFactoryProvider, dataConnection) { }

        /// <summary>
        /// Executes the specified query as transactional query using the parameters and options and returns the number of records affected.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<int> ExecuteTransactionAsync(DataOptions options, string sqlQuery, params object[] parameters)
            => await ExecuteAsync<int, DataExecutableTransaction>(options, sqlQuery, CommandType.Text, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specified command to the database using parameters and options and returns a data table.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The command to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<DataTable> ExecuteQueryTableAsync(DataOptions options, string sqlQuery, params object[] parameters)
            => await ExecuteAsync<DataTable, DataExecutableTable>(options, sqlQuery, CommandType.Text, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specified stored procedure to the database using parameters and options and returns a data table.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="storedProc">The stored procedure to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProc"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<DataTable> ExecuteProcedureTableAsync(DataOptions options, string storedProc, params object[] parameters)
            => await ExecuteAsync<DataTable, DataExecutableTable>(options, storedProc, CommandType.StoredProcedure, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the specify query to the database using options and returns a result of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<T?> ExecuteQueryAsync<T>(DataOptions options, string sqlQuery, params object[] parameters)
            where T : class, new()
            => (await ExecuteAsync<List<T>, DataExecutableMapper<T>>(options, sqlQuery, CommandType.Text, parameters).ConfigureAwait(false))
                ?.FirstOrDefault();

        /// <summary>
        /// Executes the specify query to the database using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<List<T>> ExecuteQueriesAsync<T>(DataOptions options, string sqlQuery, params object[] parameters)
            where T : class, new()
            => await ExecuteAsync<List<T>, DataExecutableMapper<T>>(options, sqlQuery, CommandType.Text, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using options and returns the number of affected rows.
        /// Use <see langword="DataOptionsBuilder().UseRetrievedIdentity()"/> to retrieve the newly created identity.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<int> ExecuteProcedureAsync(DataOptions options, string storedProcedureName, params object[] parameters)
            => await ExecuteAsync<int, DataExecutableProcedure>(options, storedProcedureName, CommandType.StoredProcedure, parameters).ConfigureAwait(false);

        /// <summary>
        /// Executes the stored procedure by its name using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<List<T>> ExecuteProceduresAsync<T>(DataOptions options, string storedProcedureName, params object[] parameters)
            where T : class, new()
            => await ExecuteAsync<List<T>, DataExecutableMapper<T>>(options, storedProcedureName, CommandType.StoredProcedure, parameters)
            .ConfigureAwait(false);

        /// <summary>
        /// Executes a query that returns a single value of specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query definition.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public async Task<T> ExecuteSingleAsync<T>(DataOptions options, string sqlQuery, params object[] parameters)
            => await ExecuteAsync<T, DataExecutableSingle<T>>(options, sqlQuery, CommandType.Text, parameters).ConfigureAwait(false);
    }
}
