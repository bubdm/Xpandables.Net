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
    /// Represents a helper class that allows implementation of the <see cref="IOperationValidator{TArgument}"/>.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public abstract class OperationValidator<TArgument> : OperationExtended, IOperationValidator<TArgument>
         where TArgument : notnull
    {
        /// <summary>
        /// Defines the <see cref="IOperationResult"/> result of the validator.
        /// </summary>
        protected IOperationResult Result = new SuccessOperationResult();

        /// <summary>
        /// If <see cref="IsSatisfiedBy(TArgument)" /> is <see langword="false" />, the property should be a <see cref="FailureOperationResult" />.
        /// </summary>
        public IOperationResult OperationResult => Result;

        /// <summary>
        /// Returns a value that determines whether or not the argument is valid.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <returns><see langword="true" /> if the argument is valid; otherwise returns <see langword="false" />.</returns>
        public abstract bool IsSatisfiedBy(TArgument argument);
    }
}
