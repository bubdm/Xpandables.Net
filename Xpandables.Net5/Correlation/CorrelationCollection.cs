
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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xpandables.Net5.Correlation
{
    /// <summary>
    /// Default implementation of <see cref="ICorrelationCollection{TKey, TValue}"/> that uses a <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// to store correlated data. You can customize the storage providing your own implementing of <see cref="ICorrelationCollection{TKey, TValue}"/> interface.
    /// </summary>
    public class CorrelationCollection<TKey, TValue> : ICorrelationCollection<TKey, TValue>
        where TKey : notnull
    {
        private ConcurrentDictionary<TKey, TValue> Items { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="CorrelationCollection{TKey, TValue}"/> class that initializes the collection.
        /// </summary>
        public CorrelationCollection() => Items = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// Sets the correlated object by its key. If key already exist, it'll be updated.
        /// Gets the correlated object by its key and returns a value when found, otherwise returns the default type value.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A value from the ambient correlation list of objects.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is null.</exception>
        /// <exception cref="OverflowException">The collection already contains the maximum number</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection
        /// of elements (<see cref="int.MaxValue" />).</exception>
        public virtual TValue this[TKey key]
        {
            get => Items[key];
            set => Items.AddOrUpdate(key, value, (_, __) => value);
        }

        /// <summary>
        /// Returns an enumerator that iterate through the collection.
        /// </summary>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
