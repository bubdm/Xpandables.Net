
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
using System.Design;
using System.Linq;

namespace System
{
    /// <summary>
    /// Describes the parameters for a command/query used to apply retry behavior.
    /// The attribute should decorate implementations of <see cref="IQuery{TResult}"/> or <see cref="ICommand"/>.
    /// Your class can implement the <see cref="IRetryBehaviorAttributeProvider"/> to dynamically return a <see cref="RetryBehaviorAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RetryBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Gets the collection of exception types that allow the retry behavior to occur.
        /// The default value is <see cref="Exception"/>.
        /// </summary>
        public Type[] ExceptionTypes { get; set; } = new[] { typeof(Exception) };

        /// <summary>
        /// Gets the number of retry. The default value is 3.
        /// </summary>
        public int RetryNumber { get; set; } = 3;

        /// <summary>
        /// Gets the interval time between retries (in milliseconds). The default value is 500.
        /// </summary>
        public int RetryInterval { get; set; } = 500;

        /// <summary>
        /// Determines whether or not the attribute arguments are valid.
        /// </summary>
        /// <returns>Returns the current instance if so, otherwise throws an exception.</returns>
        /// <exception cref="ArgumentNullException">Arguments are not valid.</exception>
        public RetryBehaviorAttribute IsValid()
        {
            if (ExceptionTypes?.Any() != true) throw new RetryBehaviorException(nameof(ExceptionTypes));
            if (!ExceptionTypes.Any(type => type.IsSubclassOf(typeof(Exception))))
                throw new RetryBehaviorException(nameof(ExceptionTypes), new ArgumentNullException(nameof(ExceptionTypes)));
            if (RetryNumber <= 0) throw new RetryBehaviorException(nameof(RetryNumber), new ArgumentOutOfRangeException(nameof(RetryNumber)));
            if (RetryInterval <= 0) throw new RetryBehaviorException(nameof(RetryInterval), new ArgumentOutOfRangeException(nameof(RetryInterval)));

            return this;
        }
    }
}
