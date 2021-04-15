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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Database;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Decorators.DomainEvents
{
    /// <summary>
    /// This class allows the application author to add domain event handler support to command control flow.
    /// The target command should implement the <see cref="IEventDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDomainEventPublisher"/> and publishes all
    /// the <see cref="IDomainEvent"/> before persistence and after decorated handler execution only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the data context exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandDomainEventDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IEventDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly IDomainEventPublisher _domainEventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDomainEventDecorator{TCommand}"/> class with
        /// the decorated handler and the domain event publisher.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="domainEventPublisher">The domain event publisher.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventPublisher"/> is null.</exception>
        public CommandDomainEventDecorator(ICommandHandler<TCommand> decoratee, IDomainEventPublisher domainEventPublisher)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        /// <summary>
        /// Asynchronously handles the specified command and publishes domain events if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.Failed)
                return resultState;

            await _domainEventPublisher.PublishAsync(cancellationToken).ConfigureAwait(false);
            return resultState;
        }
    }

    /// <summary>
    /// This class allows the application author to add domain event handler support to command control flow.
    /// The target command should implement the <see cref="IEventDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IDataContext"/>, <see cref="IDomainEventPublisher"/> and publishes all
    /// the <see cref="IDomainEvent"/> before persistence and after decorated handler execution only
    /// if there is no exception or error. You can set the <see cref="IDataContext.OnPersistenceException"/> with the
    /// delegate command, in order to manage the exception.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandDomainEventDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, IEventDecorator
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;
        private readonly IDomainEventPublisher _domainEventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDomainEventDecorator{TCommand, TResult}"/> class with
        /// the decorated handler and  the domain event publisher.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="domainEventPublisher">The domain event publisher.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventPublisher"/> is null.</exception>
        public CommandDomainEventDecorator(ICommandHandler<TCommand, TResult> decoratee, IDomainEventPublisher domainEventPublisher)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        /// <summary>
        /// Asynchronously handles the specified command and publishes domain events if there is no exception or error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var resultState = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.Failed)
                return resultState;

            await _domainEventPublisher.PublishAsync(cancellationToken).ConfigureAwait(false);
            return resultState;
        }
    }
}
