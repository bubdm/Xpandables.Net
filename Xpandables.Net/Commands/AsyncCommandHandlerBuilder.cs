
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
    /// This helper class allows the application author to implement the <see cref="IAsyncCommandHandler{TCommand}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class AsyncCommandHandlerBuilder<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, IAsyncCommand
    {
        private readonly Func<TCommand, CancellationToken, Task> _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncCommandHandlerBuilder{TArgument}"/> class with the delegate to be used
        /// as <see cref="IAsyncCommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IAsyncCommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public AsyncCommandHandlerBuilder(Func<TCommand, CancellationToken, Task> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Asynchronously handle the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await _handler(command, cancellationToken).ConfigureAwait(false);
    }
}