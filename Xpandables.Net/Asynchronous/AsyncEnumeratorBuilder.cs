
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
using System.Threading.Tasks;

namespace Xpandables.Net.Asynchronous
{
    /// <summary>
    /// Add asynchronous iteration support to a generic collection.
    /// This class implements <see cref="IAsyncEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncEnumeratorBuilder<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncEnumeratorBuilder{T}"/> with the enumerator.
        /// </summary>
        /// <param name="inner">The enumerator to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="inner"/> is null.</exception>
        public AsyncEnumeratorBuilder(IEnumerator<T> inner) => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current => _inner.Current;

        /// <summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        ///  Advances the enumerator asynchronously to the next element of the collection.
        /// </summary>
        /// <returns> A System.Threading.Tasks.ValueTask`1 that will complete with a result of true if the enumerator was successfully 
        /// advanced to the next element, or false if the enumerator has passed the end of the collection.</returns>
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    }
}
