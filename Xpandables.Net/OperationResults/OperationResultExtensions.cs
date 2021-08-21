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

namespace Xpandables.Net;

/// <summary>
/// <see cref="IOperationResult"/> extensions.
/// </summary>
public static class OperationResultExtensions
{
    /// <summary>
    /// Converts the error collection as a <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
    public static IDictionary<string, string[]> ToDictionary(this IReadOnlyCollection<OperationError> @this) => @this.ToDictionary(d => d.Key, d => d.ErrorMessages);

    /// <summary>
    /// Converts the enumerable collection of errors to <see cref="OperationErrorCollection"/>.
    /// </summary>
    /// <param name="this">The source collection.</param>
    /// <returns>A <see cref="OperationErrorCollection"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
    public static OperationErrorCollection ToOperationCollection(this IEnumerable<OperationError> @this)
        => new(@this.ToArray());

    /// <summary>
    /// Converts the value to a success operation result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The target value to act on.</param>
    /// <returns>A new instance of <see cref="SuccessOperationResult{TValue}"/> that contains the value.</returns>
    public static SuccessOperationResult<TValue> ToSuccessOperationResult<TValue>(this TValue value)
        where TValue : notnull
        => new(value);

    /// <summary>
    /// Returns a <see cref="SuccessOperationResult"/> with <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
    public static IOperationResult OkOperation(this object _) => new SuccessOperationResult();

    /// <summary>
    /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/> and result.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="result">The command result.</param>
    /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> OkOperation<TResult>(this object _, TResult result) => new SuccessOperationResult<TResult>(result);

    /// <summary>
    /// Returns a <see cref="SuccessOperationResult{TValue}"/> with <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> OkOperation<TResult>(this object _) => new SuccessOperationResult<TResult>(default!);

    /// <summary>
    /// Returns a <see cref="SuccessOperationResult{TValue}"/> with the specified status code and result.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="result">The command result.</param>
    /// <returns>A <see cref="SuccessOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> OkOperation<TResult>(this object _, HttpStatusCode statusCode, TResult result) => new SuccessOperationResult<TResult>(statusCode, result);

    /// <summary>
    /// Returns a <see cref="SuccessOperationResult"/> with the specified status code.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A <see cref="SuccessOperationResult"/>.</returns>
    public static IOperationResult OkOperation(this object _, HttpStatusCode statusCode) => new SuccessOperationResult(statusCode);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult BadOperation(this object _) => new FailureOperationResult();

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with <see cref="HttpStatusCode.BadRequest"/> and errors.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> BadOperation<TResult>(this object _, OperationErrorCollection errors) => new FailureOperationResult<TResult>(errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> BadOperation<TResult>(this object _) => new FailureOperationResult<TResult>();

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with <see cref="HttpStatusCode.BadRequest"/> and specified errors.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult BadOperation(this object _, OperationErrorCollection errors) => new FailureOperationResult(errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
    /// </summary>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult NotFoundOperation(this object _) => new FailureOperationResult(HttpStatusCode.NotFound);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.NotFound"/> status code and errors.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult NotFoundOperation(this object _, OperationErrorCollection errors) => new FailureOperationResult(HttpStatusCode.NotFound, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.NotFound"/> status code.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> NotFoundOperation<TResult>(this object _) => new FailureOperationResult<TResult>(HttpStatusCode.NotFound);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.NotFound"/> status code and error messages.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> NotFoundOperation<TResult>(this object _, OperationErrorCollection errors) => new FailureOperationResult<TResult>(HttpStatusCode.NotFound, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
    /// </summary>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult UnauthorizedOperation(this object _) => new FailureOperationResult(HttpStatusCode.Unauthorized);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code and error messages.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult UnauthorizedOperation(this object _, OperationErrorCollection errors)
        => new FailureOperationResult(HttpStatusCode.Unauthorized, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> UnauthorizedOperation<TResult>(this object _) => new FailureOperationResult<TResult>(HttpStatusCode.Unauthorized);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.Unauthorized"/> status code and error messages.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> UnauthorizedOperation<TResult>(this object _, OperationErrorCollection errors) => new FailureOperationResult<TResult>(HttpStatusCode.Unauthorized, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
    /// </summary>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult InternalErrorOperation(this object _) => new FailureOperationResult(HttpStatusCode.InternalServerError);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code and error messages.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult InternalErrorOperation(this object _, OperationErrorCollection errors)
        => new FailureOperationResult(HttpStatusCode.InternalServerError, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> InternalErrorOperation<TResult>(this object _) => new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the <see cref="HttpStatusCode.InternalServerError"/> status code and error messages.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> InternalErrorOperation<TResult>(this object _, OperationErrorCollection errors) => new FailureOperationResult<TResult>(HttpStatusCode.InternalServerError, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the specified status code.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult FailureOperation(this object _, HttpStatusCode statusCode) => new FailureOperationResult(statusCode);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult"/> with the specified status code and error messages.
    /// </summary>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult FailureOperation(this object _, HttpStatusCode statusCode, OperationErrorCollection errors) => new FailureOperationResult(statusCode, errors);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A <see cref="FailureOperationResult{TValue}"/>.</returns>
    public static IOperationResult<TResult> FailureOperation<TResult>(this object _, HttpStatusCode statusCode) => new FailureOperationResult<TResult>(statusCode);

    /// <summary>
    /// Returns a <see cref="FailureOperationResult{TValue}"/> with the specified status code and error messages.
    /// </summary>
    /// <typeparam name="TResult">The type the result.</typeparam>
    /// <param name="_">The target object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <returns>A <see cref="FailureOperationResult"/>.</returns>
    public static IOperationResult<TResult> FailureOperation<TResult>(this object _, HttpStatusCode statusCode, OperationErrorCollection errors) => new FailureOperationResult<TResult>(statusCode, errors);
}
