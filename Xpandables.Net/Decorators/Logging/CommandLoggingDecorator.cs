
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

namespace Xpandables.Net.Decorators.Logging
{
    /// <summary>
    /// This class allows the application author to add logging support to command control flow.
    /// The target command should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandLoggingDecorator<TCommand> : CommandLoggingDecorator, ICommandHandler<TCommand>
        where TCommand : class, ICommand, ILoggingDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLoggingDecorator{TCommand}"/> class
        /// with the handler to be decorated and the handler logger.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="commandQueryLogger">The operation result logger instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandQueryLogger"/> is null.</exception>
        public CommandLoggingDecorator(ICommandHandler<TCommand> decoratee, ICommandQueryLogger commandQueryLogger)
            : base(commandQueryLogger)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        /// <summary>
        /// Asynchronously applies logging on handling the command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public async Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await HandleAsync(_decoratee.HandleAsync(command, cancellationToken), _decoratee, command).ConfigureAwait(false);
    }

    /// <summary>
    /// This class allows the application author to add logging support to command control flow.
    /// The target command should implement the <see cref="ILoggingDecorator"/> interface in order to activate the behavior.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class CommandLoggingDecorator<TCommand, TResult> : CommandLoggingDecorator, ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>, ILoggingDecorator
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLoggingDecorator{TCommand, TResult}"/> class
        /// with the handler to be decorated and the handler logger.
        /// </summary>
        /// <param name="decoratee">The command handler to be decorated.</param>
        /// <param name="commandQueryLogger">The handler logger instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandQueryLogger"/> is null.</exception>
        public CommandLoggingDecorator(ICommandHandler<TCommand, TResult> decoratee, ICommandQueryLogger commandQueryLogger)
            : base(commandQueryLogger)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        /// <summary>
        /// Asynchronously applies logging on handling the command.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
        public async Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await HandleAsync(_decoratee.HandleAsync(command, cancellationToken), _decoratee, command).ConfigureAwait(false);
    }

    /// <summary>
    /// Provides with Logging decorator implementation for command.
    /// </summary>
    public abstract class CommandLoggingDecorator
    {
        private readonly ICommandQueryLogger _commandQueryLogger;

        /// <summary>
        /// Constructs a new instance of <see cref="CommandLoggingDecorator"/> with the <see cref="ICommandQueryLogger"/> instance.
        /// </summary>
        /// <param name="commandQueryLogger">The operation result logger instance.</param>
        protected CommandLoggingDecorator(ICommandQueryLogger commandQueryLogger)
            => _commandQueryLogger = commandQueryLogger ?? throw new ArgumentNullException(nameof(commandQueryLogger));

        internal async Task<TOutput> HandleAsync<TOutput>(Task<TOutput> handler, object decoratee, object command)
            where TOutput : IOperationResult
        {
            await _commandQueryLogger.OnEntryAsync(new(decoratee, command, default, default));
            TOutput? result = default;
            Exception? handledException = default;

            try
            {
                result = await handler.ConfigureAwait(false);
                await _commandQueryLogger.OnSuccessAsync(new(decoratee, command, result, default));
                return result;
            }
            catch (Exception exception)
            {
                handledException = exception;
                await _commandQueryLogger.OnExceptionAsync(new(decoratee, command, default, exception));
                throw;
            }
            finally
            {
                await _commandQueryLogger.OnExitAsync(new(decoratee, command, result, handledException));
            }
        }
    }
}
