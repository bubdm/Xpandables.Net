
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

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// This class allows the application author to add validation support to command control flow.
    /// The target command should implement the <see cref="IBehaviorValidation"/> interface in order to activate the behavior.
    /// The class decorates the target command handler with an implementation of <see cref="ICompositeValidatorRule{TArgument}"/>
    /// and applies all validators found to the target command before the command get handled. You should provide with implementation
    /// of <see cref="IValidatorRule{TArgument}"/> or <see cref="ValidatorRule{TArgument}"/> for validation.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandValidatorBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IBehaviorValidation
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ICompositeValidatorRule<TCommand> _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandValidatorBehavior{TCommand}"/>.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="validator">The validator instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validator"/> is null.</exception>
        public CommandValidatorBehavior(ICommandHandler<TCommand> decoratee, ICompositeValidatorRule<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Asynchronously handle the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(command).ConfigureAwait(false);
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}