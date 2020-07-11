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

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Represents an HTTP Rest API exception.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public sealed class HttpRestClientException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        /// <summary>
        /// Initializes a instance of <see cref="HttpRestClientException"/> class with the content.
        /// </summary>
        /// <param name="content">The exception content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="content"/> is null.</exception>
        public HttpRestClientException(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Initializes the default instance of <see cref="HttpRestClientException"/> class.
        /// </summary>
        public HttpRestClientException() { }

        /// <summary>
        /// Gets or sets the exception content.
        /// </summary>
        public string? Content { get; set; }
    }
}
