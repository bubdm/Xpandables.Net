
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
using System.Threading.Tasks;

using Xpandables.Net.HttpRestClient;

namespace Xpandables.Net.Http.Network
{
    /// <summary>
    /// Default implementation for <see cref="IHttpLocationHandler"/>.
    /// </summary>
    public sealed class HttpLocationHandler : Disposable, IHttpLocationHandler
    {
        private readonly IHttpRestClientHandler _httpRestClientHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpLocationHandler"/> class with the client to be used.
        /// </summary>
        /// <param name="httpClient">The HTTP client to be used to request Geo location.</param>
        /// <param name="httpRestClientEngine">The HTTP Rest client engine.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpLocationHandler(HttpClient httpClient, IHttpRestClientEngine httpRestClientEngine)
            => _httpRestClientHandler = new HttpRestClientHandler(httpClient, httpRestClientEngine);

        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request using http://api.ipstack.com.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse<GeoLocation>> ReadLocationAsync(LocationRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return await _httpRestClientHandler.HandleAsync(request).ConfigureAwait(false);
        }

        private bool _isDisposed;

        /// <summary>
        /// Disposes the <see cref="IHttpRestClientHandler"/> instance.
        /// </summary>
        /// <param name="disposing">Determine whether the dispose has already been called.</param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                _httpRestClientHandler?.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
