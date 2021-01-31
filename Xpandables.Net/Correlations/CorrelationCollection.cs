
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Xpandables.Net.Correlations
{
    /// <summary>
    /// Provides with a collection of objects that need to be shared across asynchronous control flows.
    /// This collection implements <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class CorrelationCollection<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IAsyncEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationCollection{TKey, TValue}"/>
        /// class that is empty, has the default concurrency level, has the default initial
        /// capacity, and uses the default comparer for the key type.
        /// </summary>
        public CorrelationCollection() { }

        /// <summary>
        /// Returns an enumerator that iterates asynchronously through the current collection.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        public IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumerator<KeyValuePair<TKey, TValue>>(GetEnumerator());
    }
}
