
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
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Http.Network
{
    /// <summary>
    /// Represents a query to request IP Address Geo-location.
    /// </summary>
    public class LocationRequest : IQuery<GeoLocation>, IQueryStringLocationRequest, IHttpRestClientAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationRequest"/> class with IP address to locate.
        /// </summary>
        /// <param name="ipAddress">The target IP address.</param>
        /// <param name="accessKey">The security access key.</param>
        /// <param name="enableSecurity">Enable the security mode = 1 for professional subscription.</param>
        /// <param name="enableHostName">Enable host-name = 1</param>
        /// <param name="output">The output format, the default is <see cref="LocationOutput.Json"/>.</param>
        /// <param name="language">The output language, the default is <see cref="LocationLanguage.EnglishUS"/></param>
        /// <exception cref="ArgumentNullException">The <paramref name="ipAddress"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="accessKey"/> is null.</exception>
        public LocationRequest(
            string ipAddress,
            string accessKey,
            int enableSecurity = 0,
            int enableHostName = 0,
            LocationOutput? output = default,
            LocationLanguage? language = default)
        {
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            EnableSecurity = enableSecurity;
            EnableHostName = enableHostName;
            Output = output ?? LocationOutput.Json;
            Language = language ?? LocationLanguage.EnglishUS;
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
        public LocationOutput Output { get; } = LocationOutput.Json;

        /// <summary>
        /// Gets the request language.
        /// </summary>
        public LocationLanguage Language { get; } = LocationLanguage.EnglishUS;

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
    public sealed class LocationOutput : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LocationOutput" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private LocationOutput(string displayName, int value)
            : base(displayName, value) { }

        /// <summary>
        /// The XML output.
        /// </summary>
        public static LocationOutput Xml => new LocationOutput("xml", 0);

        /// <summary>
        /// The JSON output.
        /// </summary>
        public static LocationOutput Json => new LocationOutput("json", 1);
    }

    /// <summary>
    /// The location language.
    /// </summary>
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public sealed class LocationLanguage : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LocationLanguage" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="language">The language name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private LocationLanguage(string displayName, string language, int value)
            : base(displayName, value) => Language = language;

        /// <summary>
        /// Gets the language name.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The English language.
        /// </summary>
        public static LocationLanguage EnglishUS => new LocationLanguage("en", nameof(EnglishUS), 0);

        /// <summary>
        /// The Germain language.
        /// </summary>
        public static LocationLanguage Germain => new LocationLanguage("de", nameof(Germain), 1);

        /// <summary>
        /// The Spanish language.
        /// </summary>
        public static LocationLanguage Spanish => new LocationLanguage("es", nameof(Spanish), 2);

        /// <summary>
        /// The French language.
        /// </summary>
        public static LocationLanguage French => new LocationLanguage("fr", nameof(French), 3);

        /// <summary>
        /// The Japanese language.
        /// </summary>
        public static LocationLanguage Japanese => new LocationLanguage("ja", nameof(Japanese), 4);

        /// <summary>
        /// The Portuguese Brazil language.
        /// </summary>
        public static LocationLanguage PortugeseBrazil => new LocationLanguage("pt-br", nameof(PortugeseBrazil), 5);

        /// <summary>
        /// The Russian language.
        /// </summary>
        public static LocationLanguage Russian => new LocationLanguage("ru", nameof(Russian), 6);

        /// <summary>
        /// The Chinese language.
        /// </summary>
        public static LocationLanguage Chinese => new LocationLanguage("zh", nameof(Chinese), 7);
    }

    /// <summary>
    /// Defines the IP Geo-location response.
    /// </summary>
    public sealed class GeoLocation
    {
        /// <summary>
        /// Returns the requested IP address.
        /// </summary>        
        public string? Ip { get; set; }

        /// <summary>
        /// Returns the host-name the requested IP resolves to, only returned if Host-name Lookup is enabled.
        /// </summary>
        public string? HostName { get; set; }

        /// <summary>
        /// Returns the IP address type IPv4 or IPv6.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Returns the 2-letter continent code associated with the IP.
        /// </summary>
        public string? Continent_Code { get; set; }

        /// <summary>
        /// Returns the name of the continent associated with the IP.
        /// </summary>
        public string? Continent_Name { get; set; }

        /// <summary>
        /// Returns the 2-letter country code associated with the IP.
        /// </summary>
        public string? Country_Code { get; set; }

        /// <summary>
        /// Returns the name of the country associated with the IP.
        /// </summary>
        public string? Country_Name { get; set; }

        /// <summary>
        /// Returns the region code of the region associated with the IP (e.g. CA for California).
        /// </summary>
        public string? Region_Code { get; set; }

        /// <summary>
        /// Returns the name of the region associated with the IP.
        /// </summary>
        public string? Region_Name { get; set; }

        /// <summary>
        /// Returns the name of the city associated with the IP.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Returns the ZIP code associated with the IP.
        /// </summary>
        public string? Zip { get; set; }

        /// <summary>
        /// Returns the latitude value associated with the IP.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Returns the longitude value associated with the IP.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///  Returns multiple location-related objects
        /// </summary>
        public IPLocation? Location { get; set; }

        /// <summary>
        /// Returns an object containing timezone-related data.
        /// </summary>
        public IPTimeZone? Time_Zone { get; set; }

        /// <summary>
        /// Returns an object containing currency-related data.
        /// </summary>
        public IPCurrency? Currency { get; set; }

        /// <summary>
        /// Returns an object containing connection-related data.
        /// </summary>
        public IPConnection? Connection { get; set; }

        /// <summary>
        /// Returns an object containing security-related data.
        /// </summary>
        public IPSecurity? Security { get; set; }
    }

    /// <summary>
    /// The location.
    /// </summary>
    public sealed class IPLocation
    {
        /// <summary>
        /// Returns the unique geoname identifier in accordance with the Geonames Registry.
        /// </summary>
        public int GeoName_Id { get; set; }

        /// <summary>
        /// Returns the capital city of the country associated with the IP.
        /// </summary>
        public string? Capital { get; set; }

        /// <summary>
        /// Returns an object containing one or multiple sub-objects per language spoken in the country associated with the IP.
        /// </summary>
        public IPLanguage[]? Languages { get; set; }

        /// <summary>
        /// Returns an HTTP URL leading to an SVG-flag icon for the country associated with the IP.
        /// </summary>
        public string? Country_Flag { get; set; }

        /// <summary>
        /// Returns the emoji icon for the flag of the country associated with the IP.
        /// </summary>
        public string? Country_Flag_Emoji { get; set; }

        /// <summary>
        /// Returns the unicode value of the emoji icon for the flag of the country associated with the IP. (e.g. U+1F1F5 U+1F1F9 for the Portuguese flag)
        /// </summary>
        public string? Country_Flag_Emoji_Unicode { get; set; }

        /// <summary>
        /// Returns the calling/dial code of the country associated with the IP. (e.g. 351) for Portugal.
        /// </summary>
        public string? Calling_Code { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the county associated with the IP is in the European Union.
        /// </summary>
        public bool Is_EU { get; set; }
    }

    /// <summary>
    /// The location language.
    /// </summary>
    public sealed class IPLanguage
    {
        /// <summary>
        /// Returns the 2-letter language code for the given language.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Returns the name (in the API request's main language) of the given language. (e.g. Portuguese)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Returns the native name of the given language. (e.g. Português)
        /// </summary>
        public string? Native { get; set; }
    }

    /// <summary>
    /// The time zone location.
    /// </summary>
    public sealed class IPTimeZone
    {
        /// <summary>
        /// Returns the ID of the time zone associated with the IP. (e.g. America/Los_Angeles for PST)
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Returns the current date and time in the location associated with the IP. (e.g. 2018-03-29T22:31:27-07:00)
        /// </summary>
        public DateTime Current_Time { get; set; }

        /// <summary>
        /// Returns the GMT offset of the given time zone in seconds. (e.g. -25200 for PST's -7h GMT offset)
        /// </summary>
        public int Gmt_Offset { get; set; }

        /// <summary>
        /// Returns the universal code of the given time zone.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given time zone is considered daylight saving time.
        /// </summary>
        public bool Is_Daylight_Saving { get; set; }
    }

    /// <summary>
    /// The currency location.
    /// </summary>
    public sealed class IPCurrency
    {
        /// <summary>
        /// Returns the 3-letter code of the main currency associated with the IP.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Returns the name of the given currency.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Returns the plural name of the given currency.
        /// </summary>
        public string? Plural { get; set; }

        /// <summary>
        /// Returns the symbol letter of the given currency.
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Returns the native symbol letter of the given currency.
        /// </summary>
        public string? Symbol_Native { get; set; }
    }

    /// <summary>
    /// The connexion location.
    /// </summary>
    public sealed class IPConnection
    {
        /// <summary>
        /// Returns the Autonomous System Number associated with the IP.
        /// </summary>
        public int ASN { get; set; }

        /// <summary>
        /// Returns the name of the ISP associated with the IP.
        /// </summary>
        public string? ISP { get; set; }
    }

    /// <summary>
    /// The security location
    /// </summary>
    public sealed class IPSecurity
    {
        /// <summary>
        /// Returns true or false depending on whether or not the given IP is associated with a proxy.
        /// </summary>
        public bool Is_Proxy { get; set; }

        /// <summary>
        /// Returns the type of proxy the IP is associated with.
        /// </summary>
        public string? Proxy_Type { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given IP is associated with a crawler.
        /// </summary>
        public bool Is_Crawler { get; set; }

        /// <summary>
        /// Returns the name of the crawler the IP is associated with.
        /// </summary>
        public string? Crawler_Name { get; set; }

        /// <summary>
        /// Returns the type of crawler the IP is associated with.
        /// </summary>
        public string? Crawler_Type { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given IP is associated with the anonymous Tor system.
        /// </summary>
        public bool Is_Tor { get; set; }

        /// <summary>
        /// Returns the type of threat level the IP is associated with.
        /// </summary>
        public string? Threat_Level { get; set; }

        /// <summary>
        /// Returns an object containing all threat types associated with the IP.
        /// </summary>
        public object? Threat_Types { get; set; }
    }
}