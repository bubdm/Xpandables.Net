
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
using System.Net;
using System.Threading.Tasks;

namespace Xpandables.Net.CommandQueryEvents
{
    /// <summary>
    /// Implementation of <see cref="ICommandQueryEvent"/>.
    /// </summary>
    public abstract class CommandQueryEvent : ICommandQueryEvent
    {
        /// <summary>
        /// Gets the unique identifier for the command.
        /// </summary>
        public Guid Guid => Guid.NewGuid();

        /// <summary>
        /// Gets the created date of the command.
        /// </summary>
        public DateTimeOffset CreatedOn => DateTimeOffset.Now;

        /// <summary>
        /// Gets the name of the user running associated with the current command.
        /// The default value is associated with the current thread.
        /// </summary>
        public string CreatedBy => Environment.UserName;

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult ReturnSuccessOperationResult() => new SuccessOperationResult();

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult"/>.</returns>
        protected static Task<IOperationResult> ReturnSuccessOperationResultAsync() => Task.FromResult<IOperationResult>(new SuccessOperationResult());

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult ReturnSuccessOperationResult(HttpStatusCode statusCode) => new SuccessOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult"/>.</returns>
        protected static Task<IOperationResult> ReturnSuccessOperationResultAsync(HttpStatusCode statusCode)
            => Task.FromResult<IOperationResult>(new SuccessOperationResult(statusCode));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult ReturnFailedOperationResult(params OperationError[] errors)
            => new FailureOperationResult(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult"/>.</returns>
        protected static Task<IOperationResult> ReturnFailedOperationResultAsync(params OperationError[] errors)
            => Task.FromResult<IOperationResult>(new FailureOperationResult(errors));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult ReturnFailedOperationResult(HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult(statusCode, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult"/>.</returns>
        protected static Task<IOperationResult> ReturnFailedOperationResultAsync(HttpStatusCode statusCode, params OperationError[] errors)
            => Task.FromResult<IOperationResult>(new FailureOperationResult(statusCode, errors));
    }

    /// <summary>
    /// Implementation of <see cref="ICommandQueryEvent"/>.
    /// </summary>
    public abstract class CommandQueryEvent<TResult> : CommandQueryEvent
    {
        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> ReturnSuccessOperationResult(TResult result) => new SuccessOperationResult<TResult>(result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult{TResult}"/>.</returns>
        protected static Task<IOperationResult<TResult>> ReturnSuccessOperationResultAsync(TResult result)
            => Task.FromResult<IOperationResult<TResult>>(new SuccessOperationResult<TResult>(result));

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> ReturnSuccessOperationResult(
            HttpStatusCode statusCode, TResult result)
            => new SuccessOperationResult<TResult>(statusCode, result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static Task<IOperationResult<TResult>> ReturnSuccessOperationResultAsync(
            HttpStatusCode statusCode, TResult result)
            => Task.FromResult<IOperationResult<TResult>>(new SuccessOperationResult<TResult>(statusCode, result));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> ReturnFailedOperationResult(params OperationError[] errors)
            => new FailureOperationResult<TResult>(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new Task<IOperationResult<TResult>> ReturnFailedOperationResultAsync(params OperationError[] errors)
            => Task.FromResult<IOperationResult<TResult>>(new FailureOperationResult<TResult>(errors));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static new IOperationResult<TResult> ReturnFailedOperationResult(HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult<TResult>(statusCode, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new Task<IOperationResult<TResult>> ReturnFailedOperationResultAsync(HttpStatusCode statusCode, params OperationError[] errors)
            => Task.FromResult<IOperationResult<TResult>>(new FailureOperationResult<TResult>(statusCode, errors));
    }
}
