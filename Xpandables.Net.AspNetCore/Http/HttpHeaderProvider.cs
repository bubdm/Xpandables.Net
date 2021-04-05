
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
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Default implementation of <see cref="IHttpHeaderProvider"/> that uses an <see cref="IHeaderDictionary"/> to access header information.
    /// </summary>
    public class HttpHeaderProvider : IHttpHeaderProvider
    {
        private readonly IHeaderDictionary _headerDictionary;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpHeaderProvider"/>.
        /// </summary>
        /// <param name="headerDictionary">The header dictionary to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="headerDictionary"/> is null.</exception>
        public HttpHeaderProvider(IHeaderDictionary headerDictionary)
        {
            _headerDictionary = headerDictionary ?? throw new ArgumentNullException(nameof(headerDictionary));
        }

        /// <summary>
        /// Gets all HTTP header values from the current HTTP context.
        /// If not found, returns an empty dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> ReadValues()
        {
            if (_headerDictionary.Count > 0)
                return _headerDictionary.ToDictionary(d => d.Key, d => (IReadOnlyList<string>)d.Value);

            return ImmutableDictionary<string, IReadOnlyList<string>>.Empty;
        }

        /// <summary>
        /// Sets the HTTP header value for the current HTTP context matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <param name="value">The value to be set.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value" /> is null.</exception>
        public void WriteValue(string key, string value) => _headerDictionary.Add(key, value);

        /// <summary>
        /// Sets a collection of header values for the current HTTP content matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="values">The collection of values.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="values" /> is null.</exception>
        public void WriteValues(string key, string[] values) => _headerDictionary.Add(key, values);
    }
}
