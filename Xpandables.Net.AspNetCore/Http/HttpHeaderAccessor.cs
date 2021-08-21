
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

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Default implementation of <see cref="IHttpHeaderReader"/> that uses an <see cref="IHttpContextAccessor"/> to access context.
    /// </summary>
    public class HttpHeaderAccessor : IHttpHeaderReader
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HttpHeaderAccessor"/>.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpContextAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The HTTP context is null.</exception>
        public HttpHeaderAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _ = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            if (httpContextAccessor.HttpContext is null)
                throw new ArgumentException("The expected HTTP Context is null.");

            Request = new HttpHeaderProvider(httpContextAccessor.HttpContext.Request.Headers);
            Response = new HttpHeaderProvider(httpContextAccessor.HttpContext.Response.Headers);
        }

        ///<inheritdoc/>
        public IHttpHeaderProvider Request { get; }

        ///<inheritdoc/>
        public IHttpHeaderProvider Response { get; }
    }
}
