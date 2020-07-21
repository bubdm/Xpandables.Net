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

namespace Xpandables.Net.Retry
{
    /// <summary>
    /// Provides with the retry behavior current execution context.
    /// </summary>
    public sealed class RetryContext : IRetryContext
    {
        private Exception _exception;
        private TimeSpan _timeSpan;
        private int _retryCount;
        private bool? _retryFailed;

        Exception IRetryContext.Exception { get => _exception; set => _exception = value; }
        TimeSpan IRetryContext.TimeInterval { get => _timeSpan; set => _timeSpan = value; }
        int IRetryContext.RetryCount { get => _retryCount; set => _retryCount = value; }
        bool? IRetryContext.RetryFailed { get => _retryFailed; set => _retryFailed = value; }

        /// <summary>
        /// Initializes a new instance of <see cref="RetryContext"/> class with context information.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="retryCount"/> must be greater that zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeInterval"/> must be greater or equal to zero.</exception>
        internal RetryContext(Exception exception, TimeSpan timeInterval, int retryCount)
        {
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
            _timeSpan = timeInterval.TotalSeconds >= 0 ? timeInterval : throw new ArgumentOutOfRangeException(nameof(timeInterval));
            _retryCount = retryCount > 0 ? retryCount : throw new ArgumentOutOfRangeException(nameof(retryCount));
        }
    }
}
