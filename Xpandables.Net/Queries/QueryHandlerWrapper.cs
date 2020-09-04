
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

namespace Xpandables.Net.Queries
{
    /// <summary>
    /// Implementation for <see cref="IQueryHandlerWrapper{TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryHandlerWrapper<TQuery, TResult> : IQueryHandlerWrapper<TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryHandlerWrapper{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="decoratee">The query handler instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        public QueryHandlerWrapper(IQueryHandler<TQuery, TResult> decoratee)
            => _decoratee = decoratee ?? throw new ArgumentNullException($"{decoratee} : {nameof(TQuery)}.{nameof(TResult)}");

        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public TResult Handle(IQuery<TResult> query) => _decoratee.Handle((TQuery)query);
    }
}
