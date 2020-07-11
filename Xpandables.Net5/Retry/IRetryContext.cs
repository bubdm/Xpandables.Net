
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

using Xpandables.Net5.Assertion;
using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.Retry
{
    /// <summary>
    /// The retry behavior execution context.
    /// </summary>
    public interface IRetryContext
    {
        /// <summary>
        /// Gets the handled exception.
        /// </summary>
        Exception Exception { get; internal set; }

        /// <summary>
        /// Gets or sets the retry interval in milliseconds.
        /// </summary>
        TimeSpan TimeInterval { get; internal set; }

        /// <summary>
        /// Gets the retry number to be executed.
        /// </summary>
        int RetryCount { get; internal set; }

        /// <summary>
        /// Determines whether the retry execution failed. Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        internal bool? RetryFailed { get; set; }

        /// <summary>
        /// Updates the <see cref="TimeInterval"/> value.
        /// </summary>
        /// <param name="timeInterval">The new value.</param>
        /// <returns>The instance with the new time interval.</returns>
        /// <exception cref="ArgumentException">The <paramref name="timeInterval"/> must be greater or equal to zero.</exception>
        public sealed IRetryContext UpdateTimeInterval(TimeSpan timeInterval)
        {
            TimeInterval = timeInterval.WhenNotGreaterThan(TimeSpan.FromMilliseconds(0), nameof(timeInterval)).ThrowArgumentOutOfRangeException();
            return this;
        }

        /// <summary>
        /// Updates the exception value.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        /// <returns>The instance with the new exception.</returns>
        /// <exception cref="ArgumentException">The <paramref name="exception"/> is null.</exception>
        internal sealed IRetryContext UpdateException(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            return this;
        }

        /// <summary>
        /// Increases the retry count value.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal IRetryContext IncreaseRetryCount()
        {
            RetryCount++;
            return this;
        }

        /// <summary>
        /// Sets the retry execution is failed.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal sealed IRetryContext RetryIsFailed() => this.With(r => r.RetryFailed = true);

        /// <summary>
        /// Sets that the retry execution is not failed.
        /// </summary>
        /// <returns>The instance with the new value.</returns>
        internal sealed IRetryContext RetryIsNotFailed() => this.With(r => r.RetryFailed = false);
    }
}
