
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

namespace Xpandables.Net.Security
{
    /// <summary>
    ///  Defines a set of methods that can be used to build a refresh token.
    /// </summary>
    public interface IRefreshTokenEngine
    {
        /// <summary>
        /// Writes a string token to be used as a refresh token.
        /// </summary>
        /// <returns>An instance of refresh token if OK.</returns>
        IOperationResult<RefreshToken> WriteToken();

        /// <summary>
        /// Uses the source object to build a string refresh token. The default behavior returns a refresh token with non-usable values.
        /// </summary>
        /// <param name="source">The source to be used.</param>
        /// <returns>An instance of refresh token if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public virtual IOperationResult<RefreshToken> WriteToken(object source)
            => new FailureOperationResult<RefreshToken>(new RefreshToken("REFRESH TOKEN VALUE", DateTime.UtcNow));

        /// <summary>
        /// Returns the collection of claims from the expired token.
        /// </summary>
        /// <param name="expiredToken">The expired token string.</param>
        /// <returns>An collection of claims if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expiredToken"/> is null.</exception>
        IOperationResult<IEnumerable<Claim>> ReadExpiredToken(string expiredToken);
    }
}
