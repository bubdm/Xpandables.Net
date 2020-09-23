﻿
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

using Xpandables.Net.Optionals;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Serilog
{
    /// <summary>
    /// This class allows the application author to add logging event support to query control flow.
    /// The target command should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ILoggerEngine"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncQueryLoggingDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IAsyncQuery<TResult>, ILoggingDecorator
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decoratee;
        private readonly ILoggerEngine _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncQueryLoggingDecorator{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="logger">the logger instance.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is null.</exception>
        public AsyncQueryLoggingDecorator(ILoggerEngine logger, IAsyncQueryHandler<TQuery, TResult> decoratee)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified query and returns an optional type-specific result.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents an optional object that may contains a value of <typeparamref name="TResult"/> or not.</returns>
        public async Task<Optional<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var results = Optional<TResult>.Empty();
            try
            {
                await _logger.OnEntryLogAsync(_decoratee, query).ConfigureAwait(false);
                results = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                return results;
            }
            catch (Exception exception)
            {
                await _logger.OnExceptionLogAsync(_decoratee, query, exception).ConfigureAwait(false);
                throw;
            }
            finally
            {
                await _logger.OnExitLogAsync(_decoratee, query, results.FirstOrDefault()).ConfigureAwait(false);
            }
        }
    }
}
