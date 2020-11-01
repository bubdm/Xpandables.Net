
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
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Newtonsoft.Json;

namespace Xpandables.Net.HttpRest
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

            if (TryDeserialize(exception.Message, out validationException, out _))
                return true;

            return false;
        }

        /// <summary>
        /// De-serializes a JSON string from stream.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Reading stream failed. See inner exception.</exception> 
        public static TResult DeserializeJsonFromStream<TResult>(this Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var result = JsonSerializer.CreateDefault().Deserialize<TResult>(jsonTextReader);
            return result!;
        }

        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public static async IAsyncEnumerable<TResult> ReadAsyncEnumerableFromStreamAsync<TResult>(this Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var jsonSerializer = JsonSerializer.CreateDefault();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (jsonTextReader.TokenType == JsonToken.StartObject)
                    if (jsonSerializer.Deserialize<TResult>(jsonTextReader) is TResult result)
                        yield return result;
            }
        }

        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to deserialize to.</typeparam>
        /// <param name="jsonString">The JSON to deserialize.</param>
        /// <param name="result">The deserialized object from the JSON string.</param>
        /// <param name="exception">The exception.</param>
        /// <returns><see langword="true"/> if OK, otherwise <see langword="false"/>.</returns>
        public static bool TryDeserialize<TResult>(
            this string jsonString,
            [MaybeNullWhen(false)] out TResult result,
            [MaybeNullWhen(true)] out Exception exception)
            where TResult : class
        {
            result = default;
            exception = default;

            try
            {
                result = JsonConvert.DeserializeObject<TResult>(jsonString);
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
