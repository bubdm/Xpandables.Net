
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
using System.Transactions;

using Xpandables.Net.Queries;

namespace Xpandables.Net.Transactions
{
    /// <summary>
    /// This class allows the application author to add transaction support to query control flow.
    /// The target query should implement the <see cref="ITransactionDecorator"/> in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ITransactionScopeProvider"/>.
    /// The transaction scope definition comes from the <see cref="ITransactionScopeProvider.GetTransactionScope{TArgument}(TArgument)"/> method.
    /// if no transaction is returned, the execution is done normally.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query that will be used as argument.</typeparam>
    /// <typeparam name="TResult">Type of the result of the query.</typeparam>
    public sealed class QueryTransactionDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ITransactionDecorator
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ITransactionScopeProvider _transactionScopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTransactionDecorator{TQuery, TResult}"/> class 
        /// with the handler to be decorated and the transaction scope provider.
        /// </summary>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <param name="transactionScopeProvider">The transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="transactionScopeProvider"/> is null.</exception>
        public QueryTransactionDecorator(IQueryHandler<TQuery, TResult> decoratee, ITransactionScopeProvider transactionScopeProvider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _transactionScopeProvider = transactionScopeProvider ?? throw new ArgumentNullException(nameof(transactionScopeProvider));
        }

        /// <summary>
        /// Asynchronously handles the specified query applying a transaction scope if available and returns the task result.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an object <typeparamref name="TResult"/> or not.</returns>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            if (_transactionScopeProvider.GetTransactionScope(query) is TransactionScope transaction)
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
