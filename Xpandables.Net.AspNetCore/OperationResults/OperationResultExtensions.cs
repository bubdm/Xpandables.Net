
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
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System;
using System.Linq;
using System.Net;

namespace Xpandables.Net.OperationResults
{
    /// <summary>
    /// <see cref="IOperationResult"/> extensions.
    /// </summary>
    public static class OperationResultExtensions
    {
        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> from the specified model state.
        /// The model state must be in a invalid state.
        /// </summary>
        /// <param name="modelState">the target model state to act on.</param>
        /// <returns>A new instance of a <see cref="FailureOperationResult"/> that contains errors from model state.</returns>
        /// <exception cref="ArgumentException">The <paramref name="modelState"/> is valid.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelState"/> is null.</exception>
        public static IOperationResult GetFailedOperationResult(this ModelStateDictionary modelState)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            if (modelState.IsValid) throw new ArgumentException("Unable to retrieve a failed operation from a valid model state");

            return new FailureOperationResult(
                HttpStatusCode.BadRequest,
                modelState
                    .Keys
                    .Where(key => modelState[key].Errors.Count > 0)
                    .Select(key => new OperationError(key, modelState[key].Errors.Select(error => error.ErrorMessage).ToArray()))
                    .ToList());
        }
    }
}
