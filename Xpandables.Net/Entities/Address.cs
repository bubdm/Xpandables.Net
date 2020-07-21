
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
using System.ComponentModel.DataAnnotations;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Defines an <see cref="Address"/> class that holds basic properties.
    /// </summary>
    public sealed class Address : ValueObject
    {
        /// <summary>
        /// Gets the street name.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string Street { get; private set; }

        /// <summary>
        /// Gets the city name.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string City { get; private set; }

        /// <summary>
        /// Gets the street number.
        /// </summary>
        public string StreetNumber { get; private set; }

        /// <summary>
        /// Gets the country name.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string Country { get; private set; }

        /// <summary>
        /// Gets the zip code.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string ZipCode { get; private set; }

        /// <summary>
        /// Creates a string representation of the string <see cref="Address"/> using the specified format and provider.
        /// The format will received address properties in the following order : <see cref="Street"/>, <see cref="StreetNumber"/>, <see cref="City"/>, <see cref="ZipCode"/>
        /// and <see cref="Country"/>.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Street, StreetNumber, City, ZipCode, Country);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="street"></param>
        /// <param name="city"></param>
        /// <param name="streetNumber"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public static Address Create(string street, string city, string streetNumber, string country, string zipCode)
            => new Address(street, city, streetNumber, country, zipCode);

        /// <summary>
        /// Updates the street name and return the current instance.
        /// </summary>
        /// <param name="street">The new street name.</param>
        public Address EditStreet(string street)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            return this;
        }

        /// <summary>
        /// Updates the city name and return the current instance.        
        /// </summary>
        /// <param name="city">The new city name.</param>
        public Address EditCity(string city)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            return this;
        }

        /// <summary>
        /// Updates the street number and return the current instance.
        /// </summary>
        /// <param name="streetNumber">The new street number.</param>
        public Address EditStreetNumber(string streetNumber)
        {
            StreetNumber = streetNumber;
            return this;
        }

        /// <summary>
        /// Updates the country name and return the current instance.
        /// </summary>
        /// <param name="country">The new country name.</param>
        public Address EditCountry(string country)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            return this;
        }

        /// <summary>
        /// Updates the zip code name and return the current instance.
        /// </summary>
        /// <param name="zipCode">The new zip code</param>
        public Address EditZipCode(string zipCode)
        {
            ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
            return this;
        }

        private Address(string street, string city, string streetNumber, string country, string zipCode)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            City = city ?? throw new ArgumentNullException(nameof(city));
            StreetNumber = streetNumber;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        }

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return StreetNumber ?? string.Empty;
            yield return Country;
            yield return ZipCode;
        }
    }
}
