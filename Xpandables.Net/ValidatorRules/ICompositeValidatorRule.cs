
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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// Defines a method contract used to validate an argument using composition (wrapping all validators of a specific-type).
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface ICompositeValidatorRule
    {
        /// <summary>
        /// Asynchronously applies all validators to the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="target">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        Task ValidateAsync(object target);
    }

    /// <summary>
    /// Defines a method contract used to validate a type-specific argument using composition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface ICompositeValidatorRule<in TArgument> : ICompositeValidatorRule
        where TArgument : class
    {
        /// <summary>
        /// Asynchronously applies all validators to the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        Task ValidateAsync(TArgument argument);
    }
}