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
using System.Linq;

namespace Xpandables.Net5.Http
{
    /// <summary>
    /// Provides with methods to retrieve an HTTP request header value matching a specific key.
    /// </summary>
    public interface IHttpHeaderAccessor
    {
        /// <summary>
        /// Gets the HTTP header value from the current HTTP request matching the specified key.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public sealed string? GetValue(string key) => GetValues(key).FirstOrDefault();

        /// <summary>
        /// Gets all HTTP header values from the current HTTP request matching the specified key.
        /// If not found, returns an empty enumerable.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public sealed IEnumerable<string> GetValues(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            return GetValues().TryGetValue(key, out var values) ? values.AsEnumerable() : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets all HTTP header values from the current HTTP request.
        /// If not found, returns an empty dictionary.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<string>> GetValues();
    }
}