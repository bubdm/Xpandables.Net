
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// This class allows the application author to add validation support to query control flow.
    /// The target query should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICompositeValidatorRule{TArgument}"/>
    /// and applies all validators found to the target query before the command get handled. You should provide with implementation
    /// of <see cref="IValidatorRule{TArgument}"/> or <see cref="ValidatorRule{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncQueryValidatorDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>, IValidationDecorator
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeValidatorRule<TQuery> _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncQueryValidatorDecorator{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="decoratee">The query handler to decorate.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public AsyncQueryValidatorDecorator(IAsyncQueryHandler<TQuery, TResult> decoratee, ICompositeValidatorRule<TQuery> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A result contains an collection of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> HandleAsync(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAsync(query).ConfigureAwait(false);
            await foreach (var result in _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false))
                yield return result;
        }
    }
}
