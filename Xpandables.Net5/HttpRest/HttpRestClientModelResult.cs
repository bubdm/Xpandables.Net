
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
using System.Text.Json.Serialization;

using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Contains HTTP Rest API validation model result.
    /// </summary>
    public sealed partial class HttpRestClientModelResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientModelResult"/> that contains the message and the member names.
        /// </summary>
        /// <param name="errors">A collection of member names for the error.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        [JsonConstructor]
        public HttpRestClientModelResult(IList<HttpRestClientModelError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientModelResult"/> that contains the validation error.
        /// </summary>
        /// <param name="validationError">the validation error.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationError"/> is null.</exception>
        public HttpRestClientModelResult(HttpRestClientModelError validationError)
        {
            Errors = new List<HttpRestClientModelError>
            {
                validationError
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientModelResult"/> that contains the <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is null.</exception>
        public HttpRestClientModelResult(ValidationResult validationResult)
        {
            _ = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
            Errors = new List<HttpRestClientModelError>();
            foreach (var memberName in validationResult.MemberNames)
                Errors.Add(new HttpRestClientModelError(memberName, validationResult.ErrorMessage.SingleToEnumerable()));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientModelResult"/> that contains a delegate used to provide validation errors.
        /// </summary>
        /// <param name="validtionErrorsProvider">The validation errors provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validtionErrorsProvider"/> is null.</exception>
        public HttpRestClientModelResult(Func<IList<HttpRestClientModelError>> validtionErrorsProvider)
        {
            Errors = validtionErrorsProvider?.Invoke() ?? throw new ArgumentNullException(nameof(validtionErrorsProvider));
        }

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public IList<HttpRestClientModelError> Errors { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
