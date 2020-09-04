
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="ICommand"/>, <see cref="IAsyncCommand"/>, <see cref="IAsyncQuery{TResult}"/> and <see cref="IQuery{TResult}"/>
    /// when targeting <see cref="ICommandHandler{TCommand}"/>, <see cref="IQueryHandler{TQuery, TResult}"/>, <see cref="IAsyncQueryHandler{TQuery, TResult}"/> or/and <see cref="IAsyncCommandHandler{TCommand}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Asynchronously send the specified query to its handler (<see cref="IAsyncQueryHandler{TQuery, TResult}"/>) 
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        IAsyncEnumerable<TResult> SendQueryResultAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : class, IAsyncQuery<TResult>;

        /// <summary>
        /// Send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/>) 
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        TResult SendQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>;

        /// <summary>
        /// Asynchronously send the specified query to its handler (<see cref="IAsyncQueryHandler{TQuery, TResult}"/> where TQuery is <see cref="IQuery{TResult}"/>)
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        IAsyncEnumerable<TResult> SendQueryAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/> where TQuery is <see cref="IQuery{TResult}"/>)
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        TResult SendQuery<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Asynchronously send the specified command to its handler (<see cref="IAsyncCommandHandler{TCommand}"/>).
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, IAsyncCommand;

        /// <summary>
        /// Send the specified command to its handler (<see cref="ICommandHandler{TCommand}"/>).
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void SendCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand;
    }
}