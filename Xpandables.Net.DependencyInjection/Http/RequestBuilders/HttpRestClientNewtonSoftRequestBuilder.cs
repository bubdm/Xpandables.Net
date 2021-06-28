
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

using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.RequestBuilders
{
    /// <summary>
    /// The <see cref="HttpRequestMessage"/> builder using NewtonSoft.
    /// </summary>
    public class HttpRestClientNewtonsoftRequestBuilder : HttpRestClientRequestBuilder
    {
        ///<inheritdoc/>
        public override async Task<HttpContent?> ReadStreamContentAsync<TSource>(
            TSource source, JsonSerializerOptions? serializerOptions = default)
            where TSource : class
        {
            ValidateInterfaceImplementation<IStreamRequest>(source);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true);
            using var jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None };
            Newtonsoft.Json.JsonSerializer.CreateDefault().Serialize(jsonTextWriter, streamContent);
            await jsonTextWriter.FlushAsync().ConfigureAwait(false);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        ///<inheritdoc/>
        public override HttpContent ReadStringContent<TSource>(
            TSource source,
            HttpRestClientAttribute attribute,
            JsonSerializerOptions? serializerOptions = default)
            where TSource : class
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonConvert.SerializeObject(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            ValidateInterfaceImplementation<IPatchRequest>(source, true);
            if (source is IPatchRequest patchRequest)
                return new StringContent(JsonConvert.SerializeObject(patchRequest.GetPatchDocument()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonConvert.SerializeObject(source), Encoding.UTF8, attribute.ContentType);
        }
    }
}
