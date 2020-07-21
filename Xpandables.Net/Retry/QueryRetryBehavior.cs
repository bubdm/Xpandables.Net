
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Queries;

namespace Xpandables.Net.Retry
{
    /// <summary>
    /// This class allows the application author to add persistence support to query control flow.
    /// The target query should implement the <see cref="IBehaviorRetry"/> interface in order to activate the behavior.
    /// The target command handler can implement the <see cref="IRetryBehaviorHandler{TArgument}"/> to manage retry execution.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the query.</typeparam>
    public sealed class QueryRetryBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IBehaviorRetry
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly IServiceProvider _serviceProvider;
        private RetryBehaviorAttribute? _retryBehaviorAttribute;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryRetryBehavior{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="decoratee">The query to decorate.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        public QueryRetryBehavior(IQueryHandler<TQuery, TResult> decoratee, IServiceProvider serviceProvider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
            try
            {
                _retryBehaviorAttribute = query.GetRetryBehaviorAttribute(_serviceProvider).IsValid();
                return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (RetryBehaviorException)
            {
                throw;
            }
            catch (Exception exception) when (_retryBehaviorAttribute!.ExceptionTypes.Contains(exception.GetType()))
            {
                IRetryContext retryContext = new RetryContext(exception, TimeSpan.FromMilliseconds(_retryBehaviorAttribute!.RetryInterval), 1);

                do
                {
                    if (_decoratee is IRetryBehaviorHandler<TQuery> exceptionHandler)
                        await exceptionHandler.BeforeRetry(query, retryContext).ConfigureAwait(false);

                    try
                    {
                        Thread.Sleep(retryContext.TimeInterval);
                        var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                        retryContext.RetryIsNotFailed();
                        return result;
                    }
                    catch (Exception handledException) when (_retryBehaviorAttribute!.ExceptionTypes.Contains(handledException.GetType()))
                    {
                        retryContext.RetryIsFailed();
                        retryContext.UpdateException(handledException);
                        retryContext.IncreaseRetryCount();
                    }

                } while (retryContext.RetryCount <= _retryBehaviorAttribute.RetryNumber && retryContext.RetryFailed == true);

                throw;
            }
        }
    }
}
