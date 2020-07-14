
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
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xpandables.Net5.Http;

namespace Xpandables.Net5.AspNetCore.Http
{
    /// <summary>
    /// Implementation for <see cref="IHttpHeaderAccessor"/>.
    /// </summary>
    public sealed class HttpHeaderAccessor : IHttpHeaderAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpHeaderAccessor"/> class.
        /// </summary>
        /// <param name="contextAccessor">The HTTP context accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="contextAccessor"/> is null.</exception>
        public HttpHeaderAccessor(IHttpContextAccessor contextAccessor)
            => _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

        /// <summary>
        /// Gets all HTTP header values from the current HTTP request.
        /// If not found, returns an empty dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetValues()
        {
            var headers = _contextAccessor.HttpContext?.Request?.Headers?.ToDictionary(d => d.Key, d => (IReadOnlyList<string>)d.Value);
            if (headers is null)
            {
                throw new InvalidOperationException(
                    $"Unable to access HttpContext from {nameof(IHttpContextAccessor.HttpContext)}",
                    new ArgumentException("Getting the header values failed."));
            }

            return new ReadOnlyDictionary<string, IReadOnlyList<string>>(headers);
        }
    }
}
