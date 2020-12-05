
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
    public sealed class ResultError
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
        /// Initializes a new instance of the <see cref="ResultError"/> class with the specified key and specified exception.
        /// </summary>
        /// <param name="key">The key of <see cref="ResultError"/> to add errors to.</param>
        /// <param name="exception">The associated exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public ResultError(string key, Exception exception)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ErrorMessages = new[] { exception.Message };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> with the specified key and specified errorMessages.
        /// </summary>
        /// <param name="key">The key of <see cref="ResultError"/> to add errors to.</param>
        /// <param name="errorMessages">The associated error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public ResultError(string key, string[] errorMessages)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ErrorMessages = errorMessages ?? throw new ArgumentNullException(nameof(errorMessages));
        }
    }

    /// <summary>
    /// A collection of <see cref="ResultError"/> instances.
    /// </summary>
    public sealed class ResultErrorCollection : Collection<ResultError>
    {
        /// <summary>
        /// Returns the <see cref="ResultError"/> matching the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find out.</param>
        /// <returns>An instance of <see cref="ResultError"/> if found, otherwise null.</returns>
        public ResultError? this[string key] => this.FirstOrDefault(error => error.Key == key);

        /// <summary>
        /// Merges two collection of errors.
        /// </summary>
        /// <param name="errors">the other collection of errors to merge to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public void Merge(IReadOnlyCollection<ResultError> errors)
        {
            _ = errors ?? throw new ArgumentNullException(nameof(errors));
            foreach (var error in errors)
            {
                if (this[error.Key] is ResultError resultError)
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
        /// <param name="key">The key of <see cref="ResultError"/> to add errors to.</param>
        /// <param name="exception">The associated exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public void Add(string key, Exception exception)
        {
            var resultError = new ResultError(key, exception);
            Add(resultError);
        }

        /// <summary>
        /// Adds the specified key with the error messages.
        /// </summary>
        /// <param name="key">The key of <see cref="ResultError"/> to add errors to.</param>
        /// <param name="errorMessages">The associated error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public void Add(string key, string[] errorMessages)
        {
            var resultError = new ResultError(key, errorMessages);
            Add(resultError);
        }

        /// <summary>
        /// Initializes a default instance of <see cref="ResultErrorCollection"/>.
        /// </summary>
        public ResultErrorCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultErrorCollection"/> class as a wrapper for the specified list.
        /// </summary>
        /// <param name="errors">The list or errors that is wrapped by the new collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public ResultErrorCollection(IList<ResultError> errors) : base(errors) { }
    }

    /// <summary>
    /// Represents the status of an operation. The result contains <see cref="IsSuccess"/> and <see cref="IsFailed"/> which determines operation exit state 
    /// and <see cref="Errors"/> which shows errors for failing operation execution.
    /// </summary>
    public interface IResultState
    {
        /// <summary>
        /// Returns a value that indicates whether the operation is completed successfully and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the operation is completed successfully; otherwise, <see langword="false"/>.</returns>
        public bool IsSuccess() => Errors.Count <= 0;

        /// <summary>
        /// Returns a value that indicates whether the operation is failed and returns <see langword="false"/> if so, otherwise <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="false"/> if the operation is failed; otherwise, <see langword="true"/>.</returns>
        public bool IsFailed() => Errors.Count > 0;

        /// <summary>
        /// Gets the collection of errors of the result state.
        /// </summary>
        IReadOnlyCollection<ResultError> Errors { get; }
    }

    /// <summary>
    /// Represents the status of an operation that contains a return value of <typeparamref name="TValue"/> type.
    /// The result contains <see langword="IsSuccess"/> and <see langword="IsFailed"/> which determines operation exit state and <see langword="Errors"/> which shows errors for failing operation execution.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    public interface IResultState<TValue> : IResultState
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        TValue Value { get; }
    }

    /// <summary>
    /// The <see cref="ResultState"/> represents the status of an operation and implement the <see cref="IResultState"/> interface.
    /// </summary>
    public class ResultState : IResultState
    {
        /// <summary>
        /// Gets the collection of errors when the operation has failed.
        /// </summary>
        public IReadOnlyCollection<ResultError> Errors { get; } = new ResultErrorCollection();

        /// <summary>
        /// Initializes a default instance of <see cref="ResultState"/>.
        /// </summary>
        internal ResultState() { }

        /// <summary>
        /// Initializes a new instance of <see cref="ResultState"/> with the specified error collection.
        /// </summary>
        /// <param name="errors">The error collection.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="errors"/> is null.</exception>
        internal ResultState(ResultErrorCollection errors) => Errors = errors ?? throw new ArgumentNullException(nameof(errors));

        /// <summary>
        /// Initializes a new instance of <see cref="ResultState"/> with the specified error list.
        /// </summary>
        /// <param name="errors">The error list.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="errors"/> is null.</exception>
        internal ResultState(IList<ResultError> errors) => Errors = new ResultErrorCollection(errors ?? throw new ArgumentNullException(nameof(errors)));

        /// <summary>
        /// Returns a success result state.
        /// </summary>
        /// <returns>A result state with no errors.</returns>
        public static ResultState Success() => new();

        /// <summary>
        /// Returns a failed result state with the specified error collection.
        /// </summary>
        /// <param name="errors">The error collection.</param>
        /// <returns>A result state with errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public static ResultState Failed(ResultErrorCollection errors) => new(errors);

        /// <summary>
        /// Returns a failed result state with the specified error list.
        /// </summary>
        /// <param name="errors">The error list.</param>
        /// <returns>A result state with errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public static ResultState Failed(IList<ResultError> errors) => new(errors);

        /// <summary>
        /// Returns a success result state with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The result value.</param>
        /// <returns>A success result state with value.</returns>
        public static ResultState<TValue> Success<TValue>(TValue value) => new(value);

        /// <summary>
        /// Returns a failed result state with the specified errors collection.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="errors">The error collection.</param>
        /// <returns>A result state with errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public static ResultState<TValue> Failed<TValue>(ResultErrorCollection errors) => new(errors);

        /// <summary>
        /// Returns a failed result state with the specified errors list.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="errors">The error list.</param>
        /// <returns>A result state with errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public static ResultState<TValue> Failed<TValue>(IList<ResultError> errors) => new(errors);
    }

    /// <summary>
    /// The <see cref="ResultState{TValue}"/> represents the status of an operation and implement the <see cref="IResultState{TValue}"/> interface.
    /// </summary>
    /// <typeparam name="TValue">the type of the value.</typeparam>
    public class ResultState<TValue> : ResultState, IResultState<TValue>
    {
        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an operation return.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Initializes a new success instance of <see cref="ResultState{TValue}"/> with the specified value.
        /// </summary>
        /// <param name="value">The value of the specific type.</param>
        internal ResultState(TValue value) => Value = value;

        /// <summary>
        /// Initializes a new failed instance of <see cref="ResultState{TValue}"/> with the specified error collection.
        /// </summary>
        /// <param name="errors">The error collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        internal ResultState(ResultErrorCollection errors) : base(errors) => Value = default!;

        /// <summary>
        /// Initializes a new failed instance of <see cref="ResultState{TValue}"/> with the specified error list.
        /// </summary>
        /// <param name="errors">The error list.</param>
        /// <returns>A result state with errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        internal ResultState(IList<ResultError> errors) : base(errors) => Value = default!;
    }
}
