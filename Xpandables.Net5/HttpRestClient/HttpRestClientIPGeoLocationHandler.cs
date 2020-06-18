
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
using System.Net.Http;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// Default implementation for <see cref="IHttpRestClientIPGeoLocationHandler"/>.
    /// </summary>
    public sealed class HttpRestClientIPGeoLocationHandler : IHttpRestClientIPGeoLocationHandler
    {
        private readonly IHttpRestClientHandler _httpRestClient;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientIPGeoLocationHandler"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClientFactory"/> is null.</exception>
        public HttpRestClientIPGeoLocationHandler(IHttpClientFactory httpClientFactory)
            => _httpRestClient = new HttpRestClientHandler(httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory)));

        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request" /> is null.</exception>
        public async Task<HttpRestClientResponse<IPGeoLocationResponse>> GetIPGeoLocationAsync(IPGeoLocationRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            _httpRestClient.Initialize(IHttpRestClientIPGeoLocationHandler.IPGeoLocationHttpClientName);
            return await _httpRestClient.HandleAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously gets the IPAddress of the current caller.
        /// </summary>
        public async Task<HttpRestClientResponse<string>> GetIPAddressAsync()
        {
            _httpRestClient.Initialize(IHttpRestClientIPGeoLocationHandler.IPGeoAddressHttpClientName);
            return await _httpRestClient.HandleAsync(new IPGeoAddressRequest()).ConfigureAwait(false);
        }
    }
}
