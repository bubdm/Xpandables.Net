
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
using Xpandables.Net.Decorators;
using Xpandables.Net.Validators;

namespace Xpandables.Net.Decorators.Validators
{
    /// <summary>
    /// This class allows the application author to add validation support to command control flow.
    /// The target command should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeValidator{TArgument}"/>
    /// and applies all validators found to the target command before the command get handled if there is no error. You should provide with implementation
    /// of <see cref="IValidator{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandValidatorDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IValidationDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ICompositeValidator<TCommand> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidatorDecorator{TCommand}"/> class
        /// with the handler to be decorated and the composite validator.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public CommandValidatorDecorator(ICommandHandler<TCommand> decoratee, ICompositeValidator<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously validates the command before handling if there is no error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var resultState = await _validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.IsSuccess)
                return await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return resultState;
        }
    }

    /// <summary>
    /// This class allows the application author to add validation support to command control flow.
    /// The target command should implement the <see cref="IValidationDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeValidator{TArgument}"/>
    /// and applies all validators found to the target command before the command get handled if there is no error. You should provide with implementation
    /// of <see cref="IValidator{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandValidatorDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, IValidationDecorator
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;
        private readonly ICompositeValidator<TCommand> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidatorDecorator{TCommand, TResult}"/> class
        /// with the handler to be decorated and the composite validator.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public CommandValidatorDecorator(ICommandHandler<TCommand, TResult> decoratee, ICompositeValidator<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously validates the command before handling if there is no error.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var resultState = await _validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
            if (resultState.IsSuccess)
                return await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return resultState.ToFailureOperationResult<TResult>();
        }
    }
}
