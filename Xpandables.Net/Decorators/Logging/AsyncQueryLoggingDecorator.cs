
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

using Xpandables.Net.Queries;

namespace Xpandables.Net.Decorators.Logging
{
    /// <summary>
    /// This class allows the application author to add logging support to query control flow.
    /// The target query should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncQueryLoggingDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>, ILoggingDecorator
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;
        private readonly IOperationResultLogger _operationResultLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncQueryLoggingDecorator{TQuery, TResult}"/> class with
        /// the query handler to be decorated and the handler logger.
        /// </summary>
        /// <param name="decoratee">The query to be decorated.</param>
        /// <param name="operationResultLogger">The operation result logger to apply</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="operationResultLogger"/> is null.</exception>
        public AsyncQueryLoggingDecorator(IAsyncQueryHandler<TQuery, TResult> decoratee, IOperationResultLogger operationResultLogger)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _operationResultLogger = operationResultLogger ?? throw new ArgumentNullException(nameof(operationResultLogger));
        }

        /// <summary>
        /// Asynchronously applies logging on method handling and returns an asynchronous result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        public async IAsyncEnumerable<TResult> HandleAsync(TQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            _operationResultLogger.OnEntry(new(_decoratee, query, default, default));
            Exception? handledException = default;

            await using var asyncEnumerator = _decoratee.HandleAsync(query, cancellationToken).GetAsyncEnumerator(cancellationToken);
            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    resultExist = false;
                    handledException = exception;
                    _operationResultLogger.OnException(new(_decoratee, query, default, exception));
                    throw;
                }
                finally
                {
                    _operationResultLogger.OnExit(new(_decoratee, query, default, handledException));
                }

                if (resultExist)
                {
                    _operationResultLogger.OnSuccess(new(_decoratee, query, new SuccessOperationResult<TResult>(asyncEnumerator.Current), default));
                    yield return asyncEnumerator.Current;
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
