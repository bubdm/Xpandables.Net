
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
using Xpandables.Net.Extensions;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// This class allows the application author to add logging event support to command control flow.
    /// The target command should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public sealed class CommandLoggingDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, ILoggingDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandLoggingDecorator{TCommand}"/> class with the logger and the command handler to be decorated.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is null.</exception>
        public CommandLoggingDecorator(ILogger logger, ICommandHandler<TCommand> decoratee)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified command adding post/rollback event to the decorated handler.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public void Handle(TCommand command)
        {
            try
            {
                AsyncExtensions.RunSync(_logger.OnEntryLogAsync(_decoratee, command));
                _decoratee.Handle(command);
            }
            catch (Exception exception)
            {
                AsyncExtensions.RunSync(_logger.OnExceptionLogAsync(_decoratee, command, exception));
                throw;
            }
            finally
            {
                AsyncExtensions.RunSync(_logger.OnExitLogAsync(_decoratee, command));
            }
        }
    }
}