
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

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client IP Geo-location.
    /// </summary>
    public interface IHttpRestClientIPGeoLocationHandler
    {
        /// <summary>
        /// The IP Address HTTP client name.
        /// </summary>
        public const string IPGeoAddressHttpClientName = "IPGeoAddress";

        /// <summary>
        /// The IP Location HTTP client name.
        /// </summary>
        public const string IPGeoLocationHttpClientName = "IPGeoLocation";

        /// <summary>
        /// Asynchronously gets the IPAddress of the current caller using https://ipinfo.io/ip.
        /// </summary>
        Task<HttpRestClientResponse<string>> GetIPAddressAsync();

        /// <summary>
        /// Asynchronously gets the IPAddress Geo-location of the specified IPAddress request using http://api.ipstack.com.
        /// </summary>
        /// <param name="request">The request to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        Task<HttpRestClientResponse<IPGeoLocationResponse>> GetIPGeoLocationAsync(IPGeoLocationRequest request);
    }
}
