
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// This class allows the application author to add notification handler support to command control flow.
    /// The target command should implement the <see cref="INotificationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/>, <see cref="INotificationDispatcher"/> and publishes all
    /// the <see cref="INotification"/> before persistence and after decorated handler execution only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandNotificationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, INotificationDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly INotificationDispatcher _notificationDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotificationDecorator{TCommand}"/> class with 
        /// the decorated handler, the notification dispatcher and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="notificationDispatcher">The notification dispatcher.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="notificationDispatcher"/> is null.</exception>
        public CommandNotificationDecorator(IDataContext dataContext, ICommandHandler<TCommand> decoratee, INotificationDispatcher notificationDispatcher)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _notificationDispatcher = notificationDispatcher ?? throw new ArgumentNullException(nameof(notificationDispatcher));
        }

        /// <summary>
        /// Asynchronously handles the specified command and publishes notifications if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.IsFailure)
                return resultState;

            var notificationTasks = _dataContext.Notifications
                .Select(async notification => await _notificationDispatcher.PublishAsync(notification, cancellationToken).ConfigureAwait(false));

            _dataContext.ClearNotifications();
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
            return new SuccessOperationResult();
        }
    }

    /// <summary>
    /// This class allows the application author to add notification handler support to command control flow.
    /// The target command should implement the <see cref="INotificationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/>, <see cref="INotificationDispatcher"/> and publishes all
    /// the <see cref="INotification"/> before persistence and after decorated handler execution only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandNotificationDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, INotificationDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand, TResult> _decoratee;
        private readonly INotificationDispatcher _notificationDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotificationDecorator{TCommand, TResult}"/> class with 
        /// the decorated handler, the notification dispatcher and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="notificationDispatcher">The notification dispatcher.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="notificationDispatcher"/> is null.</exception>
        public CommandNotificationDecorator(IDataContext dataContext, ICommandHandler<TCommand, TResult> decoratee, INotificationDispatcher notificationDispatcher)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _notificationDispatcher = notificationDispatcher ?? throw new ArgumentNullException(nameof(notificationDispatcher));
        }

        /// <summary>
        /// Asynchronously handles the specified command and publishes notifications if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.IsFailure)
                return resultState;

            var notificationTasks = _dataContext.Notifications
                .Select(async notification => await _notificationDispatcher.PublishAsync(notification, cancellationToken).ConfigureAwait(false));

            _dataContext.ClearNotifications();
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
            return resultState;
        }
    }
}
