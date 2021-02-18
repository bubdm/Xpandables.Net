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

namespace Xpandables.Net.Validators
{
    /// <summary>
    /// Defines property and method contracts used to validate a type-specific argument.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface IOperationValidator<in TArgument>
        where TArgument : notnull
    {
        /// <summary>
        /// Returns a value that determines whether or not the argument is valid.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <returns><see langword="true"/> if the argument is valid; otherwise returns <see langword="false"/>.</returns>
        bool IsSatisfiedBy(TArgument argument);

        /// <summary>
        /// If <see cref="IsSatisfiedBy(TArgument)"/> is <see langword="false"/>, the property should be a <see cref="FailureOperationResult"/>.
        /// </summary>
        IOperationResult Result { get; }
    }
}
