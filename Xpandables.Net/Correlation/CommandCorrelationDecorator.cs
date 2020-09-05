
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

using Xpandables.Net.Commands;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to command control flow.
    /// The target command should implement the <see cref="ICorrelationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICorrelationContext"/> that
    /// adds an event (post event) to be raised after the main one in the same control flow only if there is no exception,
    /// and an event (roll back event) to be raised when exception. The target command handler class implementation should reference the
    /// <see cref="ICorrelationContext"/> interface in order to set the expected actions.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public sealed class CommandCorrelationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, ICorrelationDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly CorrelationContext _correlationContext;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandCorrelationDecorator{TCommand}"/> class with the correlation context and the command handler to be decorated.
        /// </summary>
        /// <param name="correlationContext">The correlation context.</param>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="correlationContext"/> is null.</exception>
        public CommandCorrelationDecorator(CorrelationContext correlationContext, ICommandHandler<TCommand> decoratee)
        {
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified command adding post/rollback event to the decorated handler.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public void Handle(TCommand command)
        {
            try
            {
                _decoratee.Handle(command);
                _correlationContext.OnPostEvent();
            }
            catch (Exception exception)
            {
                _correlationContext.OnRollbackEvent(exception);
                throw;
            }
        }
    }
}
