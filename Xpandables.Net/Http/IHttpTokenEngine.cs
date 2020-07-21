
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
using System.Security.Claims;

namespace Xpandables.Net.Http
{
    /// <summary>
    ///  Defines a set of methods that can be used to build a token from a collection of claims
    ///  and return back this collection from that token.
    /// </summary>
    public interface IHttpTokenEngine
    {
        /// <summary>
        /// Uses the collection of claims to build a string token.
        /// </summary>
        /// <param name="claims">collection of claims to be used to build token string.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="claims"/> is null.</exception>
        string BuildToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Uses the source object to build a string token.
        /// </summary>
        /// <param name="source">The source to be used.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public virtual string BuildToken(object source) => string.Empty;

        /// <summary>
        /// Returns the collection of claims from the specified token.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>An collection of claims if OK or an empty type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token"/> is null.</exception>
        IEnumerable<Claim> GetClaims(string token);
    }
}