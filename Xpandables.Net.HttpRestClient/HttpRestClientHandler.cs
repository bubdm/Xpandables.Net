
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

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IHttpRestClientHandler"/> interface.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        /// <summary>
        /// Gets the <see cref="HttpClient"/> current instance.
        /// </summary>
        protected HttpClient HttpClientInstance { get; }

        /// <summary>
        /// Contains the <see cref="HttpContent"/> instance for the current handler.
        /// </summary>
#pragma warning disable CA1033 // Interface methods should be callable by child types
        HttpClient IHttpRestClientHandler.HttpClient => HttpClientInstance;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientHandler(HttpClient httpClient)
            => HttpClientInstance = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        private bool _isDisposed;

        /// <summary>
        /// Disposes the HTTP client instance.
        /// </summary>
        /// <param name="disposing">Determine whether the dispose has already been called.</param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                HttpClientInstance?.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
