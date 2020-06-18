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

using System.ComponentModel.DataAnnotations;

namespace System.Design
{
    /// <summary>
    /// Represents an HTTP client validation exception.
    /// This exception matches the <see cref="ValidationResult"/>.
    /// </summary>
    public sealed class HttpRestClientValidationException : Exception
    {
        /// <summary>
        /// Initializes a default instance of <see cref="HttpRestClientValidationException"/> class.
        /// </summary>
        public HttpRestClientValidationException() { }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientValidationException"/> class with arguments.
        /// </summary>
        /// <param name="memberNames">A collection of member names.</param>
        /// <param name="errorMessage">The error message.</param>
        public HttpRestClientValidationException(string[] memberNames, string errorMessage)
        {
            MemberNames = memberNames;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the list of member names.
        /// </summary>
        public string[]? MemberNames { get; set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
