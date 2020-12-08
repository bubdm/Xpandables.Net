
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
        Failed
    }

    /// <summary>
    /// Represents the status of an operation. The result contains <see cref="IsSuccess"/> and <see cref="IsFailed"/> which determines operation exit state,
    /// <see cref="GetStatusCode"/> that returns the HTTP status code and <see cref="GetErrors"/> which shows errors for failing operation execution.
    /// </summary>
    public interface IOperationResult
    {
        internal OperationStatus Status { get; }
        internal IReadOnlyCollection<OperationError> Errors { get; }
        internal HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Returns the operation HTTP status code.
        /// </summary>
        public HttpStatusCode GetStatusCode() => StatusCode;

        /// <summary>
        /// Returns a value that indicates whether the operation is completed successfully and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the operation is completed successfully; otherwise, <see langword="false"/>.</returns>
        public bool IsSuccess() => Status == OperationStatus.Success;

        /// <summary>
        /// Returns a value that indicates whether the operation is failed and returns <see langword="false"/> if so, otherwise <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="false"/> if the operation is failed; otherwise, <see langword="true"/>.</returns>
        public bool IsFailed() => Status == OperationStatus.Failed;

        /// <summary>
        /// Returns the collection of errors.
        /// </summary>
        public IReadOnlyCollection<OperationError> GetErrors() => Errors;
    }

    /// <summary>
    /// Represents the status of an operation that contains a return value of <typeparamref name="TValue"/> type.
    /// The result contains <see cref="IOperationResult.IsSuccess"/> and <see cref="IOperationResult.IsFailed"/> which determines operation exit state,
    /// <see cref="IOperationResult.GetStatusCode"/> that returns the HTTP status code and <see cref="IOperationResult.GetErrors"/> which shows errors for failing operation execution.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    public interface IOperationResult<out TValue> : IOperationResult
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        TValue Value { get; }
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

        OperationStatus IOperationResult.Status => _status;
        IReadOnlyCollection<OperationError> IOperationResult.Errors => _errors;
        HttpStatusCode IOperationResult.StatusCode => _statusCode;

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
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, IList<OperationError> errors) : this(status, statusCode) => _errors = errors.ToList() ?? throw new ArgumentNullException(nameof(errors));

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status and specified error.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="error"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, OperationError error) : this(status, statusCode, new[] { error ?? throw new ArgumentNullException(nameof(error)) }) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status, the key and specified error messages.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, string key, params string[] errorMessages) : this(status, statusCode, new OperationError(key, errorMessages)) { }
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
        protected OperationResult(OperationStatus state, HttpStatusCode statusCode, IList<OperationError> errors, TValue value) : base(state, statusCode, errors) => Value = value;

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
        protected OperationResult(OperationStatus status, HttpStatusCode statusCode, TValue value, string key, params string[] errorMessages) : this(status, statusCode, new OperationError(key, errorMessages), value) { }
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
