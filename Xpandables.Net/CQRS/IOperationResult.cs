
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// An error that occurred during operation.
    /// </summary>
    public sealed class OperationError
    {
        /// <summary>
        /// Gets the key associated with the error.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the error messages associated with this error.
        /// </summary>
        public string[] ErrorMessages { get; internal set; }

        /// <summary>
        /// Gets the Exception associated with this error.
        /// </summary>
        public Exception? Exception { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationError"/> class with the specified key and specified exception.
        /// </summary>
        /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
        /// <param name="exception">The associated exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public OperationError(string key, Exception exception)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ErrorMessages = new[] { exception.Message };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationError"/> with the specified key and specified errorMessages.
        /// </summary>
        /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
        /// <param name="errorMessages">The associated error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public OperationError(string key, string[] errorMessages)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ErrorMessages = errorMessages ?? throw new ArgumentNullException(nameof(errorMessages));
        }
    }

    /// <summary>
    /// A collection of <see cref="OperationError"/> instances.
    /// </summary>
    public sealed class OperationErrorCollection : Collection<OperationError>
    {
        /// <summary>
        /// Returns the <see cref="OperationError"/> matching the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find out.</param>
        /// <returns>An instance of <see cref="OperationError"/> if found, otherwise null.</returns>
        public OperationError? this[string key] => this.FirstOrDefault(error => error.Key == key);

        /// <summary>
        /// Merges two collection of errors.
        /// </summary>
        /// <param name="errors">the other collection of errors to merge to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public void Merge(IReadOnlyCollection<OperationError> errors)
        {
            _ = errors ?? throw new ArgumentNullException(nameof(errors));
            foreach (var error in errors)
            {
                if (this[error.Key] is OperationError resultError)
                {
                    resultError.ErrorMessages = resultError.ErrorMessages.Union(error.ErrorMessages).ToArray();
                    resultError.Exception = error.Exception is not null && resultError.Exception is not null
                        ? new AggregateException(resultError.Exception, error.Exception)
                        : error.Exception ?? resultError.Exception;
                }
                else
                {
                    Add(error);
                }
            }
        }

        /// <summary>
        /// Adds the specified key with the exception.
        /// </summary>
        /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
        /// <param name="exception">The associated exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public void Add(string key, Exception exception)
        {
            var resultError = new OperationError(key, exception);
            Add(resultError);
        }

        /// <summary>
        /// Adds the specified key with the error messages.
        /// </summary>
        /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
        /// <param name="errorMessages">The associated error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public void Add(string key, string[] errorMessages)
        {
            var resultError = new OperationError(key, errorMessages);
            Add(resultError);
        }

        /// <summary>
        /// Initializes a default instance of <see cref="OperationErrorCollection"/>.
        /// </summary>
        public OperationErrorCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationErrorCollection"/> class as a wrapper for the specified list.
        /// </summary>
        /// <param name="errors">The list or errors that is wrapped by the new collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public OperationErrorCollection(IList<OperationError> errors) : base(errors) { }

        /// <summary>
        /// Returns the error collection as a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
        public IDictionary<string, string[]> ToDictionary() => this.ToDictionary(d => d.Key, d => d.ErrorMessages);
    }

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
    /// Represents the status of an operation. The result contains <see cref="IsSuccess"/> and <see cref="IsFailed"/> which determines operation exit state 
    /// and <see cref="Errors"/> which shows errors for failing operation execution.
    /// </summary>
    public interface IOperationResult
    {
        internal OperationStatus Status { get; }

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
        /// Gets the collection of errors of the result state.
        /// </summary>
        IReadOnlyCollection<OperationError> Errors { get; }
    }

    /// <summary>
    /// Represents the status of an operation that contains a return value of <typeparamref name="TValue"/> type.
    /// The result contains <see langword="IsSuccess"/> and <see langword="IsFailed"/> which determines operation exit state and <see langword="Errors"/> which shows errors for failing operation execution.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    public interface IOperationResult<TValue> : IOperationResult
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        TValue Value { get; }
    }

    /// <summary>
    /// The <see cref="OperationResult"/> represents the status of an operation and implement the <see cref="IOperationResult"/> interface.
    /// </summary>
    public abstract class OperationResult : IOperationResult
    {
        /// <summary>
        /// Contains the state of the result.
        /// </summary>
        protected readonly OperationStatus Status;
        OperationStatus IOperationResult.Status => Status;

        /// <summary>
        /// Gets the collection of errors when the operation has failed.
        /// </summary>
        public IReadOnlyCollection<OperationError> Errors { get; } = new OperationErrorCollection();

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status.
        /// </summary>
        /// <param name="status">The operation status.</param>
        protected OperationResult(OperationStatus status) => Status = status;

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult"/> with the specified status and specified error collection.
        /// </summary>
        /// <param name="status">The operation status.</param>
        /// <param name="errors">The error collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        protected OperationResult(OperationStatus status, IList<OperationError> errors) : this(status) => Errors = errors.ToList() ?? throw new ArgumentNullException(nameof(errors));
    }

    /// <summary>
    /// The <see cref="OperationResult{TValue}"/> represents the status of an operation and implement the <see cref="IOperationResult{TValue}"/> interface with a value.
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
        /// <param name="state">The status of the operation result.</param>
        /// <param name="value">The value of the specific type.</param>
        protected OperationResult(OperationStatus state, TValue value) : base(state) => Value = value;

        /// <summary>
        /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the specified error collection and the target value.
        /// </summary>
        /// <param name="state">The status of the operation result.</param>
        /// <param name="errors">The error collection.</param>
        /// <param name="value">The value of the specific type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        protected OperationResult(OperationStatus state, IList<OperationError> errors, TValue value) : base(state, errors) => Value = value;
    }

    /// <summary>
    /// An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Success"/> status.
    /// </summary>
    public class SuccessOperationResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult"/> class with <see cref="OperationStatus.Success"/> status.
        /// </summary>
        public SuccessOperationResult() : base(OperationStatus.Success) { }
    }

    /// <summary>
    /// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Success"/> status with a value of generic type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class SuccessOperationResult<TValue> : OperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult{TValue}"/> class with <see cref="OperationStatus.Success"/> status and the content value.
        /// </summary>
        /// <param name="value">The operation value.</param>
        public SuccessOperationResult(TValue value) : base(OperationStatus.Success, value) { }
    }

    /// <summary>
    ///  An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Failed"/> status.
    /// </summary>
    public class FailedOperationResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status.
        /// </summary>
        public FailedOperationResult() : base(OperationStatus.Failed) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status and the error collection.
        /// </summary>
        /// <param name="errors">The error collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailedOperationResult(IList<OperationError> errors) : base(OperationStatus.Failed, errors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status and the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailedOperationResult(OperationError error) : this(new[] { error ?? throw new ArgumentNullException(nameof(error)) }) { }
    }

    /// <summary>
    /// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Failed"/> status of generic type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class FailedOperationResult<TValue> : OperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status.
        /// </summary>
        public FailedOperationResult() : base(OperationStatus.Failed, default!) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status and the error collection.
        /// </summary>
        /// <param name="errors">The error collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailedOperationResult(IList<OperationError> errors) : base(OperationStatus.Failed, errors, default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status and the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailedOperationResult(OperationError error) : this(new[] { error ?? throw new ArgumentNullException(nameof(error)) }) { }
    }
}
