
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;
using Xpandables.Net.Types;

using static Xpandables.Net.Dispatchers.DispatcherExtensions;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="IAsyncCommand"/> and <see cref="IAsyncQuery{TResult}"/>
    /// when targeting <see cref="IAsyncQueryHandler{TQuery, TResult}"/> or/and <see cref="IAsyncCommandHandler{TCommand}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Gets the handlers service provider.
        /// </summary>
        IDispatcherHandlerProvider DispatcherHandlerProvider { get; }

        /// <summary>
        /// Asynchronously invokes the query handler (<see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation) on the specified query and returns an asynchronous enumerable of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> InvokeAsync<TResult>(IAsyncQuery<TResult> query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(AsyncQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IAsyncQueryHandlerWrapper<TResult> handler)
                throw new NotImplementedException(
                    $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerBuilder<,>).Name} is registered.");

            await foreach (var result in handler.HandleAsync(query, cancellationToken).AsyncExecuteSafe(DispatcherExceptionHandler, cancellationToken).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously invokes the query handler (<see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation) on the specified query and returns an asynchronous enumerable of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> InvokeQueryAsync<TQuery, TResult>(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TQuery : class, IAsyncQuery<TResult>
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var handler = DispatcherHandlerProvider.GetHandler<IAsyncQueryHandler<TQuery, TResult>>()
                ?? throw new NotImplementedException($"The matching query handler for {typeof(TQuery).Name} is missing.");

            await foreach (var result in handler.HandleAsync(query, cancellationToken).AsyncExecuteSafe(DispatcherExceptionHandler, cancellationToken).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Asynchronously invokes the command handler (<see cref="IAsyncCommandHandler{TCommand}"/> implementation) on the specified command.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, IAsyncCommand
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            var handler = DispatcherHandlerProvider.GetHandler<IAsyncCommandHandler<TCommand>>()
                ?? throw new NotImplementedException($"The matching command handler for {typeof(TCommand).Name} is missing.");

            await handler.HandleAsync(command, cancellationToken).AsyncExecuteSafe<TCommand>(DispatcherExceptionHandler).ConfigureAwait(false);
        }
    }
}