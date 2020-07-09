
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
using System.Globalization;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Exception thrown for <see cref="InstanceClass"/> parsing error.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public sealed class InstanceException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        /// <summary>
        /// Returns a new instance of <see cref="InstanceException"/> with the <paramref name="message"/> and <paramref name="position"/> parameters.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="position">The position of the message</param>
        public InstanceException(string message, int position)
            : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the position of the message.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the <see cref="InstanceException"/> <see cref="ToString"/> expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, InstanceMessages.ParseExceptionFormat, Message, Position);
    }
}
