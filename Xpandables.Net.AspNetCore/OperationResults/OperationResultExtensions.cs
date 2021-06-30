
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
using System.Net;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Xpandables.Net
{
    /// <summary>
    /// <see cref="IOperationResult"/> extensions.
    /// </summary>
    public static class OperationResultExtensions
    {
        /// <summary>
        /// Returns a <see cref="FailureOperationResult"/> from the specified model state.
        /// The model state must be in an invalid state.
        /// </summary>
        /// <param name="modelState">the target model state to act on.</param>
        /// <param name="statusCode">The status code to apply to the result. The default is <see cref="HttpStatusCode.BadRequest"/>.</param>
        /// <returns>A new instance of a <see cref="FailureOperationResult"/> that contains errors from model state.</returns>
        /// <exception cref="ArgumentException">The <paramref name="modelState"/> is valid.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelState"/> is null.</exception>
        public static IOperationResult GetBadOperationResult(
            this ModelStateDictionary modelState, 
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            if (modelState.IsValid) throw new ArgumentException("Unable to retrieve a failed operation from a valid model state");

            return new FailureOperationResult(
                statusCode,
                modelState
                    .Keys
                    .Where(key => modelState[key].Errors.Count > 0)
                    .Select(key => new OperationError(key, modelState[key].Errors.Select(error => error.ErrorMessage).ToArray()))
                    .ToList());
        }

        /// <summary>
        /// Returns a <see cref="ModelStateDictionary"/> from the collection of errors.
        /// </summary>
        /// <param name="operationErrors">The collection of errors to act on.</param>
        /// <returns>A new instance of <see cref="ModelStateDictionary"/> that contains found errors.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="operationErrors"/> is null.</exception>
        public static ModelStateDictionary GetModelStateDictionary(this IReadOnlyCollection<OperationError> operationErrors)
        {
            _ = operationErrors ?? throw new ArgumentNullException(nameof(operationErrors));

            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in operationErrors)
            {
                foreach (var errorMessage in error.ErrorMessages)
                    modelStateDictionary.AddModelError(error.Key, errorMessage);
            }

            return modelStateDictionary;
        }
    }
}
