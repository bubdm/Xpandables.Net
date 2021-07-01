
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
    /// Represents a helper class that allows implementation of the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> interface.
    /// </summary>
    /// <typeparam name="TQuery">Type of argument to act on.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public abstract class AsyncQueryHandler<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>
    {
        /// <summary>
        /// Asynchronously handles the specified query and returns an asynchronous enumerable of result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public virtual IAsyncEnumerable<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
            => AsyncEnumerable<TResult>.Empty();
    }
}