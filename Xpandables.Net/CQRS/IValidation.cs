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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Defines method contracts used to validate a type-specific argument using a decorator.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// <para>Contains default implementation. You just need to override the method.</para>
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface IValidation<in TArgument>
        where TArgument : class
    {
        /// <summary>
        /// Gets the zero-base order in which the validator will be executed.
        /// The default value is zero.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// Asynchronously validates the argument and throws the <see cref="ValidationException"/> if necessary.
        /// The default behavior uses <see cref="Validator.ValidateObject(object, ValidationContext, bool)"/>.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public virtual async Task ValidateAsync(TArgument argument)
        {
            Validator.ValidateObject(argument, new ValidationContext(argument, null, null), true);
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}