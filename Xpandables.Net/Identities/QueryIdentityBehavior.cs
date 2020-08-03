
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

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// This class allows the application author to add secured information support to query control flow.
    /// The target query should implement the <see cref="IBehaviorIdentity"/> and inherit from <see cref="IdentityData"/>,
    /// <see cref="IdentityData{TData}"/> or <see cref="IdentityDataExpression{TData, TSource}"/> in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="IIdentityDataProvider"/>, that you should
    /// provide an implementation and use the extension method for registration.
    /// The decorator will set the <see cref="IdentityData.Identity"/> property with the
    /// <see cref="IIdentityDataProvider.GetIdentity"/> before the handler execution.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the query.</typeparam>
    public sealed class QueryIdentityBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IIdentityData, IQuery<TResult>, IBehaviorIdentity
    {
        private readonly IIdentityDataProvider _identityDataProvider;
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryIdentityBehavior{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="identityProvider">The secured data provider.</param>
        /// <param name="decoratee">The query handler to decorate with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="identityProvider"/> is null.</exception>
        public QueryIdentityBehavior(IIdentityDataProvider identityProvider, IQueryHandler<TQuery, TResult> decoratee)
        {
            _identityDataProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
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
            if (query is null) throw new ArgumentNullException(nameof(query));

            query.SetIdentity(_identityDataProvider.GetIdentity());
            return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}
