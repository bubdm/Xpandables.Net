
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
using System.Transactions;

using Xpandables.Net.Commands;

namespace Xpandables.Net.Transactions
{
    /// <summary>
    /// This class allows the application author to add transaction support to command control flow.
    /// The target command should implement the <see cref="ITransactionDecorator"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ITransactionScopeProvider"/>, that you should
    /// provide an implementation and use the extension method <see langword="AddTransactionScopeDecorator{TTransactionScopeProvider}"/>
    /// for registration. The transaction scope definition comes from the
    /// <see cref="ITransactionScopeProvider.GetTransactionScope{TCommand}(TCommand)"/> method.
    /// if no transaction is returned, the execution is done normally.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class AsyncCommandTransactionDecorator<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, IAsyncCommand, ITransactionDecorator
    {
        private readonly IAsyncCommandHandler<TCommand> _decoratee;
        private readonly ITransactionScopeProvider _transactionScopeProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncCommandTransactionDecorator{TCommand}"/>.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="transactionScopeProvider">The transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="transactionScopeProvider"/> is null.</exception>
        public AsyncCommandTransactionDecorator(IAsyncCommandHandler<TCommand> decoratee, ITransactionScopeProvider transactionScopeProvider)
        {
            _transactionScopeProvider = transactionScopeProvider ?? throw new ArgumentNullException(nameof(transactionScopeProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            if (_transactionScopeProvider.GetTransactionScope(command) is TransactionScope transaction)
            {
                using var scope = transaction;
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                scope.Complete();
            }
            else
            {
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
