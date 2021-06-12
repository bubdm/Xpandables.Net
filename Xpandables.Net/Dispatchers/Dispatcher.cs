
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
using Xpandables.Net.Handlers;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    internal delegate TReturn CreateResult<TReturn>(bool isValid, string key, string errorMessage) where TReturn : IOperationResult;

    /// <summary>
    /// The implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IAsyncQueryHandler{TQuery, TResult}"/>, <see cref="IQueryHandler{TQuery, TResult}"/> and
    /// <see cref="ICommandHandler{TCommand}"/> process dynamically.
    /// </summary>
    public class Dispatcher : OperationResults, IDispatcher
    {
        private readonly IHandlerAccessor _handlerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher"/> class with the handlers provider.
        /// </summary>
        /// <param name="handlerAccessor">The handlers provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerAccessor"/> is null.</exception>
        public Dispatcher(IHandlerAccessor handlerAccessor)
            => _handlerAccessor = handlerAccessor ?? throw new ArgumentNullException(nameof(handlerAccessor));

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TResult> FetchAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var handler = (IAsyncQueryHandlerWrapper<TResult>)GetHandler(typeof(AsyncQueryHandlerWrapper<,>), query.GetType(), typeof(TResult));

            IOperationResult<TResult> CreateResult(bool isValid, string key, string errorMessage)
                => isValid switch
                {
                    false => InternalErrorOperation<TResult>(key, errorMessage),
                    _ => OkOperation<TResult>()
                };

            var canHandleResult = CanHandle(CreateResult, handler, query);

            if (canHandleResult.Failed)
                return AsyncEnumerableExtensions.Empty<TResult>();

            return handler.HandleAsync(query, cancellationToken);
        }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult<TResult>> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var handler = (IQueryHandlerWrapper<TResult>)GetHandler(typeof(QueryHandlerWrapper<,>), query.GetType(), typeof(TResult));

            IOperationResult<TResult> CreateResult(bool isValid, string key, string errorMessage)
                => isValid switch
                {
                    false => InternalErrorOperation<TResult>(key, errorMessage),
                    _ => OkOperation<TResult>()
                };

            var canHandleResult = CanHandle(CreateResult, handler, query);

            if (canHandleResult.Failed)
                return canHandleResult;

            return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            dynamic handler = GetHandler(typeof(ICommandHandler<>), command.GetType());

            IOperationResult CreateResult(bool isValid, string key, string errorMessage)
                => isValid switch
                {
                    false => InternalErrorOperation(key, errorMessage),
                    _ => OkOperation()
                };

            var canHandleResult = CanHandle(CreateResult, (ICanHandle)handler, command);

            if (canHandleResult.Failed)
                return canHandleResult;

            return await handler.HandleAsync((dynamic)command, (dynamic)cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult<TResult>> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            var handler = (ICommandHandlerWrapper<TResult>)GetHandler(typeof(CommandHandlerWrapper<,>), command.GetType(), typeof(TResult));

            IOperationResult<TResult> CreateResult(bool isValid, string key, string errorMessage)
                => isValid switch
                {
                    false => InternalErrorOperation<TResult>(key, errorMessage),
                    _ => OkOperation<TResult>()
                };

            var canHandleResult = CanHandle(CreateResult, handler, command);

            if (canHandleResult.Failed)
                return canHandleResult;

            return await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }

        private static TReturn CanHandle<TReturn>(CreateResult<TReturn> createResult, ICanHandle handler, object argument)
            where TReturn : IOperationResult
        {
            var errorMessage = $"The handler {handler.GetType().Name} is unable to handle argument of {argument.GetType().Name} type. " +
                $"The target handler method 'CanHandler' returned false.";

            return createResult(handler.CanHandle(argument), nameof(argument), errorMessage);
        }

        private object GetHandler(Type genericType, params Type[] types)
        {
            if (!genericType.TryMakeGenericType(out var handlerType, out var typeException, types))
            {
                throw new InvalidOperationException("Building handler failed.", typeException);
            }

            if (!_handlerAccessor.TryGetHandler(handlerType, out var foundHandler, out var handleException))
            {
                throw new InvalidOperationException($"The matching handler for '{types[1].Name}' is missing.", handleException);
            }

            return foundHandler;
        }
    }
}