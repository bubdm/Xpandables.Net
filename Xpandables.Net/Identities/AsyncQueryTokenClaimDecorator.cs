
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
using System.Threading;

using Xpandables.Net.Queries;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// This class allows the application author to add secured information support to query control flow.
    /// The target query should implement the <see cref="ITokenClaimDecorator"/> and inherit from <see cref="TokenClaim"/>,
    /// <see cref="TokenClaim{TData}"/> or <see cref="TokenClaimExpression{TData, TSource}"/> in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ITokenClaimProvider"/>, that you should
    /// provide an implementation and use the extension method for registration.
    /// The decorator will set the <see cref="TokenClaim.Claims"/> property with the
    /// <see cref="ITokenClaimProvider.ReadTokenClaim"/> before the handler execution.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the query.</typeparam>
    public sealed class AsyncQueryTokenClaimDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, ITokenClaim, IAsyncQuery<TResult>, ITokenClaimDecorator
    {
        private readonly ITokenClaimProvider _tokenClaimProvider;
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncQueryTokenClaimDecorator{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="tokenClaimProvider">The secured data provider.</param>
        /// <param name="decoratee">The query handler to decorate with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenClaimProvider"/> is null.</exception>
        public AsyncQueryTokenClaimDecorator(ITokenClaimProvider tokenClaimProvider, IAsyncQueryHandler<TQuery, TResult> decoratee)
        {
            _tokenClaimProvider = tokenClaimProvider ?? throw new ArgumentNullException(nameof(tokenClaimProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified query and returns an asynchronous result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public IAsyncEnumerable<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            query.SetClaims(_tokenClaimProvider.ReadTokenClaim());
            return _decoratee.HandleAsync(query, cancellationToken);
        }
    }
}
