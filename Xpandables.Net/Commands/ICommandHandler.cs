
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
    /// Provides with a method to asynchronously handle a command of specific type that implements <see cref="ICommand"/> interface.
    /// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the command can be handled.
    /// Its default behavior returns <see langword="true"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to act on.</typeparam>
    public interface ICommandHandler<in TCommand> : ICanHandle<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        /// <remarks>You can throw an <see cref="OperationResultException"/> also.</remarks>
        Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides with a method to asynchronously handle a command of specific type that implements <see cref="ICommand{TResult}"/> interface.
    /// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the command can be handled.
    /// Its default behavior returns <see langword="true"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to act on.</typeparam>
    /// <typeparam name="TResult">Type of the result of the command.</typeparam>
    public interface ICommandHandler<in TCommand, TResult> : ICanHandle<TCommand>
        where TCommand : class, ICommand<TResult>
    {
        /// <summary>
        /// Asynchronously handles the specified command and returns the task result.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        /// <remarks>You can throw an <see cref="OperationResultException"/> also.</remarks>
        Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}