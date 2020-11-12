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

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with a method to retrieve the token entity from a string token.
    /// </summary>
    public interface ITokenEntityProvider
    {
        /// <summary>
        /// Returns an instance that contains token entity or throws <see cref="UnauthorizedAccessException"/> exception if not found.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Failed to find claims</exception>
        [return: NotNull]
        object ReadTokenEntity();

        /// <summary>
        /// Returns an instance that contains token of the specified type or throws <see cref="UnauthorizedAccessException"/> exception if not found.
        /// </summary>
        /// <typeparam name="TTokenEntity">The type of the token entity.</typeparam>
        /// <returns>An object of <typeparamref name="TTokenEntity"/> type or exception</returns>
        /// <exception cref="UnauthorizedAccessException">Failed to find claims.</exception>
        [return: NotNull]
        public TTokenEntity ReadTokenType<TTokenEntity>() where TTokenEntity : class => ReadTokenEntity() as TTokenEntity ?? throw new UnauthorizedAccessException();
    }
}
