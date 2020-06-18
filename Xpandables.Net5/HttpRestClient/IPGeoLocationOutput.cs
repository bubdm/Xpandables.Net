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

namespace System
{
    /// <summary>
    /// The location output format.
    /// </summary>
    public sealed class IPGeoLocationOutput : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IPGeoLocationOutput" /> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName" /> is null.</exception>
        private IPGeoLocationOutput(string displayName, int value)
            : base(displayName, value) { }

        /// <summary>
        /// The Xml output.
        /// </summary>
        public static IPGeoLocationOutput Xml => new IPGeoLocationOutput("xml", 0);

        /// <summary>
        /// The Json output.
        /// </summary>
        public static IPGeoLocationOutput Json => new IPGeoLocationOutput("json", 1);
    }
}
