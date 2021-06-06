
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
using System.Linq;
using System.Security.Claims;

namespace Xpandables.Net.Correlations
{
    /// <summary>
    /// Default implementation of <see cref="ICorrelationContext"/>.
    /// </summary>
    public sealed class CorrelationContext : ICorrelationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="CorrelationContext"/>.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="objects">The correlation connection.</param>
        public CorrelationContext(IHttpContextAccessor httpContextAccessor, CorrelationCollection<string, object> objects)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            Objects = objects;
        }

        /// <summary>
        /// Gets the collection of objects for the current context.
        /// </summary>
        public CorrelationCollection<string, object> Objects { get; }

        ///<inheritdoc/>
        public string UserId => _httpContextAccessor
            .HttpContext!
            .User
            .Claims
            .Single(u => u.Type == ClaimTypes.Sid)
            .Value;

        ///<inheritdoc/>
        public IEnumerable<Claim> Claims => _httpContextAccessor
            .HttpContext?
            .User?
            .Claims ?? Enumerable.Empty<Claim>();

        ///<inheritdoc/>
        public string CorrelationId =>
            _httpContextAccessor
            .HttpContext!
            .Request
            .Headers[ICorrelationContext.DefaultHeader];

        ///<inheritdoc/>
        public bool IsAvailable => Claims.Any();
    }
}
