
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

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// This class allows the application author to add persistence support to command control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.PersistAsync(CancellationToken)"/> after the main one in the same control flow only
    /// if there is no exception. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class AsyncCommandPersistenceDecorator<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, IAsyncCommand, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly IAsyncCommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandPersistenceDecorator{TCommand}"/> class with 
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public AsyncCommandPersistenceDecorator(IDataContext dataContext, IAsyncCommandHandler<TCommand> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified command and persists changes to database.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// This class allows the application author to add persistence support to command control flow.
    /// The target command should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContext.PersistAsync(CancellationToken)"/> after the main one in the same control flow only
    /// if there is no exception. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncCommandPersistenceDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>
        where TCommand : class, IAsyncCommand<TResult>, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly IAsyncCommandHandler<TCommand, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandPersistenceDecorator{TCommand, TResult}"/> class with 
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public AsyncCommandPersistenceDecorator(IDataContext dataContext, IAsyncCommandHandler<TCommand, TResult> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified command and persists changes to database.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the <typeparamref name="TResult"/> operation.</returns>
        public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
