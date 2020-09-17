
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
using System.Diagnostics.CodeAnalysis;

using Xpandables.Net.Http;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// Provides with a default implementation of <see cref="IIdentityDataProvider"/> that uses <see cref="IHttpTokenAccessor"/> and <see cref="IHttpTokenEngine"/> to retrieve the entity object.
    /// </summary>
    public sealed class IdentityDataProvider : IIdentityDataProvider
    {
        private readonly IHttpTokenAccessor _httpTokenAccessor;
        private readonly IHttpTokenEngine _httpTokenEngine;

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityDataProvider"/> with the token accessor and token engine.
        /// </summary>
        /// <param name="httpTokenAccessor">The accessor to get the token value.</param>
        /// <param name="httpTokenEngine">The engine to read and write token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpTokenAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpTokenEngine"/> is null.</exception>
        public IdentityDataProvider(IHttpTokenAccessor httpTokenAccessor, IHttpTokenEngine httpTokenEngine)
        {
            _httpTokenAccessor = httpTokenAccessor ?? throw new ArgumentNullException(nameof(httpTokenAccessor));
            _httpTokenEngine = httpTokenEngine ?? throw new ArgumentNullException(nameof(httpTokenEngine));
        }

        /// <summary>
        /// Returns an instance that contains identity or throw an exception if not found.
        /// </summary>
        /// <exception cref="InvalidOperationException">Failed to find secured data.</exception>
        [return: NotNull]
        public object GetIdentity()
        {
            var token = _httpTokenAccessor.ReadToken() ?? throw new InvalidOperationException("Expected token not found");
            var claims = _httpTokenEngine.ReadToken(token);
            return _httpTokenEngine.ReadIdentity(claims);
        }
    }
}
