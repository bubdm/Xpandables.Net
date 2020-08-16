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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Extensions
{
    /// <summary>
    /// Default implementation of <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncEnumerable<T> : List<T>, IAsyncEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncEnumerable{T}"/> with the collection.
        /// </summary>
        /// <param name="collection">The collection to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public AsyncEnumerable(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Returns an enumerator that iterates asynchronously through the collection.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator<T>(GetEnumerator());
    }

    /// <summary>
    /// Default implement of <see cref="IAsyncEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncEnumerator{T}"/> with the enumerator.
        /// </summary>
        /// <param name="inner">The enumerator to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="inner"/> is null.</exception>
        public AsyncEnumerator(IEnumerator<T> inner) => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

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
