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

namespace Xpandables.Net.Queries;

/// <summary>
/// Represents a wrapper interface that avoids use of C# dynamics with query pattern and allows type inference for <see cref="IAsyncQueryHandler{TQuery, TResult}"/>.
/// </summary>
/// <typeparam name="TResult">Type of the result.</typeparam>
public interface IAsyncQueryHandlerWrapper<TResult> : ICanHandle
{
    /// <summary>
    /// Asynchronously handles the specified query and returns an asynchronous result type.
    /// </summary>
    /// <param name="query">The query to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
    /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="query"/>.</exception>
    /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
    IAsyncEnumerable<TResult> HandleAsync(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation for <see cref="IAsyncQueryHandlerWrapper{TResult}"/>.
/// </summary>
/// <typeparam name="TQuery">Type of query.</typeparam>
/// <typeparam name="TResult">Type of result.</typeparam>
public sealed class AsyncQueryHandlerWrapper<TQuery, TResult> : IAsyncQueryHandlerWrapper<TResult>
    where TQuery : class, IAsyncQuery<TResult>
{
    private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncQueryHandlerWrapper{TQuery, TResult}"/> class with the handler to be wrapped.
    /// </summary>
    /// <param name="decoratee">The query handler instance to be wrapped.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
    public AsyncQueryHandlerWrapper(IAsyncQueryHandler<TQuery, TResult> decoratee)
        => _decoratee = decoratee ?? throw new ArgumentNullException($"{decoratee} : {nameof(TQuery)}.{nameof(TResult)}");

    /// <summary>
    /// Determines whether or not a an argument can be handled by the underlying context.
    /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// The default behavior returns <see langword="true"/>.
    /// </summary>
    /// <param name="argument">The argument to handle.</param>
    /// <returns><see langword="true"/> if the argument can be handled, otherwise <see langword="false"/></returns>
    public bool CanHandle(object argument) => _decoratee.CanHandle(argument);

    /// <summary>
    /// Asynchronously handles the specified query with the wrapped handler and returns an asynchronous enumerable result type.
    /// </summary>
    /// <param name="query">The query to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
    /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="query"/>.</exception>
    /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
    public IAsyncEnumerable<TResult> HandleAsync(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default)
        => _decoratee.HandleAsync((TQuery)query, cancellationToken);
}
