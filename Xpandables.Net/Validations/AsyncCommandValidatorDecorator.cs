
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

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// This class allows the application author to add validation support to command control flow.
    /// The target command should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeValidation{TArgument}"/>
    /// and applies all validators found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IValidation{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class AsyncCommandValidatorDecorator<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, IAsyncCommand, IValidationDecorator
    {
        private readonly IAsyncCommandHandler<TCommand> _decoratee;
        private readonly ICompositeValidation<TCommand> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandValidatorDecorator{TCommand}"/> class 
        /// with the handler to be decorated and the composite validator.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public AsyncCommandValidatorDecorator(IAsyncCommandHandler<TCommand> decoratee, ICompositeValidation<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously validates the command before handling.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(command).ConfigureAwait(false);
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// This class allows the application author to add validation support to command control flow.
    /// The target command should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeValidation{TArgument}"/>
    /// and applies all validators found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IValidation{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncCommandValidatorDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>
        where TCommand : class, IAsyncCommand<TResult>, IValidationDecorator
    {
        private readonly IAsyncCommandHandler<TCommand, TResult> _decoratee;
        private readonly ICompositeValidation<TCommand> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommandValidatorDecorator{TCommand, TResult}"/> class 
        /// with the handler to be decorated and the composite validator.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public AsyncCommandValidatorDecorator(IAsyncCommandHandler<TCommand, TResult> decoratee, ICompositeValidation<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously validates the command before handling.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(command).ConfigureAwait(false);
            return await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
