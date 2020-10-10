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

namespace Xpandables.Net.HttpRestClient.Network
{
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