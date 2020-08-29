
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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// This class allows the application author to add logging event support to query control flow.
    /// The target command should implement the <see cref="IBehaviorLogging"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryLoggingBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IBehaviorLogging
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryLoggingBehavior{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="logger">the logger instance.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is null.</exception>
        public QueryLoggingBehavior(ILogger logger, IQueryHandler<TQuery, TResult> decoratee)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            try
            {
                await _logger.OnEntryLogAsync(_decoratee, query).ConfigureAwait(false);
                var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                await _logger.OnExitLogAsync(_decoratee, query, result).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                await _logger.OnExceptionLogAsync(_decoratee, query, exception).ConfigureAwait(false);
                throw;
            }
        }
    }
}