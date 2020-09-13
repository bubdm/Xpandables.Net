
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Enumerables;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query control flow.
    /// The target query should implement the <see cref="ICorrelationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="IAsyncCorrelationContext"/> that
    /// adds an event (post event) to be raised after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be raised when exception. The target query handler class should reference the
    /// <see cref="IAsyncCorrelationContext"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncQueryCorrelationDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>, ICorrelationDecorator
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;
        private readonly AsyncCorrelationContext _correlationContext;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncQueryCorrelationDecorator{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="correlationContext">the event register.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="correlationContext"/> is null.</exception>
        public AsyncQueryCorrelationDecorator(AsyncCorrelationContext correlationContext, IAsyncQueryHandler<TQuery, TResult> decoratee)
        {
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A result contains an collection of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> HandleAsync(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerableAsync = _decoratee.HandleAsync(query, cancellationToken);
            await using var enumeratorAsync = enumerableAsync.GetAsyncEnumerator(cancellationToken);

            var results = new List<TResult>();
            try
            {
                results = await _decoratee.HandleAsync(query, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
                await _correlationContext.OnPostEventAsync(results).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await _correlationContext.OnRollbackEventAsync(exception).ConfigureAwait(false);
                throw;
            }

            await foreach (var result in new AsyncEnumerable<TResult>(results).ConfigureAwait(false))
                yield return result;
        }
    }
}