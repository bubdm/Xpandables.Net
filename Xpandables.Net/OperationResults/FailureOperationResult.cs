
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
///  An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Failure"/> status.
/// </summary>
public class FailureOperationResult : OperationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
    /// <see cref="OperationStatus.Failure"/> status and <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    public FailureOperationResult()
        : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new OperationErrorCollection(), default!) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with
    /// <see cref="OperationStatus.Failure"/> status and the specified status code.
    /// </summary>
    /// <param name="statusCode">The HTTP operation status code.</param>
    public FailureOperationResult(HttpStatusCode statusCode)
        : base(OperationStatus.Failure, statusCode, new OperationErrorCollection(), default!) { }

    /// <summary>
    /// Initializes a new instance of th <see cref="FailureOperationResult"/> class with 
    /// <see cref="OperationStatus.Failure"/> status and the errors collection.
    /// </summary>
    /// <param name="statusCode">The HTTP operation status code.</param>
    /// <param name="errors">The errors collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    public FailureOperationResult(HttpStatusCode statusCode, OperationErrorCollection errors)
        : base(OperationStatus.Failure, statusCode, errors, default!) { }

    /// <summary>
    /// Initializes a new instance of th <see cref="FailureOperationResult"/> class with 
    /// <see cref="OperationStatus.Failure"/> status, the errors collection and <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    /// <param name="errors">The errors collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    public FailureOperationResult(OperationErrorCollection errors)
        : base(OperationStatus.Failure, HttpStatusCode.BadRequest, errors, default!) { }
}

/// <summary>
/// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Failure"/> 
/// status of generic type.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class FailureOperationResult<TValue> : OperationResult<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with
    /// <see cref="OperationStatus.Failure"/> status and <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    public FailureOperationResult()
        : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new OperationErrorCollection(), default!) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
    /// <see cref="OperationStatus.Failure"/> status and the specified status code.
    /// </summary>
    /// <param name="statusCode">The HTTP operation status code.</param>
    public FailureOperationResult(HttpStatusCode statusCode)
        : base(OperationStatus.Failure, statusCode, new OperationErrorCollection(), default!) { }

    /// <summary>
    /// Initializes a new instance of th <see cref="FailureOperationResult{TValue}"/> class with 
    /// <see cref="OperationStatus.Failure"/> status and the errors collection.
    /// </summary>
    /// <param name="statusCode">The HTTP operation status code.</param>
    /// <param name="errors">The errors collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    public FailureOperationResult(HttpStatusCode statusCode, OperationErrorCollection errors)
        : base(OperationStatus.Failure, statusCode, errors, default!) { }

    /// <summary>
    /// Initializes a new instance of th <see cref="FailureOperationResult{TValue}"/> class with 
    /// <see cref="OperationStatus.Failure"/> status,
    /// the errors collection and <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    /// <param name="errors">The errors collection.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
    public FailureOperationResult(OperationErrorCollection errors)
        : base(OperationStatus.Failure, HttpStatusCode.BadRequest, errors, default!) { }
}
