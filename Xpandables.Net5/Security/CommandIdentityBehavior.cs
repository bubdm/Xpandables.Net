
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
using System.Design.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Behaviors
{
    /// <summary>
    /// This class allows the application author to add secured data support to command control flow.
    /// The target command should implement the <see cref="IBehaviorIdentity"/> and inherit from <see cref="IdentityData"/>,
    /// <see cref="IdentityData{TData}"/> or <see cref="IdentityExpression{TData, TSource}"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="IIdentityProvider"/>, that you should
    /// provide an implementation and use the extension method
    /// <see cref="ServiceCollectionExtensions.AddXIdentityBehavior{TSecuredDataProvider}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
    /// for registration. The decorator will set the <see cref="IdentityData.Identity"/> property with the
    /// <see cref="IIdentityProvider.GetIdentity"/> before the handler execution.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandIdentityBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, IIdentityData, ICommand, IBehaviorIdentity
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandIdentityBehavior{TCommand}"/> class.
        /// </summary>
        /// <param name="identityProvider">The secured data provider.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="identityProvider"/> is null.</exception>
        public CommandIdentityBehavior(IIdentityProvider identityProvider, ICommandHandler<TCommand> decoratee)
        {
            _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handle the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            command.SetIdentity(_identityProvider.GetIdentity());
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
