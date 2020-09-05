﻿
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
    /// This helper class allows the application author to implement the <see cref="ICommandHandler{TCommand}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandHandlerBuilder<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly Action<TCommand> _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandHandlerBuilder{TArgument}"/> class with the delegate to be used
        /// as <see cref="ICommandHandler{TCommand}.Handle(TCommand)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="ICommandHandler{TCommand}.Handle(TCommand)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public CommandHandlerBuilder(Action<TCommand> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Handles the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public void Handle(TCommand command) => _handler(command);
    }
}