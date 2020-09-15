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

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// This class allows the application author to add logging event support to query control flow.
    /// The target command should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryLoggingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ILoggingDecorator
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryLoggingDecorator{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="logger">the logger instance.</param>
        /// <param name="decoratee">The decorated query handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is null.</exception>
        public QueryLoggingDecorator(ILogger logger, IQueryHandler<TQuery, TResult> decoratee)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public TResult Handle(TQuery query)
        {
            try
            {
                AsyncExtensions.RunSync(_logger.OnEntryLogAsync(_decoratee, query));
                var result = _decoratee.Handle(query);
                AsyncExtensions.RunSync(_logger.OnExitLogAsync(_decoratee, query, result));
                return result;
            }
            catch (Exception exception)
            {
                AsyncExtensions.RunSync(_logger.OnExceptionLogAsync(_decoratee, query, exception));
                throw;
            }
        }
    }
}