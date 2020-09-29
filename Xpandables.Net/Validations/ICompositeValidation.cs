
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// Defines method contracts used to validate a type-specific argument by composition using a decorator.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface ICompositeValidation<in TArgument> : IValidation<TArgument>
        where TArgument : class
    {
        internal IEnumerable<IValidation<TArgument>> ValidationInstances { get; }

        /// <summary>
        /// Asynchronously validates the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public new async Task ValidateAsync(TArgument argument)
        {
            var tasks = ValidationInstances.OrderBy(o => o.Order).Select(validator => validator.ValidateAsync(argument));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}