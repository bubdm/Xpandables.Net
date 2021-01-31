
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

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand}"/> interface without dedicated class.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    public sealed class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly Func<TCommand, CancellationToken, Task<IOperationResult>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler{TArgument}"/> class with the delegate to be used
        /// as <see cref="ICommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="ICommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public CommandHandler(Func<TCommand, CancellationToken, Task<IOperationResult>> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Asynchronously handles the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await _handler(command, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand, TResult}"/> interface without dedicated class.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>
    {
        private readonly Func<TCommand, CancellationToken, Task<IOperationResult<TResult>>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler{TArgument, TResult}"/> class with the delegate to be used
        /// as <see cref="ICommandHandler{TCommand, TResult}.HandleAsync(TCommand, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="ICommandHandler{TCommand, TResult}.HandleAsync(TCommand, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public CommandHandler(Func<TCommand, CancellationToken, Task<IOperationResult<TResult>>> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Asynchronously handles the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await _handler(command, cancellationToken).ConfigureAwait(false);
    }
}