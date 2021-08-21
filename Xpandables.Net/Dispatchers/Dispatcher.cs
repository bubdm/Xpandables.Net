
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
using System.Diagnostics;

using Xpandables.Net.Aggregates.Events;
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers;

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
                false => InternalErrorOperation<TResult>(new OperationErrorCollection(key, errorMessage)),
                _ => OkOperation<TResult>()
            };

        var canHandleResult = CanHandle(CreateResult, handler, query);

        if (canHandleResult.IsFailed)
            return AsyncEnumerable<TResult>.Empty();

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
                false => InternalErrorOperation<TResult>(new OperationErrorCollection(key, errorMessage)),
                _ => OkOperation<TResult>()
            };

        var canHandleResult = CanHandle(CreateResult, handler, query);

        if (canHandleResult.IsFailed)
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
                false => InternalErrorOperation(new OperationErrorCollection(key, errorMessage)),
                _ => OkOperation()
            };

        var canHandleResult = CanHandle(CreateResult, (ICanHandle)handler, command);

        if (canHandleResult.IsFailed)
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
                false => InternalErrorOperation<TResult>(new OperationErrorCollection(key, errorMessage)),
                _ => OkOperation<TResult>()
            };

        var canHandleResult = CanHandle(CreateResult, handler, command);

        if (canHandleResult.IsFailed)
            return canHandleResult;

        return await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public virtual async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        var handlers = GetEventHandlers(@event);
        if (!handlers.Any()) return;

        var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private IEnumerable<IEventHandler> GetEventHandlers(IEvent @event)
    {
        var genericHandlerType = @event switch
        {
            INotification => typeof(INotificationHandler<>),
            IDomainEvent => typeof(IDomainEventHandler<>),
            _ => throw new NotSupportedException($"{@event.GetType().Name} is not supported.")
        };

        if (!genericHandlerType.TryMakeGenericType(out var typeHandler, out var typeException, @event.GetType()))
        {
            Trace.WriteLine(
                new InvalidOperationException("Building event Handler type failed.", typeException));

            return Enumerable.Empty<IEventHandler>();
        }

        if (!_handlerAccessor.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
        {
            Trace.WriteLine(new InvalidOperationException($"Matching event handlers " +
                $"for {@event.GetType().Name} are missing.", ex));

            return Enumerable.Empty<IEventHandler>();
        }

        return (IEnumerable<IEventHandler>)foundHandlers;
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
            throw new InvalidOperationException($"The matching handler for '{types[0].Name}' is missing.", handleException);
        }

        return foundHandler;
    }
}
