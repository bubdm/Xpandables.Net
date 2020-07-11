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

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Contains the HTTP Rest API model error.
    /// </summary>
    public sealed class HttpRestClientModelError
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientModelResult"/> that contains errors message and the member name.
        /// </summary>
        /// <param name="memberName">The member name for the error.</param>
        /// <param name="errorsMessage">A collection of errors message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="memberName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="errorsMessage"/> is null or empty.</exception>
        public HttpRestClientModelError(string memberName, IEnumerable<string> errorsMessage)
        {
            MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            ErrorsMessage = errorsMessage?.ToList() ?? throw new ArgumentNullException(nameof(errorsMessage));
        }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets the collection of errors message.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public IList<string> ErrorsMessage { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
