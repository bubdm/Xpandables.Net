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

namespace Xpandables.Net.Storage
{
    /// <summary>
    /// Provides with methods to de-serialize to/and from string.
    /// </summary>
    public interface ILocalStorageSerializer
    {
        /// <summary>
        /// Serializes the specified value object to string.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="value">The instance object to serialize.</param>
        /// <returns>A string that is a json representation of the value object or null.</returns>
        string? Serialize<T>(T value);

        /// <summary>
        /// Deserializes the specified json string to the target type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="json">The json string to act on.</param>
        /// <returns>An instance of <typeparamref name="T"/> object or null.</returns>
        T? Deserialize<T>(string json);
    }
}
