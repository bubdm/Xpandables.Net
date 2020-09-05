
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

using Xpandables.Net.Commands;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// This class allows the application author to add secured data support to command control flow.
    /// The target command should implement the <see cref="IIdentityDecorator"/> and inherit from <see cref="IdentityData"/>,
    /// <see cref="IdentityData{TData}"/> or <see cref="IdentityDataExpression{TData, TSource}"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IIdentityDataProvider"/>, that you should
    /// provide an implementation and use an extension method for registration.
    /// The decorator will set the <see cref="IdentityData.Identity"/> property with the
    /// <see cref="IIdentityDataProvider.GetIdentity"/> before the handler execution.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandIdentityDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, IIdentityData, ICommand, IIdentityDecorator
    {
        private readonly IIdentityDataProvider _identityProvider;
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandIdentityDecorator{TCommand}"/> class.
        /// </summary>
        /// <param name="identityProvider">The secured data provider.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="identityProvider"/> is null.</exception>
        public CommandIdentityDecorator(IIdentityDataProvider identityProvider, ICommandHandler<TCommand> decoratee)
        {
            _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public void Handle(TCommand command)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            command.SetIdentity(_identityProvider.GetIdentity());
            _decoratee.Handle(command);
        }
    }
}
