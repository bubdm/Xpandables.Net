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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Validators
{
    /// <summary>
    /// Represents a helper class that allows implementation of the <see cref="IValidator{TArgument}"/>.
    /// The default behavior uses <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}?, bool)"/>.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument.</typeparam>
    public abstract class Validator<TArgument> : OperationResults, IValidator<TArgument>
        where TArgument : notnull
    {
        /// <summary>
        /// Asynchronously validates the argument and returns validation state with errors if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <returns>Returns a result state that contains validation information.</returns>
        /// <remarks>You can throw an <see cref="OperationResultException"/> also.</remarks>
        public virtual async Task<IOperationResult> ValidateAsync(TArgument argument, CancellationToken cancellationToken = default)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(argument, new ValidationContext(argument, null, null), validationResults, true))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var errors = new OperationErrorCollection();
                foreach (var validationResult in validationResults)
                {
                    foreach (var member in validationResult.MemberNames)
                    {
                        if (validationResult.ErrorMessage is not null)
                        {
                            if (errors[member] is { } error)
                            {
                                errors[member]!.ErrorMessages = error.ErrorMessages.Union(new[] { validationResult.ErrorMessage }).ToArray();
                            }
                            else
                            {
                                errors.Add(member, validationResult.ErrorMessage);
                            }
                        }
                    }
                }

                return BadOperation(errors);
            }

            return await Task.FromResult(OkOperation()).ConfigureAwait(false);
        }
    }
}
