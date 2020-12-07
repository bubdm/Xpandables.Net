
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    ///  An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Failed"/> status.
    /// </summary>
    public sealed class FailedOperationResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status.
        /// </summary>
        public FailedOperationResult() : base(OperationStatus.Failed) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status and the errors collection.
        /// </summary>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailedOperationResult(IList<OperationError> errors) : base(OperationStatus.Failed, errors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status and the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailedOperationResult(OperationError error) : base(OperationStatus.Failed, error) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult"/> class with <see cref="OperationStatus.Failed"/> status, the specified key and error messages.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailedOperationResult(string key, params string[] errorMessages) : base(OperationStatus.Failed, key, errorMessages) { }
    }

    /// <summary>
    /// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Failed"/> status of generic type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class FailedOperationResult<TValue> : OperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status.
        /// </summary>
        public FailedOperationResult() : base(OperationStatus.Failed, default!) { }

        /// <summary>
        /// Initializes a new instance of th <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status and the errors collection.
        /// </summary>
        /// <param name="errors">The errors collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        public FailedOperationResult(IList<OperationError> errors) : base(OperationStatus.Failed, errors, default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status and the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="error"/> is null.</exception>
        public FailedOperationResult(OperationError error) : base(OperationStatus.Failed, error, default!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOperationResult{TValue}"/> class with <see cref="OperationStatus.Failed"/> status, the specified key and the error messages.
        /// </summary>
        /// <param name="key">The key of the error.</param>
        /// <param name="errorMessages">The array of error messages.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorMessages"/> is null.</exception>
        public FailedOperationResult(string key, params string[] errorMessages) : base(OperationStatus.Failed, default!, key, errorMessages) { }
    }
}
