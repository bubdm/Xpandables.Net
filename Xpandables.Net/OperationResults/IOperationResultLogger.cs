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

using Xpandables.Net.Decorators;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents a type used to perform operation results logging for classes implementing the <see cref="ILoggingDecorator"/> .
    /// </summary>
    public interface IOperationResultLogger
    {
        /// <summary>
        /// Method executed before the decorated handler to which the decorator is applied.
        /// </summary>
        /// <param name="argument">The arguments with which the handler has been invoked.</param>
        void OnEntry(LoggerArgument argument);

        /// <summary>
        /// Method executed after the decorated handler to which the decorator is applied, even when the handler exists with an exception (this method is invoked from the finally block).
        /// </summary>
        /// <param name="argument">The arguments with which the handler has been invoked.</param>
        void OnExit(LoggerArgument argument);

        /// <summary>
        /// Method executed after the decorated handler to which the decorator is applied, but only when the handler successfully returns (i.e. when no exception flies out the handler).
        /// </summary>
        /// <param name="argument">The arguments with which the handler has been invoked.</param>
        void OnSuccess(LoggerArgument argument);

        /// <summary>
        /// Method executed after the decorated handler to which the decorator is applied, in case that the handler resulted with an exception.
        /// </summary>
        /// <param name="argument">The arguments with which the handler has been invoked.</param>
        void OnException(LoggerArgument argument);
    }
}
