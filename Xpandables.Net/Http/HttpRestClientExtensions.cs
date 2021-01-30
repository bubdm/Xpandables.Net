
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
using System.Text.Json;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with extension method for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientExtensions
    {
        /// <summary>
        /// Determines whether the current exception message is <see cref="HttpRestClientValidation"/>.
        /// </summary>
        /// <param name="exception">The target exception.</param>
        /// <param name="validationException">The <see cref="HttpRestClientValidation"/> if OK.</param>
        /// <returns><see langword="true"/> if exception message is <see cref="HttpRestClientValidation"/>, otherwise <see langword="false"/>.</returns>
        public static bool IsHttpRestClientValidation(this HttpRestClientException exception, [MaybeNullWhen(false)] out HttpRestClientValidation validationException)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));
            return exception.Message.TryDeserialize(out validationException, out _);
        }

        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to deserialize to.</typeparam>
        /// <param name="jsonString">The JSON to deserialize.</param>
        /// <param name="result">The deserialized object from the JSON string.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns><see langword="true"/> if OK, otherwise <see langword="false"/>.</returns>
        public static bool TryDeserialize<TResult>(
            this string jsonString,
            [MaybeNullWhen(false)] out TResult result,
            [MaybeNullWhen(true)] out Exception exception,
            JsonSerializerOptions? options = default)
            where TResult : class
        {
            result = default;
            exception = default;

            try
            {
                result = JsonSerializer.Deserialize<TResult>(jsonString, options);
                if (result is null)
                {
                    exception = new ArgumentNullException(nameof(jsonString), "No result from deserialization.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}
