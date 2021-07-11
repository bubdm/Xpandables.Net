
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

namespace Xpandables.Net
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status and the specified error.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, OperationError error)
            : base(OperationStatus.Failure, statusCode, new OperationErrorCollection(error), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified error and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(OperationError error)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new OperationErrorCollection(error), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and error messages.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, string key, params string[] errorMessages)
            : base(OperationStatus.Failure, statusCode, new(key, errorMessages), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and error messages 
        /// and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(string key, params string[] errorMessages)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, errorMessages), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and exception.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, string key, Exception exception)
            : base(OperationStatus.Failure, statusCode, new(key, exception), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and exception and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(string key, Exception exception)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, exception), default!) { }
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
        /// <see cref="OperationStatus.Failure"/> status and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="value">The operation value.</param>
        public FailureOperationResult(TValue value)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new OperationErrorCollection(), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status and the specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        public FailureOperationResult(HttpStatusCode statusCode)
            : base(OperationStatus.Failure, statusCode, new OperationErrorCollection(), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status and the specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="value">The operation value.</param>
        public FailureOperationResult(HttpStatusCode statusCode, TValue value)
            : base(OperationStatus.Failure, statusCode, new OperationErrorCollection(), value) { }

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
        /// Initializes a new instance of th <see cref="FailureOperationResult{TValue}"/> 
        /// class with <see cref="OperationStatus.Failure"/> status and the errors collection.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailureOperationResult(TValue value, HttpStatusCode statusCode, OperationErrorCollection errors)
            : base(OperationStatus.Failure, statusCode, errors, value) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status,
        /// the errors collection and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailureOperationResult(OperationErrorCollection errors)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, errors, default!) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailureOperationResult{TValue}"/> class 
        /// with <see cref="OperationStatus.Failure"/> status,
        /// the errors collection and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailureOperationResult(TValue value, OperationErrorCollection errors)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, errors, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status and the specified error.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, OperationError error)
            : base(OperationStatus.Failure, statusCode, new(error), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> 
        /// class with <see cref="OperationStatus.Failure"/> status and the specified error.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(TValue value, HttpStatusCode statusCode, OperationError error)
            : base(OperationStatus.Failure, statusCode, new(error), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class 
        /// with <see cref="OperationStatus.Failure"/> status, the specified error and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(OperationError error)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(error), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified error and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailureOperationResult(TValue value, OperationError error)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(error), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the error messages.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, string key, params string[] errorMessages)
            : base(OperationStatus.Failure, statusCode, new(key, errorMessages), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the error messages.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(TValue value, HttpStatusCode statusCode, string key, params string[] errorMessages)
            : base(OperationStatus.Failure, statusCode, new(key, errorMessages), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the error messages and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(string key, params string[] errorMessages)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, errorMessages), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the error messages and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailureOperationResult(TValue value, string key, params string[] errorMessages)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, errorMessages), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the exception.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(HttpStatusCode statusCode, string key, Exception exception)
            : base(OperationStatus.Failure, statusCode, new(key, exception), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the exception.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(TValue value, HttpStatusCode statusCode, string key, Exception exception)
            : base(OperationStatus.Failure, statusCode, new(key, exception), value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the exception and 
        /// <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(string key, Exception exception)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, exception), default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureOperationResult{TValue}"/> class with 
        /// <see cref="OperationStatus.Failure"/> status, the specified key and the exception and <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        /// <param name="value">The operation value.</param>
        /// <param name="key">The key of the error.</param>
        /// <param name="exception">The handled exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public FailureOperationResult(TValue value, string key, Exception exception)
            : base(OperationStatus.Failure, HttpStatusCode.BadRequest, new(key, exception), value) { }
    }
}
