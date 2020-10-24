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

using System.Diagnostics.CodeAnalysis;
using System;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with a method to retrieve token claims as type.
    /// </summary>
    public interface IHttpTokenClaimProvider
    {
        /// <summary>
        /// Returns an instance that contains token claims or throw <see cref="UnauthorizedAccessException"/> exception if not found.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Failed to find claims</exception>
        [return: NotNull]
        object ReadTokenClaim();

        /// <summary>
        /// Returns an instance that contains token claims of the specified type or null if not found.
        /// </summary>
        /// <typeparam name="TTokenClaim">The type of the token claims.</typeparam>
        /// <returns>An object of <typeparamref name="TTokenClaim"/> type or null.</returns>
        /// <exception cref="UnauthorizedAccessException">Failed to find claims.</exception>
        [return: MaybeNull]
        public TTokenClaim ReadTokenClaim<TTokenClaim>() where TTokenClaim : class => ReadTokenClaim() as TTokenClaim;
    }
}
