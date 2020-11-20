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

using Xpandables.Net.Http;

namespace Xpandables.Net.Tests
{
    public sealed class HttpRestClientHandlerCustom : HttpRestClientHandler
    {
        public HttpRestClientHandlerCustom(HttpClient httpClient) : base(httpClient) { }

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
            return await Task.FromResult(result);
#nullable enable
        }

        protected override async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
        {
            ValidateInterfaceImplementation<IStreamRequest>(source, false);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true);
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
                if (jsonTextReader.TokenType == JsonToken.StartObject)
                    if (jsonSerializer.Deserialize<TResult>(jsonTextReader) is TResult result)
                        yield return result;
            }
        }
    }
}
