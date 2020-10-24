
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// This class allows the application author to add persistence support to query control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.PersistAsync(CancellationToken)"/> after the main one in the same control flow only
    /// if there is no exception. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// handler command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryPersistenceDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryPersistenceDecorator{TQuery, TResult}"/> class with 
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public QueryPersistenceDecorator(IDataContext dataContext, IQueryHandler<TQuery, TResult> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified query, persists the changes to database and returns the expected task result.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an object <typeparamref name="TResult"/> or not.</returns>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var results = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
            return results;
        }
    }
}
