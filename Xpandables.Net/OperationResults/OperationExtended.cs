
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
using System.Net;

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with <see cref="IOperationResult"/> extension methods.
    /// </summary>
    public abstract class OperationExtended
    {
        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult OkOperation() => new SuccessOperationResult();

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
        protected static IOperationResult OkOperation(HttpStatusCode statusCode) => new SuccessOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult BadOperation() => new FailureOperationResult();

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult NotFoundOperation() => new FailureOperationResult(HttpStatusCode.NotFound);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult NotFoundOperation(params OperationError[] errors)
            => new FailureOperationResult(HttpStatusCode.NotFound, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult NotFoundOperation(string key, params string[] errorMessages)
            => new FailureOperationResult(HttpStatusCode.NotFound, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult UnauthorizedOperation() => new FailureOperationResult(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult UnauthorizedOperation(params OperationError[] errors)
            => new FailureOperationResult(HttpStatusCode.Unauthorized, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult UnauthorizedOperation(string key, params string[] errorMessages)
            => new FailureOperationResult(HttpStatusCode.Unauthorized, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult InternalErrorOperation() => new FailureOperationResult(HttpStatusCode.InternalServerError);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult InternalErrorOperation(params OperationError[] errors)
            => new FailureOperationResult(HttpStatusCode.InternalServerError, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult InternalErrorOperation(string key, params string[] errorMessages)
            => new FailureOperationResult(HttpStatusCode.InternalServerError, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult FailureOperation(HttpStatusCode statusCode)
            => new FailureOperationResult(statusCode);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult BadOperation(params OperationError[] errors)
            => new FailureOperationResult(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult BadOperation(string key, params string[] errorMessages)
            => new FailureOperationResult(key, errorMessages);

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
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static IOperationResult FailureOperation(HttpStatusCode statusCode, string key, params string[] errorMessages)
            => new FailureOperationResult(statusCode, key, errorMessages);
    }

    /// <summary>
    /// Provides with <see cref="IOperationResult{TValue}"/> extension methods.
    /// </summary>
    public abstract class OperationExtended<TResult> : OperationExtended
    {
        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
        /// </summary>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> OkOperation(TResult result) => new SuccessOperationResult<TResult>(result);

        /// <summary>
        /// Returns a <see cref="SuccessOperationResult{TValue}"/> with the specified status code and result.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The command result.</param>
        /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
        protected static IOperationResult<TResult> OkOperation(HttpStatusCode statusCode, TResult result)
            => new SuccessOperationResult<TResult>(statusCode, result);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> BadOperation() => new FailureOperationResult<TResult>();

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> NotFoundOperation() => new FailureOperationResult<TResult>(HttpStatusCode.NotFound);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> NotFoundOperation(params OperationError[] errors)
            => new FailureOperationResult<TResult>(HttpStatusCode.NotFound, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> NotFoundOperation(string key, params string[] errorMessages)
            => new FailureOperationResult<TResult>(HttpStatusCode.NotFound, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> UnauthorizedOperation() => new FailureOperationResult<TResult>(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> UnauthorizedOperation(params OperationError[] errors)
            => new FailureOperationResult<TResult>(HttpStatusCode.Unauthorized, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> UnauthorizedOperation(string key, params string[] errorMessages)
            => new FailureOperationResult<TResult>(HttpStatusCode.Unauthorized, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> InternalErrorOperation() => new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> InternalErrorOperation(params OperationError[] errors)
            => new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> InternalErrorOperation(string key, params string[] errorMessages)
            => new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, key, errorMessages);

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
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> FailureOperation(HttpStatusCode statusCode, string key, params string[] errorMessages)
            => new FailureOperationResult<TResult>(statusCode, key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> BadOperation(params OperationError[] errors)
            => new FailureOperationResult<TResult>(errors);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
        protected static new IOperationResult<TResult> BadOperation(string key, params string[] errorMessages)
            => new FailureOperationResult<TResult>(key, errorMessages);

        /// <summary>
        /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and errors.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A <see cref="FailureOperationResult"/>.</returns>
        protected static new IOperationResult<TResult> FailureOperation(HttpStatusCode statusCode, params OperationError[] errors)
            => new FailureOperationResult<TResult>(statusCode, errors);
    }
}
