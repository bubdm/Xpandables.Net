
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Xpandables.Net.Asynchronous
{
    /// <summary>
    /// Allows a generic collection to be asynchronously enumerable.
    /// This class implements <see cref="IAsyncEnumerable{T}"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public sealed class AsyncEnumerable<T> : IAsyncEnumerable<T>, IEnumerable<T>
    {
        private readonly IEnumerable<T> _collection;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncEnumerable{T}"/> with the collection to be asynchronously enumerable.
        /// </summary>
        /// <param name="collection">The collection to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public AsyncEnumerable(IEnumerable<T> collection) => _collection = collection ?? throw new ArgumentNullException(nameof(collection));

        /// <summary>
        /// Returns an enumerator that iterates asynchronously through the collection.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator<T>(_collection.GetEnumerator());

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collection).GetEnumerator();
    }
}
