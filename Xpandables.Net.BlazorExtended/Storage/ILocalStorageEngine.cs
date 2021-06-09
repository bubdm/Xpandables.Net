
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Storage
{
    /// <summary>
    /// Provides with methods to access a specific storage.
    /// </summary>
    public interface ILocalStorageEngine : ILocalStorageEvent
    {
        /// <summary>
        /// Returns number of elements in the storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an int result.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether of not the <paramref name="key"/> exists in local storage.
        /// </summary>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents a boolean result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the information matching the specified key from local storage.
        /// </summary>
        /// <typeparam name="T">The type of the value to be serialized.</typeparam>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="T"/> type result or null.</returns>
        /// <remarks>The found value will be deserialized to the specified <typeparamref name="T"/> type.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task<T?> ReadAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes the specified value in storage with the key. If key already exist, the value will be updated.
        /// </summary>
        /// <typeparam name="T">The type of the value to be deserialized.</typeparam>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="value">The value to be written</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <remarks>The value will be serialized before writing.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task WriteAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the information matching the specified key from local storage.
        /// </summary>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents a string result or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task<string?> ReadAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes the specified value in storage with the key. If key already exist, the value will be updated.
        /// </summary>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="value">The string value to be written</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task WriteAsync(string key, string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the value matching the specified key.
        /// </summary>
        /// <param name="key">A string value of the name in the storage to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears all information from the storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task ClearAllAsync(CancellationToken cancellationToken = default);
    }
}
