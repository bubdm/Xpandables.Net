
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

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// Default implementation of <see cref="ICorrelationCollection{TKey, TValue}"/> that uses a <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// to store correlated data. You can customize the storage providing your own implementing of <see cref="ICorrelationCollection{TKey, TValue}"/> interface.
    /// </summary>
    public class CorrelationCollection<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, ICorrelationCollection<TKey, TValue>
        where TKey : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationCollection{TKey, TValue}"/>
        /// class that is empty, has the default concurrency level, has the default initial
        /// capacity, and uses the default comparer for the key type.
        /// </summary>
        public CorrelationCollection() : base() { }
    }
}
