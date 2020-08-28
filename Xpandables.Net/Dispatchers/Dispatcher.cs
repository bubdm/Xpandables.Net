
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Extensions;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// The default implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IQueryHandler{TQuery, TResult}"/> and
    /// <see cref="ICommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class Dispatcher : IDispatcher
    {
        private readonly IDispatcherHandlerProvider _dispatcherHandlerProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="dispatcherHandlerProvider">The handler service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherHandlerProvider"/> is null.</exception>
        public Dispatcher(IDispatcherHandlerProvider dispatcherHandlerProvider)
            => _dispatcherHandlerProvider = dispatcherHandlerProvider ?? throw new ArgumentNullException(nameof(dispatcherHandlerProvider));

        /// <summary>
        /// Asynchronously send the specified command to its handler (<see cref="ICommandHandler{TCommand}"/>).
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            try
            {
                if (command is null) throw new ArgumentNullException(nameof(command));

                var handler = _dispatcherHandlerProvider.GetHandler<ICommandHandler<TCommand>>()
                        ?? throw new ArgumentException($"The matching command handler for {typeof(TCommand).Name} is missing.");

                await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"{nameof(SendCommandAsync)} execution failed. See inner exception",
                    exception);
            }
        }

        /// <summary>
        /// Asynchronously send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/>) 
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task<TResult> SendQueryResultAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                var handler = _dispatcherHandlerProvider.GetHandler<IQueryHandler<TQuery, TResult>>()
                        ?? throw new ArgumentException($"The matching query handler for {typeof(TQuery).Name} is missing.");

                return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"{nameof(SendQueryResultAsync)} execution failed. See inner exception",
                    exception);
            }
        }

        /// <summary>
        /// Asynchronously send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/> where TQuery is <see cref="IQuery{TResult}"/>)
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task<TResult> SendQueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));
                if (!typeof(QueryHandlerWrapper<,>)
                    .TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
                {
                    throw new InvalidOperationException("Building Query wrapper failed.", typeException);
                }

                if (!(_dispatcherHandlerProvider.GetHandler(wrapperType) is IQueryHandlerWrapper<TResult> handler))
                    throw new ArgumentException($"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(QueryHandlerBuilder<,>).Name} is registered using the AddXQueryHandlerWrapper method.");

                return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                     $"{nameof(SendQueryAsync)} execution failed. See inner exception",
                    exception);
            }
        }
    }
}