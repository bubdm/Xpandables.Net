
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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with methods to extend use of <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Serializes the current instance to JSON string using <see cref="Newtonsoft.Json"/>.
        /// </summary>
        /// <param name="source">The object to act on.</param>
        /// <param name="options">The serializer options to be applied.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToNewtonsoftString<T>(this T source, JsonSerializerSettings? options = default) => JsonConvert.SerializeObject(source, options);

        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// The default implementation used the <see cref="Newtonsoft.Json"/> API.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="result">The deserialized object from the JSON string.</param>
        /// <param name="deserializerException">The exception.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns><see langword="true"/> if OK, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static bool TryNewtonsoftDeserialize<TResult>(this
            string value,
            [MaybeNullWhen(false)] out TResult result,
            [MaybeNullWhen(true)] out Exception deserializerException,
            JsonSerializerSettings? options = default)
            where TResult : class
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            result = default;
            deserializerException = default;

            try
            {
                result = JsonConvert.DeserializeObject<TResult>(value, options);
                if (result is null)
                {
                    deserializerException = new ArgumentNullException(nameof(value), "No result from deserialization.");
                    return false;
                }

                return true;
            }
            catch (Exception exception) when (exception is JsonException || exception is NotSupportedException)
            {
                deserializerException = exception;
                return false;
            }
        }
    }
}
