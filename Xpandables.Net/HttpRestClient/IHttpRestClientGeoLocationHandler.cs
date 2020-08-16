
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

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client Geo-location using a typed client HTTP Client.
    /// </summary>
    public interface IHttpRestClientGeoLocationHandler : IDisposable
    {
        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request using http://api.ipstack.com.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        Task<HttpRestClientResponse<GeoLocationResponse>> GetGeoLocationAsync(GeoLocationRequest request);
    }

    /// <summary>
    /// Default implementation for <see cref="IHttpRestClientGeoLocationHandler"/>.
    /// </summary>
    public sealed class HttpRestClientGeoLocationHandler : Disposable, IHttpRestClientGeoLocationHandler
    {
        private readonly IHttpRestClientHandler _httpRestClientHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientGeoLocationHandler"/> class with the client to be used.
        /// </summary>
        /// <param name="httpClient">The HTTP client to be used to request Geo location.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientGeoLocationHandler(HttpClient httpClient)
            => _httpRestClientHandler = new HttpRestClientHandler(httpClient ?? throw new ArgumentNullException(nameof(httpClient)));

        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request using http://api.ipstack.com.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse<GeoLocationResponse>> GetGeoLocationAsync(GeoLocationRequest request)
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
