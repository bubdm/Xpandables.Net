
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
using Xpandables.Net.Transactions;

namespace Xpandables.Net.Decorators.Transactions
{
    /// <summary>
    /// This class allows the application author to add transaction support to command control flow.
    /// The target command should implement the <see cref="ITransactionDecorator"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ITransactionScopeProvider"/>.
    /// The transaction scope definition comes from the <see cref="ITransactionScopeProvider.GetTransactionScope{TArgument}(TArgument)"/> method.
    /// if no transaction is returned, the execution is done normally. If operation is failed, do nothing.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandTransactionDecorator<TCommand> : CommandTransactionDecorator, ICommandHandler<TCommand>
        where TCommand : class, ICommand, ITransactionDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionDecorator{TCommand}"/> class
        /// with the handler to be decorated and the transaction scope provider.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="transactionScopeProvider">The transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="transactionScopeProvider"/> is null.</exception>
        public CommandTransactionDecorator(ICommandHandler<TCommand> decoratee, ITransactionScopeProvider transactionScopeProvider)
            : base(transactionScopeProvider)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        /// <summary>
        /// Asynchronously handles the specified command applying a transaction scope if available and if there is no error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await HandleAsync(_decoratee.HandleAsync(command, cancellationToken), command).ConfigureAwait(false);
    }

    /// <summary>
    /// This class allows the application author to add transaction support to command control flow.
    /// The target query should implement the <see cref="ITransactionDecorator"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ITransactionScopeProvider"/>.
    /// The transaction scope definition comes from the <see cref="ITransactionScopeProvider.GetTransactionScope{TArgument}(TArgument)"/> method.
    /// if no transaction is returned, the execution is done normally. If operation is failed, do nothing.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command that will be used as argument.</typeparam>
    /// <typeparam name="TResult">Type of the result of the query.</typeparam>
    public sealed class CommandTransactionDecorator<TCommand, TResult> : CommandTransactionDecorator, ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, ITransactionDecorator
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionDecorator{TCommand, TResult}"/> class
        /// with the handler to be decorated and the transaction scope provider.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="transactionScopeProvider">The transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="transactionScopeProvider"/> is null.</exception>
        public CommandTransactionDecorator(ICommandHandler<TCommand, TResult> decoratee, ITransactionScopeProvider transactionScopeProvider)
            : base(transactionScopeProvider)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        /// <summary>
        /// Asynchronously handles the specified command applying a transaction scope if available, no error and returns the task result.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await HandleAsync(_decoratee.HandleAsync(command, cancellationToken), command).ConfigureAwait(false);
    }

    /// <summary>
    /// Provides with Command Transaction decorator implementation for command.
    /// </summary>
    public abstract class CommandTransactionDecorator
    {
        private readonly ITransactionScopeProvider _transactionScopeProvider;

        /// <summary>
        /// Constructs a new instance of <see cref="CommandTransactionDecorator"/> with the <see cref="ITransactionScopeProvider"/> instance.
        /// </summary>
        /// <param name="transactionScopeProvider">The transaction scope provider instance.</param>
        protected CommandTransactionDecorator(ITransactionScopeProvider transactionScopeProvider)
            => _transactionScopeProvider = transactionScopeProvider ?? throw new ArgumentNullException(nameof(transactionScopeProvider));

        internal async Task<TOutput> HandleAsync<TOutput, TCommand>(Task<TOutput> handler, TCommand command)
            where TOutput : IOperationResult
            where TCommand : class, ITransactionDecorator
        {
            if (_transactionScopeProvider.GetTransactionScope(command) is { } transaction)
            {
                using var scope = transaction;
                var resultState = await handler.ConfigureAwait(false);
                if (resultState.IsSucceeded)
                    scope.Complete();

                return resultState;
            }
            else
            {
                return await handler.ConfigureAwait(false);
            }
        }
    }
}
