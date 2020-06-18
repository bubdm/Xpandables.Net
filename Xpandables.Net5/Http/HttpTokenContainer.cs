
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
using System.Security.Claims;

namespace System.Design.Http
{
    /// <summary>
    /// Provides with <see cref="IHttpTokenEngine"/>, <see cref="IHttpTokenAccessor"/> and <see cref="IHttpHeaderAccessor"/>.
    /// </summary>
    public sealed class HttpTokenContainer : IHttpTokenAccessor, IHttpTokenEngine, IHttpHeaderAccessor
    {
        private readonly IHttpTokenEngine _httpTokenEngine;
        private readonly IHttpTokenAccessor _httpTokenAccessor;
        private readonly IHttpHeaderAccessor _httpHeaderAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpTokenContainer"/> class.
        /// </summary>
        /// <param name="httpTokenEngine">The token engine.</param>
        /// <param name="httpTokenAccessor">The token accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpTokenAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpTokenEngine"/> is null.</exception>
        public HttpTokenContainer(IHttpTokenEngine httpTokenEngine, IHttpTokenAccessor httpTokenAccessor, IHttpHeaderAccessor httpHeaderAccessor)
        {
            _httpTokenEngine = httpTokenEngine ?? throw new ArgumentNullException(nameof(httpTokenEngine));
            _httpTokenAccessor = httpTokenAccessor ?? throw new ArgumentNullException(nameof(httpTokenAccessor));
            _httpHeaderAccessor = httpHeaderAccessor ?? throw new ArgumentNullException(nameof(httpHeaderAccessor));
        }

        /// <summary>
        /// Uses the collection of claims to build a string token.
        /// </summary>
        /// <param name="claims">collection of claims to be used to build token string.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="claims" /> is null.</exception>
        public string BuildToken(IEnumerable<Claim> claims) => _httpTokenEngine.BuildToken(claims);

        /// <summary>
        /// Uses the source object to build a string token.
        /// </summary>
        /// <param name="source">The source to be used.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> is null.</exception>
        public string BuildToken(object source) => _httpTokenEngine.BuildToken(source);

        /// <summary>
        /// Returns the collection of claims from the specified token.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>An collection of claims if OK or an empty type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token" /> is null.</exception>
        public IEnumerable<Claim> GetClaims(string token) => _httpTokenEngine.GetClaims(token);

        /// <summary>
        /// Returns the current token value from the current HTTP request with the specified key.
        /// If not found, returns an empty value.
        /// </summary>
        /// <param name="key">The token key to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        public string? GetToken(string key) => _httpTokenAccessor.GetToken(key);

        /// <summary>
        /// Gets all HTTP header values from the current HTTP request.
        /// If not found, returns an empty dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetValues() => _httpHeaderAccessor.GetValues();

        /// <summary>
        /// Gets the HTTP header value from the current HTTP request matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public string? GetValue(string key) => _httpHeaderAccessor.GetValue(key);

        /// <summary>
        /// Gets all HTTP header values from the current HTTP request matching the specified key.
        /// If not found, returns an empty enumerable.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public IEnumerable<string> GetValues(string key) => _httpHeaderAccessor.GetValues(key);
    }
}
