
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
using Xpandables.Net.Database;
using Xpandables.Net.Decorators;

namespace Xpandables.Net.Decorators.Persistences
{
    /// <summary>
    /// This class allows the application author to add persistence support to command control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.PersistAsync(CancellationToken)"/> after the main one in the same control flow only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandPersistenceDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandPersistenceDecorator{TCommand}"/> class with
        /// the decorated handler and the db context to act on.
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
        /// Asynchronously handles the specified command and persists changes to database if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.Succeeded)
                await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

            return resultState;
        }
    }

    /// <summary>
    /// This class allows the application author to add persistence support to command control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.PersistAsync(CancellationToken)"/> after the main one in the same control flow only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandPersistenceDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandPersistenceDecorator{TCommand, TResult}"/> class with
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public CommandPersistenceDecorator(IDataContext dataContext, ICommandHandler<TCommand, TResult> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified command and persists changes to database if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.Succeeded)
                await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

            return resultState;
        }
    }
}
