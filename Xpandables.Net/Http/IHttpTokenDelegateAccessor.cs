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

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Defines a method delegate used to retrieve the ambient token string from the current HTTP request header.
    /// </summary>
    public interface IHttpTokenDelegateAccessor : IHttpTokenAccessor
    {
        /// <summary>
        /// Gets or sets the delegate use to retrieve the token value.
        /// </summary>
        HttpTokenAccessorDelegate HttpTokenAccessorDelegate { get; set; }

        /// <summary>
        /// Returns the current token value from the current HTTP request with the specified key.
        /// </summary>
        /// <param name="key">The token key to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public new string? ReadToken(string key) => HttpTokenAccessorDelegate?.Invoke(key);
    }
}