﻿
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.RequestHandlers
{
    /// <summary>
    /// Provides with a handler that can be used with <see cref="HttpClient"/> to add header authorization value
    /// before request execution.
    /// </summary>
    public class HttpRestClientAuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpHeaderAccessor _httpHeaderAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientAuthorizationHandler"/> class with the token accessor.
        /// </summary>
        /// <param name="httpHeaderAccessor">The token accessor to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpHeaderAccessor"/> is null.</exception>
        public HttpRestClientAuthorizationHandler(IHttpHeaderAccessor httpHeaderAccessor)
            => _httpHeaderAccessor = httpHeaderAccessor ?? throw new ArgumentNullException(nameof(httpHeaderAccessor));

        /// <summary>
        /// Creates an instance of System.Net.Http.HttpResponseMessage based on the information
        /// provided in the System.Net.Http.HttpRequestMessage as an operation that will not block.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The token is not available. See inner exception.</exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Headers.Authorization is not { Parameter: null } authorization)
                return base.SendAsync(request, cancellationToken);

            var token = _httpHeaderAccessor.Request.ReadValue("Authorization") ?? throw new InvalidOperationException("Expected authorization not found.");
            request.Headers.Authorization = new AuthenticationHeaderValue(authorization.Scheme, token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
