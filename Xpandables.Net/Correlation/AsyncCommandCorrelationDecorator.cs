
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

using Xpandables.Net.Commands;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to command control flow.
    /// The target command should implement the <see cref="ICorrelationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IAsyncCorrelationContext"/> that
    /// adds an event (post event) to be raised after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be raised when exception. The target command handler class implementation should reference the
    /// <see cref="IAsyncCorrelationContext"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public sealed class AsyncCommandCorrelationDecorator<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, IAsyncCommand, ICorrelationDecorator
    {
        private readonly IAsyncCommandHandler<TCommand> _decoratee;
        private readonly AsyncCorrelationContext _correlationContext;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncCommandCorrelationDecorator{TCommand}"/> class with the correlation context and the command handler to be decorated.
        /// </summary>
        /// <param name="correlationContext">The correlation context.</param>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="correlationContext"/> is null.</exception>
        public AsyncCommandCorrelationDecorator(AsyncCorrelationContext correlationContext, IAsyncCommandHandler<TCommand> decoratee)
        {
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handle the specified command adding post/rollback event to the decorated handler.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                await _correlationContext.OnPostEventAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await _correlationContext.OnRollbackEventAsync(exception).ConfigureAwait(false);
                throw;
            }
        }
    }
}