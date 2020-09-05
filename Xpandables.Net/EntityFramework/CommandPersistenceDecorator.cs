
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

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// This class allows the application author to add persistence support to command control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.Persist()"/> after the main one in the same control flow only
    /// if there is no exception. You can set the <see cref="IDataContext.PersistenceExceptionHandler"/> with the
    /// <see cref="IDataContext.OnPersistenceException(Func{Exception, Exception?})"/> command, in order to manage
    /// the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandPersistenceDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandPersistenceDecorator{TCommand}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public CommandPersistenceDecorator(IDataContext dataContext, ICommandHandler<TCommand> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public void Handle(TCommand command)
        {
            _decoratee.Handle(command);
            _dataContext.Persist();
        }
    }
}