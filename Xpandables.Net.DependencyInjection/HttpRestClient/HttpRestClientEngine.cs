
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
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// The default implementation of <see cref="IHttpRestClientEngine"/> that uses <see cref="Newtonsoft.Json"/> for async-enumerable from stream.
    /// </summary>
    public class HttpRestClientEngine : IHttpRestClientEngine
    {
        /// <summary>
        /// Returns the source as string content.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A string content.</returns>
        public HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            IHttpRestClientEngine.ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonConvert.SerializeObject(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonConvert.SerializeObject(source), Encoding.UTF8, attribute.ContentType);
        }

        /// <summary>
        /// Returns the source as stream content.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A stream content.</returns>
        public async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
          where TSource : notnull
        {
            IHttpRestClientEngine.ValidateInterfaceImplementation<IStreamRequest>(source, false);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
            {
                Newtonsoft.Json.JsonSerializer.CreateDefault().Serialize(jsonTextWriter, streamContent);
                await jsonTextWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public async IAsyncEnumerable<TResult> ReadAsyncEnumerableFromStreamAsync<TResult>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (jsonTextReader.TokenType == JsonToken.StartObject)
                    yield return jsonSerializer.Deserialize<TResult>(jsonTextReader)!;
            }
        }

        /// <summary>
        /// De-serializes a JSON string from stream.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="stream"/> does not support reading.</exception> 
        public async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream, CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var result = Newtonsoft.Json.JsonSerializer.CreateDefault().Deserialize<TResult>(jsonTextReader);
            return await Task.FromResult(result!).ConfigureAwait(false);
        }


        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="result">The deserialized object from the JSON string.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns><see langword="true"/> if OK, otherwise <see langword="false"/>.</returns>
        public bool TryDeserialize<TResult>(
            string value,
            [MaybeNullWhen(false)] out TResult result,
            [MaybeNullWhen(true)] out Exception exception,
            JsonSerializerOptions? options = default)
            where TResult : class
        {
            result = default;
            exception = default;

            try
            {
                result = JsonConvert.DeserializeObject<TResult>(value);
                if (result is null)
                {
                    exception = new ArgumentNullException(nameof(value), "No result from deserialization.");
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
