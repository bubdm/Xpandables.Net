
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

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with helper methods for <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Converts the collection to exposes an enumerator that provides asynchronous iteration over values of <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The collection of elements.</param>
        /// <returns>An async-enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return new AsyncEnumerable<T>(source);
        }
    }
}
