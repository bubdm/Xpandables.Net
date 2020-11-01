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

using Xpandables.Net.HttpRest;

namespace Xpandables.Net.Http.Network
{
    /// <summary>
    /// Default implementation for <see cref="IHttpLocationAccessor"/>.
    /// </summary>
    public sealed class HttpLocationAccessor : Disposable, IHttpLocationAccessor
    {
        private readonly IHttpRestClientHandler _httpRestClientHandler;
        IHttpRestClientHandler IHttpLocationAccessor.HttpRestClientHandler => _httpRestClientHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLocationAccessor"/> class that uses the http://api.ipstack.com to retrieve the user location.
        /// </summary>
        public HttpLocationAccessor()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("http://api.ipstack.com") };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            _httpRestClientHandler = new HttpRestClientHandler(httpClient);
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