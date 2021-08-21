
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
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Xpandables.Net;

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
/// Represents the status of an operation. The result contains <see cref="IsSucceeded"/> and <see cref="IsFailed"/> which determines operation exit state,
/// <see cref="StatusCode"/> that returns the HTTP status code and <see cref="Errors"/> which shows errors for failing operation execution.
/// </summary>
public interface IOperationResult
{
    /// <summary>
    /// Gets a object that qualifies or contains information about an operation return.
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// Gets the operation result status.
    /// </summary>
    OperationStatus Status { get; }

    /// <summary>
    /// Gets the collection of errors.
    /// </summary>
    OperationErrorCollection Errors { get; }

    /// <summary>
    /// Gets the operation HTTP status code.
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the URL for location header.
    /// Mostly used with <see cref="HttpStatusCode.Created"/>.
    /// </summary>
    string? LocationUrl { get; }

    /// <summary>
    /// Determines whether or not the <see cref="Value"/> has a content.
    /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// </summary>
    public bool HasValue => Value is not null;

    /// <summary>
    /// Determines whether or not the current instance is generic.
    /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// </summary>
    public bool IsGeneric => false;

    /// <summary>
    /// Gets a value that indicates whether the operation is completed successfully and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the operation is completed successfully; otherwise, <see langword="false"/>.</returns>
    public sealed bool IsSucceeded => Status == OperationStatus.Success;

    /// <summary>
    /// Gets a value that indicates whether the operation is failed and returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the operation is failed; otherwise, <see langword="true"/>.</returns>
    public sealed bool IsFailed => Status == OperationStatus.Failure;

    /// <summary>
    /// Defines the <see cref="LocationUrl"/> value.
    /// </summary>
    /// <param name="url">The location URL value.</param>
    /// <returns>The current instance.</returns>
    IOperationResult AddLocationUrl([Url] string url);

    /// <summary>
    /// Converts the current success operation instance to the generic success operation with the specified value.
    /// </summary>
    /// /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>A new instance of <see cref="SuccessOperationResult{TValue}"/> with the status code of the current success operation and the specified value.</returns>
    public virtual SuccessOperationResult<TValue> ToSuccessOperationResult<TValue>(TValue value) => new(StatusCode, value);

    /// <summary>
    /// Converts the current failed operation instance to the generic failed operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>A new instance of <see cref="FailureOperationResult{TValue}"/> with the status code and errors from the failed operation.</returns>
    public virtual FailureOperationResult<TValue> ToFailureOperationResult<TValue>() => new(StatusCode, Errors);
}

/// <summary>
/// Represents the status of an operation that contains a return value of <typeparamref name="TValue"/> type.
/// The result contains <see cref="IOperationResult.IsSucceeded"/> and <see cref="IOperationResult.IsFailed"/> which determines operation exit state,
/// <see cref="IOperationResult.StatusCode"/> that returns the HTTP status code and <see cref="IOperationResult.Errors"/> which shows errors for failing operation execution.
/// </summary>
/// <typeparam name="TValue">The type of the return value.</typeparam>
public interface IOperationResult<out TValue> : IOperationResult
{
    /// <summary>
    /// Gets a user-defined object that qualifies or contains information about an operation return.
    /// </summary>
    new TValue Value { get; }

    object? IOperationResult.Value => Value;

    /// <summary>
    /// Determines whether or not the current instance is generic.
    /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
    /// </summary>
    public new bool IsGeneric => true;

    bool IOperationResult.IsGeneric => IsGeneric;

    /// <summary>
    /// Defines the <see cref="IOperationResult.LocationUrl"/> value.
    /// </summary>
    /// <param name="url">The location URL value.</param>
    /// <returns>The current instance.</returns>
    new IOperationResult<TValue> AddLocationUrl([Url] string url);

    /// <summary>
    /// Converts the current generic success operation instance to the non-generic success operation.
    /// </summary>
    /// <returns>A new instance of <see cref="SuccessOperationResult"/> with the status code of the current success operation.</returns>
    public virtual SuccessOperationResult ToSuccessOperationResult() => new(StatusCode);

    /// <summary>
    /// Converts the current generic failed operation instance to the non-generic failed operation.
    /// </summary>
    /// <returns>A new instance of <see cref="FailureOperationResult"/> with the status code and errors from the generic failed operation.</returns>
    public virtual FailureOperationResult ToFailureOperationResult() => new(StatusCode, Errors);
}

/// <summary>
/// The <see cref="OperationResult"/> represents the status of an operation and implements the <see cref="IOperationResult"/> interface.
/// </summary>
public abstract class OperationResult : IOperationResult
{
    ///<inheritdoc/>
    public object Value { get; }

    /// <inheritdoc/>
    public string? LocationUrl { get; protected set; }

    /// <inheritdoc/>
    public OperationStatus Status { get; }

    /// <inheritdoc/>
    public OperationErrorCollection Errors { get; } = new();

    /// <inheritdoc/>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="OperationResult"/> with the specified status and specified errors collection.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <param name="statusCode">The HTTP operation status code.</param>
    /// <param name="errors">The errors collection.</param>
    /// <param name="value">The value of the result.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    protected OperationResult(OperationStatus status, HttpStatusCode statusCode, OperationErrorCollection errors, object value)
        => (Status, StatusCode, Value, Errors) = (status, statusCode, value, errors ?? throw new ArgumentNullException(nameof(errors)));

    /// <inheritdoc/>
    public IOperationResult AddLocationUrl([Url] string url)
    {
        _ = url ?? throw new ArgumentNullException(nameof(url));
        LocationUrl = url;
        return this;
    }
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
    public new TValue Value { get; }

    /// <inheritdoc/>
    public new IOperationResult<TValue> AddLocationUrl([Url] string url)
    {
        _ = url ?? throw new ArgumentNullException(nameof(url));
        LocationUrl = url;
        return this;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperationResult{TValue}"/> with the specified status, the specified error collection and the target value.
    /// </summary>
    /// <param name="state">The status of the operation result.</param>
    /// <param name="statusCode">The HTTP operation status code.</param>
    /// <param name="errors">The errors collection.</param>
    /// <param name="value">The value of the specific type.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    protected OperationResult(OperationStatus state, HttpStatusCode statusCode, OperationErrorCollection errors, TValue value)
        : base(state, statusCode, errors, value!) => Value = value;
}
