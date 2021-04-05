
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
using System.Linq;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with methods to access headers information.
    /// </summary>
    public interface IHttpHeaderProvider
    {
        /// <summary>
        /// Gets the HTTP header value from the current HTTP context matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public sealed string? ReadValue(string key) => ReadValues(key).FirstOrDefault();

        /// <summary>
        /// Gets all HTTP header values from the current HTTP context matching the specified key.
        /// If not found, returns an empty enumerable.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public sealed IEnumerable<string> ReadValues(string key)
            => ReadValues().TryGetValue(key, out var values) ? values.AsEnumerable() : Enumerable.Empty<string>();

        /// <summary>
        /// Gets all HTTP header values from the current HTTP context.
        /// If not found, returns an empty dictionary.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<string>> ReadValues();

        /// <summary>
        /// Sets the HTTP header value for the current HTTP context matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <param name="value">The value to be set.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        void WriteValue(string key, string value);

        /// <summary>
        /// Sets a collection of header values for the current HTTP content matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="values">The collection of values.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="values"/> is null.</exception>
        void WriteValues(string key, string[] values);
    }
}
