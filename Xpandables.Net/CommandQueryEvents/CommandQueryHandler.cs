
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
    /// Provides with <see cref="IOperationResult"/> extension methods.
    /// </summary>
    public abstract class CommandQueryHandler
    {
        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult SuccessOperation() => new SuccessOperationResult();

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult"/>.</returns>
        protected static Task<IOperationResult> SuccessOperationAsync() => Task.FromResult<IOperationResult>(new SuccessOperationResult());

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult SuccessOperation(HttpStatusCode statusCode) => new SuccessOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult"/>.</returns>
        protected static Task<IOperationResult> SuccessOperationAsync(HttpStatusCode statusCode)
            => Task.FromResult<IOperationResult>(new SuccessOperationResult(statusCode));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult FailureOperation(HttpStatusCode statusCode)
            => new FailureOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static Task<IOperationResult> FailureOperationAsync(HttpStatusCode statusCode)
            => Task.FromResult<IOperationResult>(new FailureOperationResult(statusCode));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult FailureOperation(params OperationError[] errors)
            => new FailureOperationResult(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult"/>.</returns>
        protected static Task<IOperationResult> FailureOperationAsync(params OperationError[] errors)
            => Task.FromResult<IOperationResult>(new FailureOperationResult(errors));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult FailureOperation(HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult(statusCode, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult"/>.</returns>
        protected static Task<IOperationResult> FailureOperationAsync(HttpStatusCode statusCode, params OperationError[] errors)
            => Task.FromResult<IOperationResult>(new FailureOperationResult(statusCode, errors));
    }

    /// <summary>
    /// Provides with <see cref="IOperationResult{TValue}"/> extension methods.
    /// </summary>
    public abstract class CommandQueryHandler<TResult> : CommandQueryHandler
    {
        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> SuccessOperation(TResult result) => new SuccessOperationResult<TResult>(result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult{TResult}"/>.</returns>
        protected static Task<IOperationResult<TResult>> SuccessOperationAsync(TResult result)
            => Task.FromResult<IOperationResult<TResult>>(new SuccessOperationResult<TResult>(result));

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> SuccessOperation(
            HttpStatusCode statusCode, TResult result)
            => new SuccessOperationResult<TResult>(statusCode, result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A task that represents an object of <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static Task<IOperationResult<TResult>> SuccessOperationAsync(
            HttpStatusCode statusCode, TResult result)
            => Task.FromResult<IOperationResult<TResult>>(new SuccessOperationResult<TResult>(statusCode, result));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> FailureOperation(HttpStatusCode statusCode)
            => new FailureOperationResult<TResult>(statusCode);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new Task<IOperationResult<TResult>> FailureOperationAsync(HttpStatusCode statusCode)
            => Task.FromResult<IOperationResult<TResult>>(new FailureOperationResult<TResult>(statusCode));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> FailureOperation(params OperationError[] errors)
            => new FailureOperationResult<TResult>(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new Task<IOperationResult<TResult>> FailureOperationAsync(params OperationError[] errors)
            => Task.FromResult<IOperationResult<TResult>>(new FailureOperationResult<TResult>(errors));

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static new IOperationResult<TResult> FailureOperation(HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult<TResult>(statusCode, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A task that represents an object of <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new Task<IOperationResult<TResult>> FailureOperationAsync(HttpStatusCode statusCode, params OperationError[] errors)
            => Task.FromResult<IOperationResult<TResult>>(new FailureOperationResult<TResult>(statusCode, errors));
    }
}
