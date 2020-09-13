
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
using System.Threading.Tasks;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data
{
    public static partial class DataBaseExtensions
    {
        /// <summary>
        /// Asynchronously executes the stored procedure by its name using options and returns an asynchronous collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="dataOptions">The database options.</param>       
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProcedureAsync<TResult>(
            this IDataBase dataBase,
            IDataConnection dataConnection,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataConnection, dataOptions, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the stored procedure by its name using options and returns an asynchronous collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProcedureAsync<TResult>(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataOptions, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the stored procedure by its name using connection and returns an asynchronous collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProcedureAsync<TResult>(
            this IDataBase dataBase,
            IDataConnection dataConnection,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataConnection, commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the stored procedure by its name returns an asynchronous collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedProcedureAsync<TResult>(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                commandText, CommandType.StoredProcedure, parameters)
                .ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the specify query against the database using connection, options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database connection.</param>
        /// <param name="dataOptions">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueryAsync<TResult>(
            this IDataBase dataBase,
            IDataConnection dataConnection,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataConnection, dataOptions, commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the specify query against the database using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataOptions">The database connection.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueryAsync<TResult>(
            this IDataBase dataBase,
            IDataOptions dataOptions,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataOptions, commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the specify query against the database using connection and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="dataConnection">The database options.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueryAsync<TResult>(
            this IDataBase dataBase,
            IDataConnection dataConnection,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                dataConnection, commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously executes the specify query against the database and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="dataBase">The target database.</param>
        /// <param name="commandText">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        public static async IAsyncEnumerable<TResult> ExecuteMappedQueryAsync<TResult>(
            this IDataBase dataBase,
            string commandText,
            params object[] parameters)
            where TResult : class, new()
        {
            await foreach (var result in dataBase.ExecuteMappedAsync<TResult, DataExecutableMapperFuncProc<TResult>>(
                commandText, CommandType.Text, parameters).
                ConfigureAwait(false))
                yield return result;
        }
    }
}
