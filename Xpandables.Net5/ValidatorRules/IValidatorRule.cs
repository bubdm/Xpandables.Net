﻿
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

namespace Xpandables.Net5.ValidatorRules
{
    /// <summary>
    /// Defines a method contract used to validate an argument.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// <para>Contains default implementation. You just need to override the method.</para>
    /// </summary>
    public interface IValidatorRule
    {
        /// <summary>
        /// Asynchronous applies validation process and throws the <see cref="ValidationException"/> if necessary.
        /// <para>The default implementation use <see cref="Validator.ValidateObject(object, ValidationContext, bool)"/>.</para>
        /// </summary>
        /// <param name="target">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public async Task ValidateAsync(object target)
        {
            Validator.ValidateObject(target, new ValidationContext(target, null, null), true);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Determines the zero-base order in which the validator will be executed.
        /// The default value is zero.
        /// </summary>
        int Order { get => 0; }
    }

    /// <summary>
    /// Defines a method contract used to validate a type-specific argument.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// <para>Contains default implementation. You just need to override the method.</para>
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface IValidatorRule<in TArgument> : IValidatorRule
        where TArgument : class
    {
        /// <summary>
        /// Asynchronously applies validation the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public async Task ValidateAsync(TArgument argument) => await ValidateAsync(argument).ConfigureAwait(false);
    }
}