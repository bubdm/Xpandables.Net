
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

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand}"/> interface.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Asynchronously handles the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public abstract Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="System.Net.HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected IOperationResult ReturnSuccessOperationResult() => new SuccessOperationResult();

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected IOperationResult ReturnSuccessOperationResult(System.Net.HttpStatusCode statusCode) => new SuccessOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="System.Net.HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected IOperationResult ReturnFailedOperationResult(params OperationError[] errors)
            => new FailureOperationResult(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected IOperationResult ReturnFailedOperationResult(System.Net.HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult(statusCode, errors);
    }

    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="ICommandHandler{TCommand, TResult}"/> interface.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to act on.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>
    {
        /// <summary>
        /// Asynchronously handles the specified command using the delegate from the constructor.
        /// </summary>
        /// <param name="command">The command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public abstract Task<IOperationResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="System.Net.HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected IOperationResult<TResult> ReturnSuccessOperationResult(TResult result) => new SuccessOperationResult<TResult>(result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected IOperationResult<TResult> ReturnSuccessOperationResult(
            System.Net.HttpStatusCode statusCode, TResult result)
            => new SuccessOperationResult<TResult>(statusCode, result);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="System.Net.HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected IOperationResult<TResult> ReturnFailedOperationResult(params OperationError[] errors)
            => new FailureOperationResult<TResult>(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected IOperationResult<TResult> ReturnFailedOperationResult(System.Net.HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult<TResult>(statusCode, errors);
    }
}