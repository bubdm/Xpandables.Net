
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

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// Defines a set of methods to automatically handle <see cref="ICommand"/>, <see cref="ICommand{TResult}"/>
    /// when targeting <see cref="ICommandHandler{TCommand}"/> or <see cref="ICommandHandler{TCommand, TResult}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Asynchronously sends the command to the <see cref="ICommandHandler{TCommand}"/> implementation handler.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        Task<IOperationResult> SendAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously sends the command to the <see cref="ICommandHandler{TCommand}"/> implementation handler and return a result..
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        Task<IOperationResult<TResult>> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    }
}