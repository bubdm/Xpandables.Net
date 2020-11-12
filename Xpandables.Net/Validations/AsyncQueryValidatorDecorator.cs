
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

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Validations;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// This class allows the application author to add validation support to query control flow.
    /// The target query should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target query handler with an implementation of <see cref="ICompositeValidation{TArgument}"/>
    /// and applies all validators found to the target query before the command get handled. You should provide with implementation
    /// of <see cref="IValidation{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncQueryValidatorDecorator<TQuery, TResult> : IAsyncEnumerableQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncEnumerableQuery<TResult>, IValidationDecorator
    {
        private readonly IAsyncEnumerableQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeValidation<TQuery> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncQueryValidatorDecorator{TQuery, TResult}"/> class with
        /// the handler to be decorated and the composite validator.
        /// </summary>
        /// <param name="decoratee">The query handler to decorate.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public AsyncQueryValidatorDecorator(IAsyncEnumerableQueryHandler<TQuery, TResult> decoratee, ICompositeValidation<TQuery> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously validates the query before handling and returns an asynchronous result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public IAsyncEnumerable<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            _validator.ValidateAsync(query).RunSync();
            return _decoratee.HandleAsync(query, cancellationToken);
        }
    }
}
