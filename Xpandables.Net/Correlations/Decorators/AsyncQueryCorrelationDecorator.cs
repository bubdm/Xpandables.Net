﻿
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.Correlations.Decorators
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query control flow.
    /// The target query should implement the <see cref="ICorrelationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICorrelationEvent"/> that
    /// adds an event (post event) to be raised after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be raised when exception. The target query handler class should reference the
    /// <see cref="ICorrelationEvent"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncQueryCorrelationDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>, ICorrelationDecorator
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;
        private readonly CorrelationEvent _correlationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncQueryCorrelationDecorator{TQuery, TResult}"/> class
        /// with the correlation context and the query handler to be decorated..
        /// </summary>
        /// <param name="correlationContext">the event register.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="correlationContext"/> is null.</exception>
        public AsyncQueryCorrelationDecorator(CorrelationEvent correlationContext, IAsyncQueryHandler<TQuery, TResult> decoratee)
        {
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified query using the decorated handler, executes the post event before returning each result,
        /// and executes the rollback event in case of exception.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public async IAsyncEnumerable<TResult> HandleAsync(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await using var asyncEnumerator = _decoratee.HandleAsync(query, cancellationToken).GetAsyncEnumerator(cancellationToken);
            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await _correlationContext.OnRollbackEventAsync(exception).ConfigureAwait(false);
                    throw;
                }

                if (resultExist)
                {
                    await _correlationContext.OnPostEventAsync(asyncEnumerator.Current).ConfigureAwait(false);
                    yield return asyncEnumerator.Current;
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}