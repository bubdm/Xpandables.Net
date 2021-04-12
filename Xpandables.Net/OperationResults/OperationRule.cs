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

using System.Diagnostics.CodeAnalysis;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents a helper class that allows implementation of the <see cref="IOperationRule{TArgument}"/>.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be checked.</typeparam>
    public abstract class OperationRule<TArgument> : OperationResultBase, IOperationRule<TArgument>
         where TArgument : notnull
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OperationRule{TArgument}"/> class that sets <see cref="Result"/> to <see cref="SuccessOperationResult"/>.
        /// The default <see cref="Result"/> is <see cref="SuccessOperationResult"/>.
        /// </summary>
        protected OperationRule() => Result = new SuccessOperationResult();

        /// <summary>
        /// If <see cref="IsSatisfiedBy(TArgument)" /> is <see langword="false" />, the property should be a <see cref="FailureOperationResult" />.
        /// </summary>
        public IOperationResult Result { get; protected set; }

        /// <summary>
        /// Returns a value that determines whether or not the instance is satisfied by the argument, after calling the <see cref="ApplyRule(TArgument)"/> method.
        /// </summary>
        /// <param name="argument">The target argument to be checked.</param>
        /// <returns><see langword="true"/> if the argument satisfies the instance; otherwise returns <see langword="false"/>.</returns>
        public bool IsSatisfiedBy(TArgument argument)
        {
            ApplyRule(argument);
            return Result.Succeeded;
        }

        /// <summary>
        /// When overridden in derived class, this method will do the actual job of applying the rule and set the <see cref="Result"/> property to the expected value.
        /// The default <see cref="Result"/> is <see cref="SuccessOperationResult"/>.
        /// </summary>
        /// <param name="argument">The target argument to be checked.</param>
        protected abstract void ApplyRule(TArgument argument);
    }
}
