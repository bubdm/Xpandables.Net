
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
    /// This interface is used as a marker for commands when using the command pattern.
    /// Class implementation is used with the <see cref="ICommandHandler{TCommand}"/> where
    /// "TCommand" is <see cref="ICommand"/> class implementation.
    /// This can also be enhanced with some useful decorators.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface ICommand { }
#pragma warning restore CA1040 // Avoid empty interfaces

    /// <summary>
    /// Allows an application author to define a handler for a specific type command.
    /// The command must implement <see cref="ICommand"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public interface ICommandHandler<in TCommand> : ICanHandle<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}