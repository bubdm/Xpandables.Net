
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.VisitorRules
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
    public sealed class QueryVisitorDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IVisitable
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeVisitorRule<TQuery> _visitor;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryVisitorDecorator{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="decoratee">The query to be decorated.</param>
        /// <param name="visitor">The composite visitor to apply</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public QueryVisitorDecorator(IQueryHandler<TQuery, TResult> decoratee, ICompositeVisitorRule<TQuery> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <returns>An object that represents the result of <typeparamref name="TResult"/>.</returns>
        public TResult Handle(TQuery query)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            query.Accept(_visitor);
            return _decoratee.Handle(query);
        }
    }
}