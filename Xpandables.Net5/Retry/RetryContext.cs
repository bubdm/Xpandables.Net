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

namespace System
{
    /// <summary>
    /// Provides with the retry behavior current execution context.
    /// </summary>
    public sealed class RetryContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RetryContext"/> class with context information.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="retryCount"/> must be greater that zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeInterval"/> must be greater or equal to zero.</exception>
        public RetryContext(Exception exception, TimeSpan timeInterval, int retryCount)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            TimeInterval = timeInterval.WhenNotGreaterThan(TimeSpan.FromMilliseconds(0), nameof(timeInterval)).ThrowArgumentOutOfRangeException();
            RetryCount = retryCount.WhenNotGreaterThan(0, nameof(retryCount)).ThrowArgumentOutOfRangeException();
        }

        /// <summary>
        /// Gets the handled exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets the retry interval in milliseconds.
        /// </summary>
        public TimeSpan TimeInterval { get; private set; }

        /// <summary>
        /// Gets the retry number to be executed.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Determines whether the retry execution failed. Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        internal bool? RetryFailed { get; private set; }

        /// <summary>
        /// Updates the <see cref="TimeInterval"/> value.
        /// </summary>
        /// <param name="timeInterval">The new value.</param>
        /// <returns>The instance with the new value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="timeInterval"/> must be greater or equal to zero.</exception>
        public RetryContext UpdateTimeInterval(TimeSpan timeInterval)
        {
            TimeInterval = timeInterval.WhenNotGreaterThan(TimeSpan.FromMilliseconds(0), nameof(timeInterval)).ThrowArgumentOutOfRangeException();
            return this;
        }

        /// <summary>
        /// Updates the exception value.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        /// <returns>The instance with the new value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="exception"/> is null.</exception>
        internal RetryContext UpdateException(Exception exception)
        {
            Exception = exception ?? throw new ArgumentException(nameof(exception));
            return this;
        }

        /// <summary>
        /// Increases the retry count value.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal RetryContext IncreaseRetryCount()
        {
            RetryCount++;
            return this;
        }

        /// <summary>
        /// Sets the retry execution is failed.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal RetryContext RetryIsFailed() => this.With(r => r.RetryFailed = true);

        /// <summary>
        /// Sets that the retry execution is not failed.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal RetryContext RetryIsNotFailed() => this.With(r => r.RetryFailed = false);
    }
}
