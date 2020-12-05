
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
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// This class allows the application author to add visitor support to query control flow.
    /// The target query should implement the <see cref="IVisitable{TVisitable}"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICompositeVisitor{TElement}"/>
    /// and applies all visitors found to the target query before the query get handled. You should provide with implementation
    /// of <see cref="IVisitor{TElement}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncEnumerableQueryVisitorDecorator<TQuery, TResult> : IAsyncEnumerableQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncEnumerableQuery<TResult>, IVisitable<TQuery>
    {
        private readonly IAsyncEnumerableQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeVisitor<TQuery> _visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEnumerableQueryVisitorDecorator{TQuery, TResult}"/> class with
        /// the query handler to be decorated and the composite visitor.
        /// </summary>
        /// <param name="decoratee">The query to be decorated.</param>
        /// <param name="visitor">The composite visitor to apply</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public AsyncEnumerableQueryVisitorDecorator(IAsyncEnumerableQueryHandler<TQuery, TResult> decoratee, ICompositeVisitor<TQuery> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Asynchronously applies visitor before handling the query and returns an asynchronous result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> HandleAsync(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            await query.AcceptAsync(_visitor, cancellationToken).ConfigureAwait(false);
            await foreach (var result in _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false))
                yield return result;
        }
    }
}
