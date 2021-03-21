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
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Handlers;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// The implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IAsyncQueryHandler{TQuery, TResult}"/>, <see cref="IQueryHandler{TQuery, TResult}"/> and
    /// <see cref="ICommandHandler{TCommand}"/> process dynamically.
    /// </summary>
    public class Dispatcher : IDispatcher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher"/> class with the handlers provider.
        /// </summary>
        /// <param name="handlerAccessor">The handlers provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public Dispatcher(IHandlerAccessor handlerAccessor)
            => _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));

        /// <summary>
        /// Asynchronously fetches the query handler (<see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation) on the specified query
        /// and returns an asynchronous enumerable of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual IAsyncEnumerable<TResult> FetchAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(AsyncQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, query.GetType(), typeof(TResult)))
            {
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);
            }

            if (!_handlerAccessor.TryGetHandler(wrapperType, out var foundHandler, out var exception))
            {
                throw new InvalidOperationException(
                      $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerWrapper<,>).Name} and handler are registered.", exception);
            }

            var handler = (IAsyncQueryHandlerWrapper<TResult>)foundHandler;

            if (!handler.CanHandle(query))
            {
                WriteLineException(new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {query.GetType().Name} type."));
                return AsyncEnumerableExtensions.Empty<TResult>();
            }

            return handler.HandleAsync(query, cancellationToken);
        }

        /// <summary>
        /// Asynchronously fetches the query handler(<see cref="IQueryHandler{TQuery, TResult}"/> implementation) on the specified query.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual async Task<IOperationResult<TResult>> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            if (!typeof(QueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, query.GetType(), typeof(TResult)))
            {
                throw new InvalidOperationException("Building Query wrapper failed.", typeException);
            }

            if (!_handlerAccessor.TryGetHandler(wrapperType, out var foundHandler, out var ex))
            {
                throw new InvalidOperationException(
                    $"The matching command handler for {query.GetType().Name} is missing. Be sure the {typeof(QueryHandlerWrapper<,>).Name} and handler are registered.", ex);
            }

            var handler = (IQueryHandlerWrapper<TResult>)foundHandler;

            if (!handler.CanHandle(query))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {query.GetType().Name} type.");
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
            }

            return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously sends the command handler (<see cref="ICommandHandler{TCommand}"/> implementation) on the specified command.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual async Task<IOperationResult> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            if (!typeof(ICommandHandler<>).TryMakeGenericType(out var handlerType, out var typeException, command.GetType()))
            {
                throw new InvalidOperationException("Building command handler failed.", typeException);
            }

            if (!_handlerAccessor.TryGetHandler(handlerType, out dynamic? handler, out var ex))
            {
                throw new InvalidOperationException($"The matching command handler for {command.GetType().Name} is missing.", ex);
            }

            if (!((ICanHandle)handler).CanHandle(command))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");
                WriteLineException(exception);
                return new FailureOperationResult(HttpStatusCode.BadRequest, nameof(command), exception);
            }

            return await handler.HandleAsync((dynamic)command, (dynamic)cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously sends the command handler(<see cref="ICommandHandler{TCommand, TResult}"/> implementation) on the specified command.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual async Task<IOperationResult<TResult>> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            if (!typeof(CommandHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, command.GetType(), typeof(TResult)))
            {
                throw new InvalidOperationException("Building Command wrapper failed.", typeException);
            }

            if (!_handlerAccessor.TryGetHandler(wrapperType, out var foundHandler, out var ex))
            {
                throw new InvalidOperationException(
                    $"The matching command handler for {command.GetType().Name} is missing. Be sure the {typeof(CommandHandlerWrapper<,>).Name} and handler are registered.", ex);
            }

            var handler = (ICommandHandlerWrapper<TResult>)foundHandler;

            if (!handler.CanHandle(command))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            return await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }

        private static void WriteLineException(Exception exception)
        {
#if DEBUG
            Debug.WriteLine(exception);
#else
            Trace.WriteLine(exception);
#endif
        }
    }
}