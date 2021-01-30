
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
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Xpandables.Net.Http
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Provides with a derived class of <see cref="HttpRestClientHandlerUsingNewtonsoft"/> that uses <see cref="Newtonsoft"/>.
    /// </summary>
    public sealed class HttpRestClientHandlerUsingNewtonsoft : HttpRestClientHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientHandlerUsingNewtonsoft(HttpClient httpClient) : base(httpClient) { }

        protected override HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonConvert.SerializeObject(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonConvert.SerializeObject(source), Encoding.UTF8, attribute.ContentType);
        }

        protected override async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream, JsonSerializerOptions? options = null)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} doe snot support reading.");

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var result = Newtonsoft.Json.JsonSerializer.CreateDefault().Deserialize<TResult>(jsonTextReader);
#nullable disable
            return await Task.FromResult(result).ConfigureAwait(false);
#nullable enable
        }

        protected override async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            ValidateInterfaceImplementation<IStreamRequest>(source);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true);
            using var jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None };
            Newtonsoft.Json.JsonSerializer.CreateDefault().Serialize(jsonTextWriter, streamContent);
            await jsonTextWriter.FlushAsync(cancellationToken).ConfigureAwait(false);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        protected override async IAsyncEnumerable<TResult> AsyncEnumerableBuilderFromStreamAsync<TResult>(
            Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default, JsonSerializerOptions? options = null)
        {
            var jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (jsonTextReader.TokenType != JsonToken.StartObject) continue;
                if (jsonSerializer.Deserialize<TResult>(jsonTextReader) is { } result)
                    yield return result;
            }
        }
    }
}
