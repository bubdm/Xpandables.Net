
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

namespace Xpandables.Net.VisitorRules
{
    /// <summary>
    /// This class allows the application author to add visitor support to command control flow.
    /// The target command should implement the <see cref="IVisitable"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeVisitorRule{TElement}"/>
    /// and applies all visitors found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IVisitorRule{TElement}"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandVisitorDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IVisitable
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ICompositeVisitorRule<TCommand> _visitor;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandVisitorDecorator{TCommand}"/>.
        /// </summary>
        /// <param name="decoratee">the decorated command handler.</param>
        /// <param name="visitor">the visitor to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public CommandVisitorDecorator(ICommandHandler<TCommand> decoratee, ICompositeVisitorRule<TCommand> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public void Handle(TCommand command)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            command.Accept(_visitor);
            _decoratee.Handle(command);
        }
    }
}