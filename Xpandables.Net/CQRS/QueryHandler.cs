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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Represents a helper class that allows implementation of the <see cref="IQueryHandler{TQuery, TResult}"/> interface without dedicated class.
    /// </summary>
    /// <typeparam name="TQuery">Type of argument to act on.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly Func<TQuery, CancellationToken, Task<IOperationResult<TResult>>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandler{TQuery, TResult}"/> class with the delegate to be used
        /// as <see cref="IQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public QueryHandler(Func<TQuery, CancellationToken, Task<IOperationResult<TResult>>> handler) => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Asynchronously handles the specified query using the delegate from the constructor and returns the task result.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default) => await _handler(query, cancellationToken).ConfigureAwait(false);
    }
}