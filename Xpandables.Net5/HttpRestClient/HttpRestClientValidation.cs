
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

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// Contains HTTP Rest API validation model result.
    /// </summary>
    public sealed partial class HttpRestClientValidation : Dictionary<string, IEnumerable<string>>
    {
        /// <summary>
        /// Adds the specified member name and list of error messages to the dictionary.
        /// </summary>
        /// <param name="memberName">The member name of the list to add.</param>
        /// <param name="errorMessages">The list of the error messages to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="memberName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="errorMessages"/> is null.</exception>
        /// <exception cref="ArgumentException">A member with the same name already exists in the dictionary.</exception>
        public new void Add(string memberName, IEnumerable<string> errorMessages)
            => base.Add(memberName, errorMessages);

        /// <summary>
        /// Gets or sets the list of error messages associated with the specified member name.
        /// </summary>
        /// <param name="memberName">The member name of the list of error messages to get or set.</param>
        /// <returns>The list of messages associated with the specified member. If the specified member name is not found,
        /// a get operation throws a <see cref="KeyNotFoundException"/>, and
        /// a set operation creates a new list of error messages with the specified member name.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberName"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public new IEnumerable<string> this[string memberName]
        {
            get => base[memberName];
            set => base[memberName] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientValidation"/> class
        /// that is empty, has the default initial capacity, and uses the default equality
        /// comparer for the member name.
        /// </summary>
        public HttpRestClientValidation() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientValidation"/> class
        /// that contains elements copied from the specified <see cref="Dictionary{TKey, TValue}"/>
        /// and uses the default equality comparer for the member name type.
        /// </summary>
        /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> whose elements are copied to the
        /// new <see cref="HttpRestClientValidation"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dictionary"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        public HttpRestClientValidation(IDictionary<string, IEnumerable<string>> dictionary)
            : base(dictionary) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientValidation"/> class
        /// that contains elements copied from the specified <see cref="IEnumerable{T}"/>
        /// and uses the default equality comparer for the member name type.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new <see cref="HttpRestClientValidation"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public HttpRestClientValidation(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection)
            : base(collection) { }

        //        /// <summary>
        //        /// Initializes a new instance of <see cref="HttpRestClientValidationResult"/> that contains the message and the member names.
        //        /// </summary>
        //        /// <param name="errors">A collection of member names for the error.</param>
        //        /// <exception cref="ArgumentNullException">The <paramref name="errors"/> is null.</exception>
        //        [JsonConstructor]
        //        public HttpRestClientValidationResult(IList<HttpRestClientValidationError> errors)
        //        {
        //            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        //        }

        //        /// <summary>
        //        /// Initializes a new instance of <see cref="HttpRestClientValidationResult"/> that contains the validation error.
        //        /// </summary>
        //        /// <param name="validationError">the validation error.</param>
        //        /// <exception cref="ArgumentNullException">The <paramref name="validationError"/> is null.</exception>
        //        public HttpRestClientValidationResult(HttpRestClientValidationError validationError)
        //        {
        //            Errors = new List<HttpRestClientValidationError>
        //            {
        //                validationError
        //            };
        //        }

        //        /// <summary>
        //        /// Initializes a new instance of <see cref="HttpRestClientValidationResult"/> that contains the <see cref="ValidationResult"/>.
        //        /// </summary>
        //        /// <param name="validationResult">The validation result.</param>
        //        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is null.</exception>
        //        public HttpRestClientValidationResult(ValidationResult validationResult)
        //        {
        //            _ = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        //            Errors = new List<HttpRestClientValidationError>();
        //            foreach (var memberName in validationResult.MemberNames)
        //                Errors.Add(new HttpRestClientValidationError(memberName, validationResult.ErrorMessage.SingleToEnumerable()));
        //        }

        //        /// <summary>
        //        /// Initializes a new instance of <see cref="HttpRestClientValidationResult"/> that contains a delegate used to provide validation errors.
        //        /// </summary>
        //        /// <param name="validtionErrorsProvider">The validation errors provider.</param>
        //        /// <exception cref="ArgumentNullException">The <paramref name="validtionErrorsProvider"/> is null.</exception>
        //        public HttpRestClientValidationResult(Func<IList<HttpRestClientValidationError>> validtionErrorsProvider)
        //        {
        //            Errors = validtionErrorsProvider?.Invoke() ?? throw new ArgumentNullException(nameof(validtionErrorsProvider));
        //        }

        //        /// <summary>
        //        /// Gets the collection of validation errors.
        //        /// </summary>
        //#pragma warning disable CA2227 // Collection properties should be read only
        //        public IList<HttpRestClientValidationError> Errors { get; set; }
        //#pragma warning restore CA2227 // Collection properties should be read only
    }
}
