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

using Xpandables.Net5.Enumerations;

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// The location language.
    /// </summary>
    public sealed class IPGeoLocationLanguage : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IPGeoLocationLanguage" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="language">The language name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private IPGeoLocationLanguage(string displayName, string language, int value)
            : base(displayName, value)
        {
            Language = language;
        }

        /// <summary>
        /// Gets the language name.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The English language.
        /// </summary>
        public static IPGeoLocationLanguage EnglishUS => new IPGeoLocationLanguage("en", nameof(EnglishUS), 0);

        /// <summary>
        /// The Germain language.
        /// </summary>
        public static IPGeoLocationLanguage Germain => new IPGeoLocationLanguage("de", nameof(Germain), 1);

        /// <summary>
        /// The Spanish language.
        /// </summary>
        public static IPGeoLocationLanguage Spanish => new IPGeoLocationLanguage("es", nameof(Spanish), 2);

        /// <summary>
        /// The French language.
        /// </summary>
        public static IPGeoLocationLanguage French => new IPGeoLocationLanguage("fr", nameof(French), 3);

        /// <summary>
        /// The Japanese language.
        /// </summary>
        public static IPGeoLocationLanguage Japanese => new IPGeoLocationLanguage("ja", nameof(Japanese), 4);

        /// <summary>
        /// The Portuguese Brazil language.
        /// </summary>
        public static IPGeoLocationLanguage PortugeseBrazil => new IPGeoLocationLanguage("pt-br", nameof(PortugeseBrazil), 5);

        /// <summary>
        /// The Russian language.
        /// </summary>
        public static IPGeoLocationLanguage Russian => new IPGeoLocationLanguage("ru", nameof(Russian), 6);

        /// <summary>
        /// The Chinese language.
        /// </summary>
        public static IPGeoLocationLanguage Chinese => new IPGeoLocationLanguage("zh", nameof(Chinese), 7);
    }
}
