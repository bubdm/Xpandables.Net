
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

namespace Xpandables.Net.Decorators.Persistences
{
    /// <summary>
    /// This class allows the application author to add persistence support to command control flow with aggregate.
    /// The target command should implement the <see cref="IAggregatePersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IAggregateDataContext"/> and executes the
    /// the <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)"/> if available after the main one in the same control flow only
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class AggregateCommandPersistenceDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IAggregatePersistenceDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly IAggregateDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCommandPersistenceDecorator{TCommand}"/> class with
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public AggregateCommandPersistenceDecorator(IAggregateDataContext context, ICommandHandler<TCommand> decoratee)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            var result = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (result.IsSucceeded)
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }

    /// <summary>
    /// This class allows the application author to add persistence support to command control flow with aggregate.
    /// The target command should implement the <see cref="IAggregatePersistenceDecorator"/>
    /// interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IAggregateDataContext"/> 
    /// and executes the
    /// the <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)"/> 
    /// if available after the main one in the same control flow only
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AggregateCommandPersistenceDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, IAggregatePersistenceDecorator
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;
        private readonly IAggregateDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCommandPersistenceDecorator{TCommand, TResult}"/> class with
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public AggregateCommandPersistenceDecorator(IAggregateDataContext context, ICommandHandler<TCommand, TResult> decoratee)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            var result = await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            if (result.IsSucceeded)
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
