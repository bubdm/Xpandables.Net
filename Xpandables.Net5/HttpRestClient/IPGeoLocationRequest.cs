
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
using System.Collections.Generic;
using System.Design;

namespace System
{
    /// <summary>
    /// The IPAddress Geo-location request.
    /// </summary>
    public class IPGeoLocationRequest : IQuery<IPGeoLocationResponse>, IQueryStringRequest, IHttpRestClientAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IPGeoLocationRequest"/> class.
        /// </summary>
        /// <param name="ipAddress">The target IP address.</param>
        /// <param name="accessKey">The security access key.</param>
        /// <param name="enableSecurity">Enable the security mode = 1 for professional subscription.</param>
        /// <param name="enableHostName">Enable host-name = 1</param>
        /// <param name="output">The output format, the default is <see cref="IPGeoLocationOutput.Json"/>.</param>
        /// <param name="language">The output language, the default is <see cref="IPGeoLocationLanguage.EnglishUS"/></param>
        /// <exception cref="ArgumentNullException">The <paramref name="ipAddress"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="accessKey"/> is null.</exception>
        public IPGeoLocationRequest(
            string ipAddress,
            string accessKey,
            int enableSecurity = 0,
            int enableHostName = 0,
            IPGeoLocationOutput? output = default,
            IPGeoLocationLanguage? language = default)
        {
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            EnableSecurity = enableSecurity;
            EnableHostName = enableHostName;
            Output = output ?? IPGeoLocationOutput.Json;
            Language = language ?? IPGeoLocationLanguage.EnglishUS;
        }

        /// <summary>
        /// Gets the IPAddress to search for location.
        /// </summary>
        public string IpAddress { get; }

        /// <summary>
        /// Gets the access key to be used.
        /// </summary>
        public string AccessKey { get; }

        /// <summary>
        /// Gets the security value.
        /// </summary>
        public int EnableSecurity { get; } = 0;

        /// <summary>
        /// Determine whether or not to use host-name.
        /// </summary>
        public int EnableHostName { get; } = 0;

        /// <summary>
        /// Gets the request output format.
        /// </summary>
        public IPGeoLocationOutput Output { get; } = IPGeoLocationOutput.Json;

        /// <summary>
        /// Gets the request language.
        /// </summary>
        public IPGeoLocationLanguage Language { get; } = IPGeoLocationLanguage.EnglishUS;

        /// <summary>
        /// Returns the <see cref="HttpRestClientAttribute" /> to be applied on the current instance.
        /// </summary>
        public HttpRestClientAttribute GetHttpRestClientAttribute()
            => new HttpRestClientAttribute { Path = IpAddress, IsNullable = true, IsSecured = false };

        /// <summary>
        /// Returns the keys and values for the Uri.
        /// </summary>
        public IDictionary<string, string> GetQueryString()
            => new Dictionary<string, string>(
                new[]
                {
                    new KeyValuePair<string, string>("access_key", AccessKey),
                    new KeyValuePair<string, string>("output", Output),
                    new KeyValuePair<string, string>("language", Language),
                    new KeyValuePair<string, string>("hostname", EnableHostName.ToString()),
                    new KeyValuePair<string, string>("security", EnableSecurity.ToString())
                });
    }
}