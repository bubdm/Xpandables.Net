
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

namespace Xpandables.Net.HttpRestClient.Network
{
    /// <summary>
    /// Default implementation for <see cref="IHttpRestClientIPHandler"/>.
    /// </summary>
    public sealed class HttpRestClientIPHandler : Disposable, IHttpRestClientIPHandler
    {
        private readonly IHttpRestClientHandler _httpRestClientHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientIPHandler"/> class with the client to be used.
        /// </summary>
        /// <param name="httpClient">The client to be used to get the IP address.</param>
        /// <param name="httpRestClientEngine">The HTTP Rest client engine.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientEngine"/> is null.</exception>
        public HttpRestClientIPHandler(HttpClient httpClient, IHttpRestClientEngine httpRestClientEngine)
            => _httpRestClientHandler = new HttpRestClientHandler(httpClient, httpRestClientEngine);

        /// <summary>
        /// Asynchronously gets the IPAddress of the current caller using https://ipinfo.io/ip.
        /// </summary>
        public async Task<HttpRestClientResponse<string>> ReadIPAddressAsync() => await _httpRestClientHandler.HandleAsync(new GetIP()).ConfigureAwait(false);

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
