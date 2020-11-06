
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
    /// Represents a wrapper interface that avoids use of C# dynamics with query pattern and allows type inference for <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IAsyncEnumerableQueryHandlerWrapper<TResult> : ICanHandle
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
        IAsyncEnumerable<TResult> HandleAsync(IAsyncEnumerableQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}