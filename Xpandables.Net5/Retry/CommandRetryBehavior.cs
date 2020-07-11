
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net5.Commands;

namespace Xpandables.Net5.Retry
{
    /// <summary>
    /// This class allows the application author to add retry support to command control flow.
    /// The target command should implement the <see cref="IBehaviorRetry"/> interface in order to activate the behavior.
    /// The target command handler can implement the <see cref="IRetryBehaviorHandler{TArgument}"/> to manage retry execution.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandRetryBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IBehaviorRetry
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly IServiceProvider _serviceProvider;
        private RetryBehaviorAttribute? _retryBehaviorAttribute;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandRetryBehavior{TCommand}"/> class.
        /// </summary>
        /// <param name="decoratee">The decorated command handler.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public CommandRetryBehavior(ICommandHandler<TCommand> decoratee, IServiceProvider serviceProvider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Asynchronously handle the specified command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _retryBehaviorAttribute = command.GetRetryBehaviorAttribute(_serviceProvider).IsValid();
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (RetryBehaviorException)
            {
                throw;
            }
            catch (Exception exception) when (_retryBehaviorAttribute!.ExceptionTypes.Contains(exception.GetType()))
            {
                IRetryContext retryContext = new RetryContext(exception, TimeSpan.FromMilliseconds(_retryBehaviorAttribute!.RetryInterval), 1);

                do
                {
                    if (_decoratee is IRetryBehaviorHandler<TCommand> exceptionHandler)
                        await exceptionHandler.BeforeRetry(command, retryContext).ConfigureAwait(false);

                    try
                    {
                        Thread.Sleep(retryContext.TimeInterval);
                        await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                        retryContext.RetryIsNotFailed();
                    }
                    catch (Exception handledException) when (_retryBehaviorAttribute!.ExceptionTypes.Contains(handledException.GetType()))
                    {
                        retryContext.RetryIsFailed();
                        retryContext.UpdateException(handledException);
                        retryContext.IncreaseRetryCount();
                    }

                } while (retryContext.RetryCount <= _retryBehaviorAttribute.RetryNumber && retryContext.RetryFailed == true);

                if (retryContext.RetryFailed == true)
                    throw;
            }
        }
    }
}
