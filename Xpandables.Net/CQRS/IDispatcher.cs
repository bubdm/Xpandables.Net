
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

            if (!typeof(AsyncQueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new[] { query.GetType(), typeof(TResult) }))
            {
                WriteLineException(new InvalidOperationException("Building Query wrapper failed.", typeException));
                return AsyncEnumerableExtensions.Empty<TResult>();
            }

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IAsyncQueryHandlerWrapper<TResult> handler)
            {
                WriteLineException(new NotImplementedException(
                      $"The matching query handler for {query.GetType().Name} is missing. Be sure the {typeof(AsyncQueryHandlerWrapper<,>).Name} and handler are registered."));
                return AsyncEnumerableExtensions.Empty<TResult>();
            }

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

            if (!typeof(ICommandHandler<>).TryMakeGenericType(out var handlerType, out var typeException, new[] { command.GetType() }))
            {
                var exception = new InvalidOperationException("Building command handler failed.", typeException);
                WriteLineException(exception);
                return new FailedOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            dynamic? handler = DispatcherHandlerProvider.GetHandler(handlerType);
            if (handler is null)
            {
                var exception = new NotImplementedException($"The matching command handler for {command.GetType().Name} is missing.");
                WriteLineException(exception);
                return new FailedOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (!((ICanHandle)handler).CanHandle(command))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");
                WriteLineException(exception);
                return new FailedOperationResult(HttpStatusCode.InternalServerError, nameof(command), exception);
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

            if (!typeof(CommandHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { command.GetType(), typeof(TResult) }))
            {
                var exception = new InvalidOperationException("Building Command wrapper failed.", typeException);
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not ICommandHandlerWrapper<TResult> handler)
            {
                var exception = new NotImplementedException($"The matching command handler for {command.GetType().Name} is missing. Be sure the {typeof(CommandHandlerWrapper<,>).Name} and handler are registered.");
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
            }

            if (!handler.CanHandle(command))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {command.GetType().Name} type.");
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(command), exception);
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

            if (!typeof(QueryHandlerWrapper<,>).TryMakeGenericType(out var wrapperType, out var typeException, new Type[] { query.GetType(), typeof(TResult) }))
            {
                var exception = new InvalidOperationException("Building Query wrapper failed.", typeException);
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
            }

            if (DispatcherHandlerProvider.GetHandler(wrapperType) is not IQueryHandlerWrapper<TResult> handler)
            {
                var exception = new NotImplementedException($"The matching command handler for {query.GetType().Name} is missing. Be sure the {typeof(QueryHandlerWrapper<,>).Name} and handler are registered.");
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
            }

            if (!handler.CanHandle(query))
            {
                var exception = new ArgumentException($"{handler.GetType().Name} is unable to handle argument of {query.GetType().Name} type.");
                WriteLineException(exception);
                return new FailedOperationResult<TResult>(HttpStatusCode.InternalServerError, nameof(query), exception);
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

            if (DispatcherHandlerProvider.GetHandlers(typeHandler) is not IEnumerable<INotificationHandler> handlers)
            {
                WriteLineException(new NotImplementedException($"Matching notification handlers for {notification.GetType().Name} are missing."));
                return;
            }

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