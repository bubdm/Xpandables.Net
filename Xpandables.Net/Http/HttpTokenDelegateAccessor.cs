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
    /// The default implementation for <see cref="IHttpTokenDelegateAccessor"/>.
    /// </summary>
    public sealed class HttpTokenDelegateAccessor : IHttpTokenDelegateAccessor
    {
        /// <summary>
        /// Gets or sets the delegate use to retrieve the token value.
        /// </summary>
        public HttpTokenAccessorDelegate HttpTokenAccessorDelegate { get; set; } = _ => default;
        
        string? IHttpTokenAccessor.ReadToken(string key) => HttpTokenAccessorDelegate?.Invoke(key);
    }
}
