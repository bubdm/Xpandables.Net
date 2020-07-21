
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
using System.ComponentModel.DataAnnotations;

using Xpandables.Net.HttpRestClient;

namespace Xpandables.NetCore.Helpers
{
    /// <summary>
    /// Helpers for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientHelpers
    {
        /// <summary>
        /// Return an <see cref="HttpRestClientValidation"/> from a model state.
        /// </summary>
        /// <param name="modelState">The model state to act on.</param>
        /// <returns>An instance of <see cref="HttpRestClientValidation"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelState"/> is null.</exception>
        public static HttpRestClientValidation GetHttpRestClientValidation(this ModelStateDictionary modelState)
        {
            _ = modelState ?? throw new ArgumentNullException(nameof(modelState));
            return new HttpRestClientValidation(modelState
                .Keys
                .Where(key => modelState[key].Errors.Count > 0)
                .Select(key => new { MemberName = key, ErrorMessages = modelState[key].Errors.Select(error => error.ErrorMessage) })
                .ToDictionary(d => d.MemberName, d => d.ErrorMessages));
        }

        /// <summary>
        /// Return an <see cref="HttpRestClientValidation"/> from a validation exception.
        /// </summary>
        /// <param name="validationException">The validation exception to act on.</param>
        /// <returns>An instance of <see cref="HttpRestClientValidation"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validationException"/> is null.</exception>
        public static HttpRestClientValidation GetHttpRestClientValidation(this ValidationException validationException)
        {
            _ = validationException ?? throw new ArgumentNullException(nameof(validationException));
            return new HttpRestClientValidation(validationException
                .ValidationResult
                .MemberNames
                .Select(member => new { MemberName = member, ErrorMessages = new[] { validationException.ValidationResult.ErrorMessage } })
                .ToDictionary(d => d.MemberName, d => d.ErrorMessages.AsEnumerable()));
        }
    }
}
