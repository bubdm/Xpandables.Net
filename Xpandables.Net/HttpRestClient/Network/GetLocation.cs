
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Xpandables.Net.Enumerations;
using Xpandables.Net.Queries;

namespace Xpandables.Net.HttpRestClient.Network
{
    /// <summary>
    /// The IPAddress Geo-location request.
    /// </summary>
    public class GetLocation : IQuery<GeoLocation>, IQueryStringLocationRequest, IHttpRestClientAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GetLocation"/> class.
        /// </summary>
        /// <param name="ipAddress">The target IP address.</param>
        /// <param name="accessKey">The security access key.</param>
        /// <param name="enableSecurity">Enable the security mode = 1 for professional subscription.</param>
        /// <param name="enableHostName">Enable host-name = 1</param>
        /// <param name="output">The output format, the default is <see cref="GeoLocationOutput.Json"/>.</param>
        /// <param name="language">The output language, the default is <see cref="GeoLocationLanguage.EnglishUS"/></param>
        /// <exception cref="ArgumentNullException">The <paramref name="ipAddress"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="accessKey"/> is null.</exception>
        public GetLocation(
            string ipAddress,
            string accessKey,
            int enableSecurity = 0,
            int enableHostName = 0,
            GeoLocationOutput? output = default,
            GeoLocationLanguage? language = default)
        {
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            EnableSecurity = enableSecurity;
            EnableHostName = enableHostName;
            Output = output ?? GeoLocationOutput.Json;
            Language = language ?? GeoLocationLanguage.EnglishUS;
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
        public int EnableSecurity { get; }

        /// <summary>
        /// Determine whether or not to use host-name.
        /// </summary>
        public int EnableHostName { get; }

        /// <summary>
        /// Gets the request output format.
        /// </summary>
        public GeoLocationOutput Output { get; } = GeoLocationOutput.Json;

        /// <summary>
        /// Gets the request language.
        /// </summary>
        public GeoLocationLanguage Language { get; } = GeoLocationLanguage.EnglishUS;

        /// <summary>
        /// Returns the <see cref="HttpRestClientAttribute" /> to be applied on the current instance.
        /// </summary>
        public HttpRestClientAttribute ReadHttpRestClientAttribute()
            => new HttpRestClientAttribute { Path = IpAddress, IsNullable = true, IsSecured = false, In = ParameterLocation.Query };

        /// <summary>
        /// Returns the keys and values for the Uri.
        /// </summary>
        public IDictionary<string, string?> GetQueryStringSource()
            => new Dictionary<string, string?>(
                new[]
                {
                    new KeyValuePair<string, string?>("access_key", AccessKey),
                    new KeyValuePair<string, string?>("output", Output),
                    new KeyValuePair<string, string?>("language", Language),
                    new KeyValuePair<string, string?>("hostname", EnableHostName.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string?>("security", EnableSecurity.ToString(CultureInfo.InvariantCulture))
                });
    }

    /// <summary>
    /// The location output format.
    /// </summary>
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public sealed class GeoLocationOutput : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GeoLocationOutput" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private GeoLocationOutput(string displayName, int value)
            : base(displayName, value) { }

        /// <summary>
        /// The XML output.
        /// </summary>
        public static GeoLocationOutput Xml => new GeoLocationOutput("xml", 0);

        /// <summary>
        /// The JSON output.
        /// </summary>
        public static GeoLocationOutput Json => new GeoLocationOutput("json", 1);
    }

    /// <summary>
    /// The location language.
    /// </summary>
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public sealed class GeoLocationLanguage : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GeoLocationLanguage" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="language">The language name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private GeoLocationLanguage(string displayName, string language, int value)
            : base(displayName, value) => Language = language;

        /// <summary>
        /// Gets the language name.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The English language.
        /// </summary>
        public static GeoLocationLanguage EnglishUS => new GeoLocationLanguage("en", nameof(EnglishUS), 0);

        /// <summary>
        /// The Germain language.
        /// </summary>
        public static GeoLocationLanguage Germain => new GeoLocationLanguage("de", nameof(Germain), 1);

        /// <summary>
        /// The Spanish language.
        /// </summary>
        public static GeoLocationLanguage Spanish => new GeoLocationLanguage("es", nameof(Spanish), 2);

        /// <summary>
        /// The French language.
        /// </summary>
        public static GeoLocationLanguage French => new GeoLocationLanguage("fr", nameof(French), 3);

        /// <summary>
        /// The Japanese language.
        /// </summary>
        public static GeoLocationLanguage Japanese => new GeoLocationLanguage("ja", nameof(Japanese), 4);

        /// <summary>
        /// The Portuguese Brazil language.
        /// </summary>
        public static GeoLocationLanguage PortugeseBrazil => new GeoLocationLanguage("pt-br", nameof(PortugeseBrazil), 5);

        /// <summary>
        /// The Russian language.
        /// </summary>
        public static GeoLocationLanguage Russian => new GeoLocationLanguage("ru", nameof(Russian), 6);

        /// <summary>
        /// The Chinese language.
        /// </summary>
        public static GeoLocationLanguage Chinese => new GeoLocationLanguage("zh", nameof(Chinese), 7);
    }
}