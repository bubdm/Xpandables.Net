﻿
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
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="IAsyncCommand"/>, <see cref="IAsyncQuery{TResult}"/> and <see cref="IAsyncEnumerableQuery{TResult}"/>
    /// when targeting <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}"/>, <see cref="IAsyncQueryHandler{TQuery, TResult}"/> or/and <see cref="IAsyncCommandHandler{TCommand}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDispatcher
    {
        internal IDispatcherHandlerProvider DispatcherHandlerProvider { get; }

        /// <summary>
        /// Asynchronously invokes the query handler (<see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}"/> implementation) on the specified query 
        /// and returns an asynchronous enumerable of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="query"/>.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public virtual IAsyncEnumerable<TResult> InvokeAsync<TResult>(IAsyncEnumerableQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(AsyncEnumerableQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new[] { query.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IAsyncEnumerableQueryHandlerWrapper<TResult> handler)
                throw new NotImplementedException(
                    $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncEnumerableQueryHandlerWrapper<,>).Name} is registered.");

            if (!handler.CanHandle(query))
                throw new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {query.GetType().Name} type.");

            return DoAsyncEnumerableInvokeAsync(handler.HandleAsync(query, cancellationToken), cancellationToken);
        }

        private static async IAsyncEnumerable<TResult> DoAsyncEnumerableInvokeAsync<TResult>(IAsyncEnumerable<TResult> asyncEnumerable, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = asyncEnumerable ?? throw new ArgumentNullException(nameof(asyncEnumerable));

            await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);

            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    resultExist = false;
                    switch (exception)
                    {
                        case ArgumentException:
                        case ValidationException:
                        case OperationCanceledException:
                        case InvalidOperationException: throw;
                        default: throw new InvalidOperationException($"{nameof(IDispatcher)} execution failed. See inner exception", exception);
                    };
                }

                if (resultExist)
                    yield return asyncEnumerator.Current;
                else
                    yield break;
            }
        }

        /// <summary>
        /// Asynchronously invokes the command handler (<see cref="IAsyncCommandHandler{TCommand}"/> implementation) on the specified command.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="command"/>.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task<IResultState> InvokeAsync(IAsyncCommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            if (!typeof(IAsyncCommandHandler<>).TryMakeGenericType(out var handlerType, out var typeException, new[] { command.GetType() }))
                throw new InvalidOperationException("Building command handler failed.", typeException);

            dynamic handler = DispatcherHandlerProvider.GetHandler(handlerType)
                ?? throw new NotImplementedException($"The matching command handler for {command.GetType().Name} is missing.");

            if (!((ICanHandle)handler).CanHandle(command))
                throw new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");

            try
            {
                return await handler.HandleAsync((dynamic)command, (dynamic)cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException:
                    case ValidationException:
                    case OperationCanceledException:
                    case InvalidOperationException: throw;
                    default: throw new InvalidOperationException($"{nameof(IDispatcher)} execution failed. See inner exception", exception);
                };
            }
        }

        /// <summary>
        /// Asynchronously invokes the command handler(<see cref="IAsyncCommandHandler{TCommand, TResult}"/> implementation) on the specified command
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="command"/>.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an object <typeparamref name="TResult"/> or not.</returns>
        public virtual async Task<IResultState<TResult>> InvokeAsync<TResult>(IAsyncCommand<TResult> command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            if (!typeof(AsyncCommandHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { command.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Command wrapper failed.", typeException);

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IAsyncCommandHandlerWrapper<TResult> handler)
                throw new NotImplementedException(
                    $"The matching command handler for {command.GetType().Name} is missing. Be sure the {typeof(AsyncCommandHandlerWrapper<,>).Name} is registered.");

            if (!handler.CanHandle(command))
                throw new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");

            try
            {
                return await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException:
                    case ValidationException:
                    case OperationCanceledException:
                    case InvalidOperationException: throw;
                    default: throw new InvalidOperationException($"{nameof(IDispatcher)} execution failed. See inner exception", exception);
                };
            }
        }

        /// <summary>
        /// Asynchronously invokes the query handler(<see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation) on the specified query
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="query"/>.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an object <typeparamref name="TResult"/> or not.</returns>
        public virtual async Task<IResultState<TResult>> InvokeAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(AsyncQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IAsyncQueryHandlerWrapper<TResult> handler)
                throw new NotImplementedException(
                    $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerWrapper<,>).Name} is registered.");

            if (!handler.CanHandle(query))
                throw new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {query.GetType().Name} type.");

            try
            {
                return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException:
                    case ValidationException:
                    case OperationCanceledException:
                    case InvalidOperationException: throw;
                    default: throw new InvalidOperationException($"{nameof(IDispatcher)} execution failed. See inner exception", exception);
                };
            }
        }
    }
}