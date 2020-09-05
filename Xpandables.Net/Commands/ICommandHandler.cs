
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

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// Allows an application author to define a handler for a specific type command.
    /// The command must implement <see cref="ICommand"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// This interface inherits from <see cref="ICanHandle{TArgument}"/> that determines whether or not the command can be handled. Its default behavior returns <see langword="true"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public interface ICommandHandler<in TCommand> : ICanHandle<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Handle(TCommand command);
    }
}