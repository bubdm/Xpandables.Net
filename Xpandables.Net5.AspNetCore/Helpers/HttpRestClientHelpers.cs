
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
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net5.HttpRestClient;

namespace Xpandables.Net5.AspNetCore.Helpers
{
    /// <summary>
    /// Helpers for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientHelpers
    {
        /// <summary>
        /// Returns a collection of <see cref="HttpRestClientModelError"/> from the model state.
        /// </summary>
        /// <param name="modelState">The model state to act on.</param>
        /// <returns>A collection of <see cref="HttpRestClientModelError"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelState"/> is null.</exception>
        public static IEnumerable<HttpRestClientModelError> GetHttpRestClientModelErrors(this ModelStateDictionary modelState)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            return modelState
                .Keys
                .Where(key => modelState[key].Errors.Count > 0)
                .Select(key => new HttpRestClientModelError(key, modelState[key].Errors.Select(error => error.ErrorMessage)));
        }

        /// <summary>
        /// Return an <see cref="HttpRestClientModelResult"/> from a model state.
        /// </summary>
        /// <param name="modelState">The model state to act on.</param>
        /// <returns>An instance of <see cref="HttpRestClientModelResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelState"/> is null.</exception>
        public static HttpRestClientModelResult GetHttpRestClientModelResult(this ModelStateDictionary modelState)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            var errors = modelState.GetHttpRestClientModelErrors();
            return new HttpRestClientModelResult(errors.ToList());
        }
    }
}
