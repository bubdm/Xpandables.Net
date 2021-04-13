
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
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand}"/> interface.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    public abstract class CommandHandler<TCommand> : OperationResultBase, ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public abstract Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand, TResult}"/> interface.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class CommandHandler<TCommand, TResult> : OperationResultBase, ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>
    {
        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public abstract Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}