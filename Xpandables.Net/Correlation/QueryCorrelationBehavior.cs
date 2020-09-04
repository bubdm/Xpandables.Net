
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

using Xpandables.Net.Extensions;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query control flow.
    /// The target query should implement the <see cref="IBehaviorCorrelation"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICorrelationContext"/> that
    /// adds an event (post event) to be raised after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be raised when exception. The target query handler class should reference the
    /// <see cref="ICorrelationContext"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryCorrelationBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IBehaviorCorrelation
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly CorrelationContext _correlationContext;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryCorrelationBehavior{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="correlationContext">the event register.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="correlationContext"/> is null.</exception>
        public QueryCorrelationBehavior(CorrelationContext correlationContext, IQueryHandler<TQuery, TResult> decoratee)
        {
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public TResult Handle(TQuery query)
        {
            try
            {
                var results = _decoratee.Handle(query);
                AsyncExtensions.RunSync(_correlationContext.OnPostEventAsync(results));
                return results;
            }
            catch (Exception exception)
            {
                AsyncExtensions.RunSync(_correlationContext.OnRollbackEventAsync(exception));
                throw;
            }
        }
    }
}
