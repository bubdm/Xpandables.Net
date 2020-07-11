
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

using Xpandables.Net5.Commands;

namespace Xpandables.Net5.Correlation
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to command control flow.
    /// The target command should implement the <see cref="IBehaviorCorrelation"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICorrelationContext"/> that
    /// adds an event (post event) to be executed after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be executed when exception. The target command handler class should reference the
    /// <see cref="ICorrelationContext"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandCorrelationBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IBehaviorCorrelation
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly CorrelationContext _eventRegister;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandCorrelationBehavior{TCommand}"/> class.
        /// </summary>
        /// <param name="eventRegister">The event register.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="eventRegister"/> is null.</exception>
        public CommandCorrelationBehavior(CorrelationContext eventRegister, ICommandHandler<TCommand> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handle the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                await _eventRegister.OnPostEventAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await _eventRegister.OnRollbackEventAsync(exception).ConfigureAwait(false);
                throw;
            }
        }
    }
}