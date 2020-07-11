
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

namespace Xpandables.Net5.Transactions
{
    /// <summary>
    /// This class allows the application author to add transaction support to query control flow.
    /// The target query should implement the <see cref="IBehaviorTransaction"/> in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ITransactionScopeProvider"/>, that you should
    /// provide an implementation and use the extension method for registration.
    /// The transaction scope definition comes from the <see cref="ITransactionScopeProvider.GetTransactionScope{TArgument}(TArgument)"/> method.
    /// if no transaction is returned, the execution is done normally.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class QueryTransactionBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IBehaviorTransaction
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ITransactionScopeProvider _transactionScopeProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryTransactionBehavior{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="decoratee">The query handler to decorate.</param>
        /// <param name="transactionScopeProvider">The transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="transactionScopeProvider"/> is null.</exception>
        public QueryTransactionBehavior(IQueryHandler<TQuery, TResult> decoratee, ITransactionScopeProvider transactionScopeProvider)
        {
            _transactionScopeProvider = transactionScopeProvider ?? throw new ArgumentNullException(nameof(transactionScopeProvider));
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
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var optionalAttribute = _transactionScopeProvider.GetTransactionScope(query);

            if (optionalAttribute is { } transaction)
            {
                using var scope = transaction;

                var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                scope.Complete();

                return result;
            }
            else
            {
                return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
