
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

namespace Xpandables.Net.Queries
{
    /// <summary>
    /// Represents a helper class that allows implementation of the <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}"/> interface without dedicated class.
    /// </summary>
    /// <typeparam name="TQuery">Type of argument to act on.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncQueryHandler<TQuery, TResult> : IAsyncEnumerableQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncEnumerableQuery<TResult>
    {
        private readonly Func<TQuery, CancellationToken, IAsyncEnumerable<TResult>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncQueryHandler{TQuery, TResult}"/> class with the delegate to be used
        /// as <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public AsyncQueryHandler(Func<TQuery, CancellationToken, IAsyncEnumerable<TResult>> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Asynchronously handles the specified query using the delegate from the constructor and returns an asynchronous enumerable of result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="query"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public IAsyncEnumerable<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default) => _handler(query, cancellationToken);
    }
}