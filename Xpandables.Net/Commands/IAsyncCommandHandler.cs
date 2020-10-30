
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
    /// Provides with a method to asynchronously handle a command of specific type that implements <see cref="IAsyncCommand"/> interface.
    /// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the command can be handled. Its default behavior returns <see langword="true"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to act on.</typeparam>
    public interface IAsyncCommandHandler<in TCommand> : ICanHandle<TCommand>
        where TCommand : class, IAsyncCommand
    {
        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="command"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides with a method to asynchronously handle a command of specific type that implements <see cref="IAsyncCommand{TResult}"/> interface.
    /// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the command can be handled. Its default behavior returns <see langword="true"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to act on.</typeparam>
    /// <typeparam name="TResult">Type of the result of the command.</typeparam>
    public interface IAsyncCommandHandler<in TCommand, TResult> : ICanHandle<TCommand>
        where TCommand : class, IAsyncCommand<TResult>
    {
        /// <summary>
        /// Asynchronously handles the specified command and returns the task result.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The handler is unable to handle the <paramref name="command"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}