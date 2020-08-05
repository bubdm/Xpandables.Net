
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
using System.Threading.Tasks;

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IValidatorRule{TArgument}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    public sealed class ValidatorRuleBuilder<TArgument> : IValidatorRule<TArgument>
        where TArgument : class
    {
        private readonly Func<TArgument, Task> _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatorRuleBuilder{TArgument}"/> class with the delegate to be used
        /// as <see cref="IValidatorRule{TArgument}"/> implementation.
        /// </summary>
        /// <param name="validator"></param>
        public ValidatorRuleBuilder(Func<TArgument, Task> validator) =>
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        /// <summary>
        /// Asynchronously applies validation the argument and throws the <see langword="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        public async Task ValidateAsync(TArgument argument) => await _validator(argument).ConfigureAwait(false);
    }
}
