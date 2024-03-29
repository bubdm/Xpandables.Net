﻿
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
    ///  Defines a set of methods that can be used to build a token from a collection of claims
    ///  and return back this collection from that token.
    /// </summary>
    public interface ITokenEngine
    {
        /// <summary>
        /// Uses the collection of claims to build a token.
        /// </summary>
        /// <param name="claims">collection of claims to be used to build token string.</param>
        /// <returns>An instance of <see cref="AccessToken"/> token if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="claims"/> is null.</exception>
        IOperationResult<AccessToken> WriteToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Uses the source object to build a string token. The default behavior throws an <see cref="OperationResultException"/>.
        /// </summary>
        /// <param name="source">The source to be used.</param>
        /// <returns>An instance of string token if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public virtual IOperationResult<AccessToken> WriteToken(object source)
            => throw new OperationResultException(new FailureOperationResult<AccessToken>(new OperationErrorCollection("WriteToken", "Method not implemented")));

        /// <summary>
        /// Returns after validation the collection of claims from the specified token.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>An collection of claims if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token"/> is null.</exception>
        IOperationResult<IEnumerable<Claim>> ReadToken(string token);

        /// <summary>
        /// Returns without validation the collection of claims from the specified token.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>An collection of claims if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token"/> is null.</exception>
        IOperationResult<IEnumerable<Claim>> ReadUnsafeToken(string token);

        /// <summary>
        /// Returns after validation the collection of claims from the specified token.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>An collection of claims if OK.</returns>
        public virtual IOperationResult<IEnumerable<Claim>> ReadToken(AccessToken token)
            => ReadToken(token.Value);
    }
}