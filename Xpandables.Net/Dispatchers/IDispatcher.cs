
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

using Xpandables.Net.Commands;
using Xpandables.Net.Optionals;
using Xpandables.Net.Queries;
using Xpandables.Net.Types;

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
        /// Gets the handlers service provider.
        /// </summary>
        IDispatcherHandlerProvider DispatcherHandlerProvider { get; }

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
        /// <returns>An object that contains an enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> SendAsyncQueryResultAsync<TQuery, TResult>(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TQuery : class, IAsyncQuery<TResult>
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var handler = DispatcherHandlerProvider.GetHandler<IAsyncQueryHandler<TQuery, TResult>>()
                    ?? throw new NotImplementedException($"The matching query handler for {typeof(TQuery).Name} is missing.");

            await foreach (var result in AsyncExecute(handler.HandleAsync(query, cancellationToken), cancellationToken).ConfigureAwait(false))
                yield return result;
        }

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
        /// <returns>An object that contains an enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> SendQueryAsync<TResult>(IAsyncQuery<TResult> query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(AsyncQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);

            if (!(DispatcherHandlerProvider.GetHandler(wrapperType) is IAsyncQueryHandlerWrapper<TResult> handler))
                throw new NotImplementedException(
                    $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerBuilder<,>).Name} is registered.");

            await foreach (var result in AsyncExecute(handler.HandleAsync(query, cancellationToken), cancellationToken).ConfigureAwait(false))
                yield return result;
        }

        /// <summary>
        /// Send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/>) 
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
        /// <returns>A task that represents an optional object that may contains a value of <typeparamref name="TResult"/> or not.</returns>
        public async Task<Optional<TResult>> SendQueryResultAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var handler = DispatcherHandlerProvider.GetHandler<IQueryHandler<TQuery, TResult>>()
                    ?? throw new NotImplementedException($"The matching query handler for {typeof(TQuery).Name} is missing.");

            return await AsyncExecute(handler.HandleAsync(query, cancellationToken)).ConfigureAwait(false);
        }

        /// <summary>
        /// Send the specified query to its handler (<see cref="IQueryHandler{TQuery, TResult}"/> where TQuery is <see cref="IQuery{TResult}"/>)
        /// and returns a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an optional object that may contains a value of <typeparamref name="TResult"/> or not.</returns>
        public async Task<Optional<TResult>> SendQueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(QueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);

            if (!(DispatcherHandlerProvider.GetHandler(wrapperType) is IQueryHandlerWrapper<TResult> handler))
                throw new NotImplementedException(
                    $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(QueryHandlerBuilder<,>).Name} is registered.");

            return await AsyncExecute(handler.HandleAsync(query, cancellationToken)).ConfigureAwait(false);
        }

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
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, IAsyncCommand
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            var handler = DispatcherHandlerProvider.GetHandler<IAsyncCommandHandler<TCommand>>()
                ?? throw new NotImplementedException($"The matching command handler for {typeof(TCommand).Name} is missing.");

            await AsyncExecute(handler.HandleAsync(command, cancellationToken)).ConfigureAwait(false);
        }

        /// <summary>
        /// Send the specified command to its handler (<see cref="ICommandHandler{TCommand}"/>).
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public void SendCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            var handler = DispatcherHandlerProvider.GetHandler<ICommandHandler<TCommand>>()
                    ?? throw new NotImplementedException($"The matching command handler for {typeof(TCommand).Name} is missing.");

            try
            {
                handler.Handle(command);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"{nameof(Dispatcher)} execution failed. See inner exception",
                    exception);
            }
        }

        private static async IAsyncEnumerable<TResult> AsyncExecute<TResult>(IAsyncEnumerable<TResult> asyncExecutable, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await using var asyncEnumerator = asyncExecutable.GetAsyncEnumerator(cancellationToken);
            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
                {
                    throw new InvalidOperationException(
                        $"{nameof(Dispatcher)} execution failed. See inner exception",
                        exception);
                }

                if (resultExist)
                    yield return asyncEnumerator.Current;
            }
        }

        private static async Task<Optional<TResult>> AsyncExecute<TResult>(Task<Optional<TResult>> asyncExecutable)
        {
            try
            {
                return await asyncExecutable.ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"{nameof(Dispatcher)} execution failed. See inner exception",
                    exception);
            }
        }

        private static async Task AsyncExecute(Task asyncExecutable)
        {
            try
            {
                await asyncExecutable.ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !exception.GetType().Name.Contains("ValidationException")
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"{nameof(Dispatcher)} execution failed. See inner exception",
                    exception);
            }
        }
    }
}