
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
using System.Threading.Tasks;

using Xpandables.Net.HttpRest;

namespace Xpandables.Net.Http.Network
{
    /// <summary>
    /// Provides with a method to request Geo-location using a typed client HTTP Client.
    /// </summary>
    public interface IHttpLocationAccessor : IDisposable
    {
        internal IHttpRestClientHandler HttpRestClientHandler { get; }

        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request using http://api.ipstack.com.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse<GeoLocation>> ReadLocationAsync(LocationRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return await HttpRestClientHandler.HandleAsync(request).ConfigureAwait(false);
        }
    }
}
