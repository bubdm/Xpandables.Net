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
/// Defines a generic method that a class implements to handle a type-specific query and returns a type-specific result.
/// The implementation must be thread-safe when working in a multi-threaded environment.
/// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the query can be handled. Its default behavior returns <see langword="true"/>.
/// </summary>
/// <typeparam name="TQuery">Type of the query that will be used as argument.</typeparam>
/// <typeparam name="TResult">Type of the result of the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : ICanHandle<TQuery>
    where TQuery : class, IQuery<TResult>
{
    /// <summary>
    /// Asynchronously handles the specified query and returns the task result.
    /// </summary>
    /// <param name="query">The query to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
    /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
    /// <remarks>You can throw an <see cref="OperationResultException"/> also.</remarks>
    Task<IOperationResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a helper class that allows implementation of the <see cref="IQueryHandler{TQuery, TResult}"/> interface.
/// </summary>
/// <typeparam name="TQuery">Type of argument to act on.</typeparam>
/// <typeparam name="TResult">Type of result.</typeparam>
public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    ///<inheritdoc/>
    public abstract Task<IOperationResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
