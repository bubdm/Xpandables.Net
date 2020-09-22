
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
    /// The delegate that is used to return the token from the specified header key.
    /// </summary>
    /// <param name="key">The key to read the token from.</param>
    /// <returns>A string token value if found or null.</returns>
    public delegate string HttpTokenAccessorDelegate(string key);

    /// <summary>
    /// A helper class used to implement the <see cref="IHttpTokenAccessor"/> interface.
    /// </summary>
    public sealed class HttpTokenAccessorBuilder : IHttpTokenAccessor
    {
        private readonly HttpTokenAccessorDelegate _tokenAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpTokenAccessorBuilder"/> class with the delegate to be used
        /// as <see cref="IHttpTokenAccessor"/> implementation.
        /// </summary>
        /// <param name="tokenAccessor">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IHttpTokenAccessor"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenAccessor"/> is null.</exception>
        public HttpTokenAccessorBuilder(HttpTokenAccessorDelegate tokenAccessor)
            => _tokenAccessor = tokenAccessor ?? throw new ArgumentNullException(nameof(tokenAccessor));

        /// <summary>
        /// Returns the current token value from the current HTTP request with the specified key.
        /// If not found, returns an empty value.
        /// </summary>
        /// <param name="key">The token key to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        public string? ReadToken(string key) => _tokenAccessor(key);
    }
}