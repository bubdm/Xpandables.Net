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

using Newtonsoft.Json;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Defines the IP Geo-location response.
    /// </summary>
    public sealed class GeoLocationResponse
    {
        /// <summary>
        /// Returns the requested IP address.
        /// </summary>
        [JsonProperty("ip")]
        public string? Ip { get; set; }

        /// <summary>
        /// Returns the host-name the requested IP resolves to, only returned if Host-name Lookup is enabled.
        /// </summary>
        [JsonProperty("hostname")]
        public string? HostName { get; set; }

        /// <summary>
        /// Returns the IP address type IPv4 or IPv6.
        /// </summary>
        [JsonProperty("type")]
        public string? IPType { get; set; }

        /// <summary>
        /// Returns the 2-letter continent code associated with the IP.
        /// </summary>
        [JsonProperty("continent_code")]
        public string? ContinentCode { get; set; }

        /// <summary>
        /// Returns the name of the continent associated with the IP.
        /// </summary>
        [JsonProperty("continent_name")]
        public string? ContinentName { get; set; }

        /// <summary>
        /// Returns the 2-letter country code associated with the IP.
        /// </summary>
        [JsonProperty("country_code")]
        public string? CountryCode { get; set; }

        /// <summary>
        /// Returns the name of the country associated with the IP.
        /// </summary>
        [JsonProperty("country_name")]
        public string? CountryName { get; set; }

        /// <summary>
        /// Returns the region code of the region associated with the IP (e.g. CA for California).
        /// </summary>
        [JsonProperty("region_code")]
        public string? RegionCode { get; set; }

        /// <summary>
        /// Returns the name of the region associated with the IP.
        /// </summary>
        [JsonProperty("region_name")]
        public string? RegionName { get; set; }

        /// <summary>
        /// Returns the name of the city associated with the IP.
        /// </summary>
        [JsonProperty("city")]
        public string? City { get; set; }

        /// <summary>
        /// Returns the ZIP code associated with the IP.
        /// </summary>
        [JsonProperty("zip")]
        public string? ZipCode { get; set; }

        /// <summary>
        /// Returns the latitude value associated with the IP.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Returns the longitude value associated with the IP.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        ///  Returns multiple location-related objects
        /// </summary>
        [JsonProperty("location")]
        public IPLocation? Location { get; set; }

        /// <summary>
        /// Returns an object containing timezone-related data.
        /// </summary>
        [JsonProperty("time_zone")]
        public IPTimeZone? TimeZone { get; set; }

        /// <summary>
        /// Returns an object containing currency-related data.
        /// </summary>
        [JsonProperty("currency")]
        public IPCurrency? Currency { get; set; }

        /// <summary>
        /// Returns an object containing connection-related data.
        /// </summary>
        [JsonProperty("connection")]
        public IPConnection? Connection { get; set; }

        /// <summary>
        /// Returns an object containing security-related data.
        /// </summary>
        [JsonProperty("security")]
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
        [JsonProperty("geoname_id")]
        public int GeoNameID { get; set; }

        /// <summary>
        /// Returns the capital city of the country associated with the IP.
        /// </summary>
        [JsonProperty("capital")]
        public string? Capital { get; set; }

        /// <summary>
        /// Returns an object containing one or multiple sub-objects per language spoken in the country associated with the IP.
        /// </summary>
        [JsonProperty("languages")]
#pragma warning disable CA1819 // Properties should not return arrays
        public IPLanguage[]? Languages { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Returns an HTTP URL leading to an SVG-flag icon for the country associated with the IP.
        /// </summary>
        [JsonProperty("country_flag")]
#pragma warning disable CA1056 // Uri properties should not be strings
        public string? CountryFlagUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Returns the emoji icon for the flag of the country associated with the IP.
        /// </summary>
        [JsonProperty("country_flag_emoji")]
        public string? CountryFlagEmoji { get; set; }

        /// <summary>
        /// Returns the unicode value of the emoji icon for the flag of the country associated with the IP. (e.g. U+1F1F5 U+1F1F9 for the Portuguese flag)
        /// </summary>
        [JsonProperty("country_flag_emoji_unicode")]
        public string? CountryFlagEmojiUnicode { get; set; }

        /// <summary>
        /// Returns the calling/dial code of the country associated with the IP. (e.g. 351) for Portugal.
        /// </summary>
        [JsonProperty("calling_code")]
        public string? CallingCode { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the county associated with the IP is in the European Union.
        /// </summary>
        [JsonProperty("is_eu")]
        public bool IsEU { get; set; }
    }

    /// <summary>
    /// The location language.
    /// </summary>
    public sealed class IPLanguage
    {
        /// <summary>
        /// Returns the 2-letter language code for the given language.
        /// </summary>
        [JsonProperty("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Returns the name (in the API request's main language) of the given language. (e.g. Portuguese)
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Returns the native name of the given language. (e.g. Português)
        /// </summary>
        [JsonProperty("native")]
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
        [JsonProperty("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Returns the current date and time in the location associated with the IP. (e.g. 2018-03-29T22:31:27-07:00)
        /// </summary>
        [JsonProperty("current_time")]
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// Returns the GMT offset of the given time zone in seconds. (e.g. -25200 for PST's -7h GMT offset)
        /// </summary>
        [JsonProperty("gmt_offset")]
        public int GMTOffset { get; set; }

        /// <summary>
        /// Returns the universal code of the given time zone.
        /// </summary>
        [JsonProperty("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given time zone is considered daylight saving time.
        /// </summary>
        [JsonProperty("is_daylight_saving")]
        public bool IsDaylightSaving { get; set; }
    }

    /// <summary>
    /// The currency location.
    /// </summary>
    public sealed class IPCurrency
    {
        /// <summary>
        /// Returns the 3-letter code of the main currency associated with the IP.
        /// </summary>
        [JsonProperty("Code")]
        public string? Code { get; set; }

        /// <summary>
        /// Returns the name of the given currency.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Returns the plural name of the given currency.
        /// </summary>
        [JsonProperty("plural")]
        public string? Plural { get; set; }

        /// <summary>
        /// Returns the symbol letter of the given currency.
        /// </summary>
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }

        /// <summary>
        /// Returns the native symbol letter of the given currency.
        /// </summary>
        [JsonProperty("symbol_native")]
        public string? SymbolNative { get; set; }
    }

    /// <summary>
    /// The connexion location.
    /// </summary>
    public sealed class IPConnection
    {
        /// <summary>
        /// Returns the Autonomous System Number associated with the IP.
        /// </summary>
        [JsonProperty("asn")]
        public int ASN { get; set; }

        /// <summary>
        /// Returns the name of the ISP associated with the IP.
        /// </summary>
        [JsonProperty("isp")]
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
        [JsonProperty("is_proxy")]
        public bool IsProxy { get; set; }

        /// <summary>
        /// Returns the type of proxy the IP is associated with.
        /// </summary>
        [JsonProperty("proxy_type")]
        public string? ProxyType { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given IP is associated with a crawler.
        /// </summary>
        [JsonProperty("is_crawler")]
        public bool IsCrawler { get; set; }

        /// <summary>
        /// Returns the name of the crawler the IP is associated with.
        /// </summary>
        [JsonProperty("crawler_name")]
        public string? CrawlerName { get; set; }

        /// <summary>
        /// Returns the type of crawler the IP is associated with.
        /// </summary>
        [JsonProperty("crawler_type")]
        public string? CrawlerType { get; set; }

        /// <summary>
        /// Returns true or false depending on whether or not the given IP is associated with the anonymous Tor system.
        /// </summary>
        [JsonProperty("is_tor")]
        public bool IsTor { get; set; }

        /// <summary>
        /// Returns the type of threat level the IP is associated with.
        /// </summary>
        [JsonProperty("threat_level")]
        public string? ThreatLevel { get; set; }

        /// <summary>
        /// Returns an object containing all threat types associated with the IP.
        /// </summary>
        [JsonProperty("threat_types")]
        public object? ThreatTypes { get; set; }
    }
}