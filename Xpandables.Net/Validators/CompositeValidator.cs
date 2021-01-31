
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Validators
{
    /// <summary>
    /// The composite validation class used to wrap all validators for a specific type.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated</typeparam>
    [Serializable]
    public class CompositeValidator<TArgument> : Validator<TArgument>, ICompositeValidator<TArgument>
        where TArgument : class
    {
        private readonly IEnumerable<IValidator<TArgument>> _validationInstances;

        /// <summary>
        /// Initializes the composite validation with all validation instances for the argument.
        /// </summary>
        /// <param name="validationInstances">The collection of validators to act with.</param>
        public CompositeValidator(IEnumerable<IValidator<TArgument>> validationInstances)
            => _validationInstances = validationInstances;

        /// <summary>
        /// Asynchronously validates the argument and returns validation state with errors if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <returns>Returns a result state that contains validation information.</returns>
        public override async Task<IOperationResult> ValidateAsync(TArgument argument, CancellationToken cancellationToken)
        {
            foreach (var validator in _validationInstances.OrderBy(o => o.Order))
            {
                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
                var result = await validator.ValidateAsync(argument, cancellationToken).ConfigureAwait(false);
                if (result.IsFailure)
                    return result;
            }

            return new SuccessOperationResult();
        }
    }
}