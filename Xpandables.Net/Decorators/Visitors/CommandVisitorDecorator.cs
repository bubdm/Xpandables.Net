
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

using Xpandables.Net.Visitors;

namespace Xpandables.Net.Decorators.Visitors
{
    /// <summary>
    /// This class allows the application author to add visitor support to command control flow.
    /// The target command should implement the <see cref="IVisitable{TVisitable}"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeVisitor{TElement}"/>
    /// and applies all visitors found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IVisitor{TElement}"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandVisitorDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IVisitable<TCommand>
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ICompositeVisitor<TCommand> _visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandVisitorDecorator{TCommand}"/> class with
        /// the handler to be decorated and the composite visitor.
        /// </summary>
        /// <param name="decoratee">the decorated command handler.</param>
        /// <param name="visitor">the visitor to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public CommandVisitorDecorator(ICommandHandler<TCommand> decoratee, ICompositeVisitor<TCommand> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Asynchronously applies visitor and handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            await command.AcceptAsync(_visitor, cancellationToken).ConfigureAwait(false);
            return await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// This class allows the application author to add visitor support to command control flow.
    /// The target command should implement the <see cref="IVisitable{TVisitable}"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeVisitor{TElement}"/>
    /// and applies all visitors found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IVisitor{TElement}"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandVisitorDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, IVisitable<TCommand>
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;
        private readonly ICompositeVisitor<TCommand> _visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandVisitorDecorator{TCommand, TResult}"/> class with
        /// the handler to be decorated and the composite visitor.
        /// </summary>
        /// <param name="decoratee">the decorated command handler.</param>
        /// <param name="visitor">the visitor to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public CommandVisitorDecorator(ICommandHandler<TCommand, TResult> decoratee, ICompositeVisitor<TCommand> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        /// Asynchronously applies visitor and handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            await command.AcceptAsync(_visitor, cancellationToken).ConfigureAwait(false);
            return await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
