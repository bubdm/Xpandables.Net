
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

namespace Xpandables.Net.Http
{
    /// <summary>
    /// The default implementation for <see cref="IHttpTokenAccessor"/>.
    /// </summary>
    public sealed class HttpTokenAccessor : IHttpTokenAccessor
    {
        private readonly IHttpHeaderAccessor _httpHeaderAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpTokenAccessor"/> class.
        /// </summary>
        /// <param name="securedHeaderAccessor">The header accessor to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="securedHeaderAccessor"/> is null.</exception>
        public HttpTokenAccessor(IHttpHeaderAccessor securedHeaderAccessor)
            => _httpHeaderAccessor = securedHeaderAccessor ?? throw new ArgumentNullException(nameof(securedHeaderAccessor));

        /// <summary>
        /// Returns the current token value from the current HTTP request with the specified key.
        /// If not found, returns an empty value.
        /// </summary>
        /// <param name="key">The token key to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        public string? ReadToken(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            var token = _httpHeaderAccessor.ReadValue(key);
            return token?.StartsWith("Bearer", StringComparison.InvariantCulture) == true
                ? token.Remove(0, "Bearer ".Length - 1).Trim()
                : token;
        }
    }
}