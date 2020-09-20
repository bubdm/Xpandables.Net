
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

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IValidation{TArgument}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    public sealed class ValidationBuilder<TArgument> : IValidation<TArgument>
        where TArgument : class
    {
        private readonly Action<TArgument> _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationBuilder{TArgument}"/> class with the delegate to be used
        /// as <see cref="IValidation{TArgument}"/> implementation.
        /// </summary>
        /// <param name="validator">The delegate validator.</param>
        /// <exception cref="ArgumentException">The <paramref name="validator"/> is null.</exception>
        public ValidationBuilder(Action<TArgument> validator) =>
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        /// <summary>
        /// Validates the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public void Validate(TArgument argument) => _validator(argument);
    }
}
