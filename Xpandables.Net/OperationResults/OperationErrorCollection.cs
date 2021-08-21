
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
using System.Collections.ObjectModel;

namespace Xpandables.Net;

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
        ErrorMessages = new[] { $"{exception}" };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationError"/> with the specified key and specified errorMessages.
    /// </summary>
    /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
    /// <param name="errorMessages">The associated error messages.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
    public OperationError(string key, params string[] errorMessages)
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
    public void Merge(OperationErrorCollection errors)
    {
        _ = errors ?? throw new ArgumentNullException(nameof(errors));
        foreach (var error in errors)
        {
            if (this[error.Key] is { } resultError)
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
        _ = key ?? throw new ArgumentNullException(nameof(key));
        _ = exception ?? throw new ArgumentNullException(nameof(exception));

        if (this[key] is { } error)
        {
            error.Exception = error.Exception is not null ? new AggregateException(error.Exception, exception) : exception;
        }
        else
        {
            var resultError = new OperationError(key, exception);
            Add(resultError);
        }
    }

    /// <summary>
    /// Adds the specified key with the error messages.
    /// </summary>
    /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
    /// <param name="errorMessages">The associated error messages.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
    public void Add(string key, params string[] errorMessages)
    {
        _ = key ?? throw new ArgumentNullException(nameof(key));
        _ = errorMessages ?? throw new ArgumentNullException(nameof(errorMessages));

        if (this[key] is { } error)
        {
            error.ErrorMessages = error.ErrorMessages.Union(errorMessages).ToArray();
        }
        else
        {
            var resultError = new OperationError(key, errorMessages);
            Add(resultError);
        }
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
    /// Initializes a new instance of the <see cref="OperationErrorCollection"/> class as a wrapper for the specified error.
    /// </summary>
    /// <param name="error">The list or errors that is wrapped by the new collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="error"/> is null.</exception>
    public OperationErrorCollection(OperationError error) : this(new[] { error }) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationErrorCollection"/> class as a wrapper for the specified error.
    /// </summary>
    /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
    /// <param name="errorMessages">The associated error messages.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
    public OperationErrorCollection(string key, params string[] errorMessages) : this(new OperationError(key, errorMessages)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationErrorCollection"/> class as a wrapper for the specified error.
    /// </summary>
    /// <param name="key">The key of <see cref="OperationError"/> to add errors to.</param>
    /// <param name="exception">The associated exception.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
    public OperationErrorCollection(string key, Exception exception) : this(new OperationError(key, exception)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationErrorCollection"/> class as a wrapper for the specified array.
    /// </summary>
    /// <param name="errors">the array of errors that is wrapped by the new collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    public OperationErrorCollection(params OperationError[] errors) : base(errors) { }
}
