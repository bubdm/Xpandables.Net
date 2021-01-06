
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
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Determines the status of the operation result.
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// The result is a success.
        /// </summary>
        Success,

        /// <summary>
        /// The result is failed.
        /// </summary>
        Failure
    }

    /// <summary>
    /// Represents the status of an operation. The result contains <see cref="IsSuccess"/> and <see cref="IsFailure"/> which determines operation exit state,
    /// <see cref="StatusCode"/> that returns the HTTP status code and <see cref="Errors"/> which shows errors for failing operation execution.
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// Gets the operation result status.
        /// </summary>
        OperationStatus Status { get; }

        /// <summary>
        /// Gets the collection of errors.
        /// </summary>
        IReadOnlyCollection<OperationError> Errors { get; }

        /// <summary>
        /// Gets the operation HTTP status code.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets a value that indicates whether the operation is completed successfully and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the operation is completed successfully; otherwise, <see langword="false"/>.</returns>
        public sealed bool IsSuccess => Status == OperationStatus.Success;

        /// <summary>
        /// Gets a value that indicates whether the operation is failed and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the operation is failed; otherwise, <see langword="true"/>.</returns>
        public sealed bool IsFailure => Status == OperationStatus.Failure;

        /// <summary>
        /// Converts the current success operation instance to the generic success operation with the specified value.
        /// </summary>
        /// /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>A new instance of <see cref="SuccessOperationResult{TValue}"/> with the status code of the current success operation and the specified value.</returns>
        public virtual SuccessOperationResult<TValue> ToSuccessOperationResult<TValue>(TValue value) => new(StatusCode, value);

        /// <summary>
        /// Converts the current failed operation instance to the -generic failed operation.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>A new instance of <see cref="FailureOperationResult{TValue}"/> with the status code and errors from the failed operation.</returns>
        public virtual FailureOperationResult<TValue> ToFailedOperationResult<TValue>() => new(StatusCode, Errors);
    }

    /// <summary>
    /// Represents the status of an operation that contains a return value of <typeparamref name="TValue"/> type.
    /// The result contains <see cref="IOperationResult.IsSuccess"/> and <see cref="IOperationResult.IsFailure"/> which determines operation exit state,
    /// <see cref="IOperationResult.StatusCode"/> that returns the HTTP status code and <see cref="IOperationResult.Errors"/> which shows errors for failing operation execution.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    public interface IOperationResult<out TValue> : IOperationResult
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Converts the current generic success operation instance to the non-generic success operation.
        /// </summary>
        /// <returns>A new instance of <see cref="SuccessOperationResult"/> with the status code of the current success operation.</returns>
        public virtual SuccessOperationResult ToSuccessOperationResult() => new(StatusCode);

        /// <summary>
        /// Converts the current generic failed operation instance to the non-generic failed operation.
        /// </summary>
        /// <returns>A new instance of <see cref="FailureOperationResult"/> with the status code and errors from the generic failed operation.</returns>
        public virtual FailureOperationResult ConvertToFailedOperationResult() => new(StatusCode, Errors);
    }

    /// <summary>
    /// The <see cref="OperationResult"/> represents the status of an operation and implements the <see cref="IOperationResult"/> interface.
    /// </summary>
    public abstract class OperationResult : IOperationResult
    {
        /// <summary>
        /// Contains the state of the result.
        /// </summary>
        protected readonly OperationStatus _status;

        /// <summary>
        /// Contains the HTTP status code.
        /// </summary>
        protected readonly HttpStatusCode _statusCode = HttpStatusCode.OK;

        /// <summary>
        /// Contains the errors collection.
        /// </summary>
        protected readonly IReadOnlyCollection<OperationError> _errors = new OperationErrorCollection();

        /// <summary>
        /// Gets the operation result status.
        /// </summary>
        public OperationStatus Status => _status;

        /// <summary>
        /// Gets the collection of errors.
        /// </summary>
        public IReadOnlyCollection<OperationError> Errors => _errors;

        /// <summary>
        /// Gets the operation HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode => _statusCode;

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode) => (_status, _statusCode) = (status, statusCode);

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status and specified errors collection.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, IReadOnlyCollection<OperationError> errors)
            : this(status, statusCode) => _errors = errors ?? throw new ArgumentNullException(nameof(errors));

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status and specified error.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="error"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, OperationError error)
            : this(status, statusCode, new[] { error ?? throw new ArgumentNullException(nameof(error)) }) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status, the key and specified error messages.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, string key, params string[] errorMessages)
            : this(status, statusCode, new OperationError(key, errorMessages)) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status, the key and specified exception.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, string key, Exception exception)
            : this(status, statusCode, new OperationError(key, exception)) { }
    }

    /// <summary>
    /// The <see cref="OperationResult{TValue}"/> represents the status of an operation and implements the <see cref="IOperationResult{TValue}"/> interface with a value.
    /// </summary>
    /// <typeparam name="TValue">the type of the value.</typeparam>
    public abstract class OperationResult<TValue> : OperationResult, IOperationResult<TValue>
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status and the target value.
        /// </summary>
        /// <param name="status">The status of the operation result.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="value">The value of the specific type.</param>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, TValue value) : base(status, statusCode) => Value = value;

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the specified error collection and the target value.
        /// </summary>
        /// <param name="state">The status of the operation result.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="errors">The errors collection.</param>
        /// <param name="value">The value of the specific type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        protected OperationResult(OperationStatus state, HttpStatusCode statusCode, IReadOnlyCollection<OperationError> errors, TValue value)
            : base(state, statusCode, errors) => Value = value;

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the specified error and the target value.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <param name="value">The value of the specific type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="error"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, OperationError error, TValue value)
            : this(status, statusCode, new[] { error ?? throw new ArgumentNullException(nameof(error)) }, value) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the value, the key and specified error messages.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="value">The value of the specific type.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, TValue value, string key, params string[] errorMessages)
            : this(status, statusCode, new OperationError(key, errorMessages), value) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the value, the key and specified exception.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="value">The value of the specific type.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, TValue value, string key, Exception exception)
            : this(status, statusCode, new OperationError(key, exception), value) { }
    }

    /// <summary>
    /// <see cref="IOperationResult"/> extensions.
    /// </summary>
    public static class IOPerationResultExtensions
    {
        /// <summary>
        /// Converts the error collection as a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
        public static IDictionary<string, string[]> ToDictionary(this IReadOnlyCollection<OperationError> @this) => @this.ToDictionary(d => d.Key, d => d.ErrorMessages);
    }
}
