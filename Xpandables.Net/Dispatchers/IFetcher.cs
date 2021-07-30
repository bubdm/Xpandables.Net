
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="IQuery{TResult}"/> and <see cref="IAsyncQuery{TResult}"/>
    /// when targeting <see cref="IAsyncQueryHandler{TQuery, TResult}"/> or <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IFetcher
    {
        /// <summary>
        /// Asynchronously fetches the query to the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> implementation
        /// and returns an enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        IAsyncEnumerable<TResult> FetchAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously fetches the query to the <see cref="IQueryHandler{TQuery, TResult}"/> implementation and returns a result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        Task<IOperationResult<TResult>> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}