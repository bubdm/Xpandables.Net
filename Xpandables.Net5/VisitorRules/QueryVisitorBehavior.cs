
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

using Xpandables.Net5.Queries;

namespace Xpandables.Net5.VisitorRules
{
    /// <summary>
    /// This class allows the application author to add visitor support to query control flow.
    /// The target query should implement the <see cref="IVisitable"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICompositeVisitorRule{TElement}"/>
    /// and applies all visitors found to the target query before the query get handled. You should provide with implementation
    /// of <see cref="IVisitorRule{TElement}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryVisitorBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IVisitable
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeVisitorRule<TQuery> _visitor;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryVisitorBehavior{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="decoratee">The query to be decorated.</param>
        /// <param name="visitor">The composite visitor to apply</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public QueryVisitorBehavior(IQueryHandler<TQuery, TResult> decoratee, ICompositeVisitorRule<TQuery> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Asynchronously handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            await query.AcceptAsync(_visitor).ConfigureAwait(false);
            return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}