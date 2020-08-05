
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// The composite validator used to wrap all validators for a specific type.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated</typeparam>
    [Serializable]
    public sealed class CompositeValidatorRule<TArgument> : ICompositeValidatorRule<TArgument>
        where TArgument : class
    {
        private readonly IEnumerable<IValidatorRule<TArgument>> _validators;

        /// <summary>
        /// Initializes the composite validator with all validators for the argument.
        /// </summary>
        /// <param name="validators">The collection of validators to act with.</param>
        public CompositeValidatorRule(IEnumerable<IValidatorRule<TArgument>> validators)
            => _validators = validators ?? Enumerable.Empty<IValidatorRule<TArgument>>();

        /// <summary>
        /// Asynchronously applies all validators to the argument and throws the <see langword="ValidationException" /> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public async Task ValidateAsync(TArgument argument)
        {
            var tasks = _validators.OrderBy(o => o.Order).Select(validator => validator.ValidateAsync(argument));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        async Task ICompositeValidatorRule.ValidateAsync(object target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            if (target is TArgument argument)
                await ValidateAsync(argument).ConfigureAwait(false);
            else
                throw new ArgumentException($"{nameof(target)} is not of {typeof(TArgument).Name} type.");
        }
    }
}