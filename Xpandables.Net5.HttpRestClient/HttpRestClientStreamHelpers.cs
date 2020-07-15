
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
using Newtonsoft.Json;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// Helpers for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientStreamHelpers
    {
        /// <summary>
        /// De-serializes a json string from a stream.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A object of type <typeparamref name="T"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="stream"/> does not support reading.</exception>
        public static T DeserializeJsonFromStream<T>(this Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var result = JsonSerializer.CreateDefault().Deserialize<T>(jsonTextReader);
            return result!;
        }

        /// <summary>
        /// Reads a stream to string.
        /// </summary>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A string from the stream.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="stream"/> does not support reading.</exception>
        public static async Task<string> StreamToStringAsync(this Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            return content;
        }
    }
}
