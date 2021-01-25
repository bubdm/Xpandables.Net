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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="ICommand"/>, <see cref="ICommand{TResult}"/>, <see cref="IQuery{TResult}"/>, <see cref="INotification"/> and <see cref="IAsyncQuery{TResult}"/>
    /// when targeting <see cref="IAsyncQueryHandler{TQuery, TResult}"/>, <see cref="IQueryHandler{TQuery, TResult}"/>, <see cref="INotificationHandler{TNotification}"/> or/and <see cref="ICommandHandler{TCommand}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDispatcher
    {
        internal IDispatcherHandlerProvider DispatcherHandlerProvider { get; }

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
                WriteLineException(new InvalidOperationException("Building Query wrapper failed.", typeException));
                return AsyncEnumerableExtensions.Empty<TResult>();
            }

            if (!DispatcherHandlerProvider.TryGetHandler(wrapperType, out var foundHandler, out var exception))
            {
                WriteLineException(new InvalidOperationException(
                      $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerWrapper<,>).Name} and handler are registered.", exception));
                return AsyncEnumerableExtensions.Empty<TResult>();
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
                var exception = new InvalidOperationException("Building command handler failed.", typeException);
                WriteLineException(exception);
                return new FailureOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (!DispatcherHandlerProvider.TryGetHandler(handlerType, out dynamic? handler, out var ex))
            {
                var exception = new InvalidOperationException($"The matching command handler for {command.GetType().Name} is missing.", ex);
                WriteLineException(exception);
                return new FailureOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (!((ICanHandle)handler).CanHandle(command))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");
                WriteLineException(exception);
                return new FailureOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
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
                var exception = new InvalidOperationException("Building Command wrapper failed.", typeException);
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (!DispatcherHandlerProvider.TryGetHandler(wrapperType, out var foundHandler, out var ex))
            {
                var exception = new InvalidOperationException($"The matching command handler for {command.GetType().Name} is missing. Be sure the {typeof(CommandHandlerWrapper<,>).Name} and handler are registered.", ex);
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
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
                var exception = new InvalidOperationException("Building Query wrapper failed.", typeException);
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
            }

            if (!DispatcherHandlerProvider.TryGetHandler(wrapperType, out var foundHandler, out var ex))
            {
                var exception = new InvalidOperationException($"The matching command handler for {query.GetType().Name} is missing. Be sure the {typeof(QueryHandlerWrapper<,>).Name} and handler are registered.", ex);
                WriteLineException(exception);
                return new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
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
        /// Asynchronously publishes the notification across all <see cref="INotificationHandler{TNotification}"/>.
        /// </summary>
        /// <param name="notification">The notification to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual async Task PublishAsync(INotification notification, CancellationToken cancellationToken = default)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            if (!typeof(INotificationHandler<>).TryMakeGenericType(out var typeHandler, out var typeException, notification.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building Notification Handler type failed.", typeException));
                return;
            }

            if (!DispatcherHandlerProvider.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching notification handlers for {notification.GetType().Name} are missing.", ex));
                return;
            }

            var handlers = (IEnumerable<INotificationHandler>)foundHandlers;

            await Task.WhenAll(handlers.Select(handler => handler.HandleAsync(notification, cancellationToken))).ConfigureAwait(false);
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