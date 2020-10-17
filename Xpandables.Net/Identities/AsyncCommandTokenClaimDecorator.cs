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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// This class allows the application author to add secured data support to command control flow.
    /// The target command should implement the <see cref="ITokenClaimDecorator"/> and inherit from <see cref="TokenClaim"/>,
    /// <see cref="TokenClaim{TData}"/> or <see cref="TokenClaimExpression{TData, TSource}"/> in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ITokenClaimProvider"/>, that you should
    /// provide an implementation and use an extension method for registration.
    /// The decorator will set the <see cref="TokenClaim.Claims"/> property with the
    /// <see cref="ITokenClaimProvider.ReadTokenClaim"/> before the handler execution.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class AsyncCommandTokenClaimDecorator<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, ITokenClaim, IAsyncCommand, ITokenClaimDecorator
    {
        private readonly ITokenClaimProvider _tokenClaimProvider;
        private readonly IAsyncCommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncCommandTokenClaimDecorator{TCommand}"/> class.
        /// </summary>
        /// <param name="tokenClaimProvider">The secured data provider.</param>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenClaimProvider"/> is null.</exception>
        public AsyncCommandTokenClaimDecorator(ITokenClaimProvider tokenClaimProvider, IAsyncCommandHandler<TCommand> decoratee)
        {
            _tokenClaimProvider = tokenClaimProvider ?? throw new ArgumentNullException(nameof(tokenClaimProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        /// <summary>
        /// Asynchronously handles the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            command.SetClaims(_tokenClaimProvider.ReadTokenClaim());
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}