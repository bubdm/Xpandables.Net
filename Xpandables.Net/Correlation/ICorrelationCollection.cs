
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
using System.Diagnostics.CodeAnalysis;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// Provides a collection of objects that need to be shared across asynchronous control flows.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface ICorrelationCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        /// <summary>
        /// Sets the correlated object by its key. If key already exist, it'll be updated.
        /// Gets the correlated object by its key and returns a value when found, otherwise returns the default type value.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A value from the ambient correlation list of objects.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="OverflowException">The collection already contains the maximum number
        /// of elements (<see cref="int.MaxValue"/>).</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        TValue this[TKey key] { set; get; }

        /// <summary>
        /// Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>An value from the ambient correlation list of objects if found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public TValue GetValue(TKey key) => this[key];

        /// <summary>
        ///Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value"> When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns> <see langword="true"/> if the collection contains an element with the specified key;
        /// otherwise, <see langword="false"/>.</returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            try
            {
                value = this[key];
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException
                                            || exception is OverflowException
                                            || exception is KeyNotFoundException)
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Adds the correlated object by its key. If key already exist, it'll be updated.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <param name="value">A object to be stored to the ambient correlation list of objects.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="OverflowException">The collection already contains the maximum number
        /// of elements (<see cref="int.MaxValue"/>).</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public void AddOrUpdateValue(TKey key, TValue value) => this[key] = value;
    }
}
