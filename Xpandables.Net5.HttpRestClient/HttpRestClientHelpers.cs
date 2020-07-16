
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// Helpers for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientHelpers
    {
        /// <summary>
        /// Returns the <see cref="HttpRequestMessage"/> from the attribute.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <param name="attribute">The <see cref="HttpRestClientAttribute"/> attribute.</param>
        /// <param name="source">The source of data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        internal static HttpRequestMessage GetHttpRequestMessage<TSource>(this HttpRestClientAttribute attribute, TSource source)
            where TSource : class
        {
            _ = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _ = source ?? throw new ArgumentNullException(nameof(source));

            attribute.Path ??= "/";
            Uri uri;
            if (source is IQueryStringRequest queryStringRequest)
            {
                var queryString = queryStringRequest.GetQueryString();
                var queryStringUri = attribute.Path.AddQueryString(queryString);
                uri = new Uri(queryStringUri, UriKind.Relative);
            }
            else
            {
                uri = new Uri(attribute.Path, UriKind.Relative);
            }

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(attribute.Method), uri);
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(attribute.Accept));
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(System.Threading.Thread.CurrentThread.CurrentCulture.Name));

            if (!attribute.IsNullable)
            {
                switch (attribute.BodyFormat)
                {
                    case BodyFormat.ByteArray:
                        ValidateImplementation<IByteArrayRequest>(source);
                        if ((source as IByteArrayRequest)!.GetByteContent() is { } byteContent)
                            httpRequestMessage.Content = new ByteArrayContent(byteContent);
                        break;
                    case BodyFormat.FormUrlEncoded:
                        ValidateImplementation<IFormUrlEncodedRequest>(source);
                        if ((source as IFormUrlEncodedRequest)!.GetFormContent() is { } formContent)
                            httpRequestMessage.Content = new FormUrlEncodedContent(formContent);
                        break;
                    case BodyFormat.Multipart:
                        ValidateImplementation<IMultipartRequest>(source);
                        var multipartContent = new MultipartFormDataContent();
                        var multipartSource = source as IMultipartRequest;

                        if (multipartSource!.GetByteContent() is { } mbyteContent)
#pragma warning disable CA2000 // Dispose objects before losing scope
                            multipartContent.Add(new ByteArrayContent(mbyteContent));
#pragma warning restore CA2000 // Dispose objects before losing scope

                        if (multipartSource.GetStringContent() is { } mstringContent)
#pragma warning disable CA2000 // Dispose objects before losing scope
                            multipartContent.Add(new StringContent(
                                JsonConvert.SerializeObject(mstringContent), Encoding.UTF8, attribute.ContentType));
#pragma warning restore CA2000 // Dispose objects before losing scope

                        httpRequestMessage.Content = multipartContent;
                        break;
                    case BodyFormat.Stream:
                        ValidateImplementation<IStreamRequest>(source);
                        var memoryStream = new MemoryStream();
                        using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true))
                        using (var jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
                        {
                            JsonSerializer.CreateDefault().Serialize(jsonTextWriter, source);
                            jsonTextWriter.Flush();
                        }
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        httpRequestMessage.Content = new StreamContent(memoryStream);
                        break;
                    case BodyFormat.String:
                        ValidateImplementation<IStringRequest>(source, true);
                        httpRequestMessage.Content = new StringContent(
                            JsonConvert.SerializeObject(source), Encoding.UTF8, attribute.ContentType);
                        break;
                }

                httpRequestMessage.Content!.Headers.ContentType = new MediaTypeHeaderValue(attribute.ContentType);
            }

            if (attribute.IsSecured)
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(attribute.Scheme);

            return httpRequestMessage;
        }

        internal static void ValidateImplementation<TInterface>(object source, bool optional = false)
        {
            if (!typeof(TInterface).IsAssignableFrom(source.GetType()) && !optional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }

        internal static HttpRequestMessage GetHttpRequestMessage<TSource>(TSource source)
            where TSource : class
        {
            var attribute = GetHttpClientDescriptionAttribute(source);
            return attribute.GetHttpRequestMessage(source);
        }

        internal static HttpRestClientAttribute GetHttpClientDescriptionAttribute<TSource>(TSource source)
              where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.GetHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                         ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        internal static NameValueCollection GetHttpResponseHeaders(HttpResponseMessage httpResponse)
            => Enumerable
                .Empty<(string Name, string Value)>()
                .Concat(
                    httpResponse.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            )
                        )
                .Concat(
                    httpResponse.Content?.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            ) ?? Enumerable.Empty<(string, string)>()
                        )
                .Aggregate(
                    seed: new NameValueCollection(),
                    func: (nvc, pair) => { nvc.Add(pair.Name, pair.Value); return nvc; },
                    resultSelector: nvc => nvc
                    );

        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// </summary>
        /// <typeparam name="TSource"> The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="obj">The deserialized object from the JSON string.</param>
        /// <param name="exception">The handled exception.</param>
        /// <param name="options">The JSON converter options.</param>
        public static bool TryDeserializeObject<TSource>(
            this string value,
            [MaybeNullWhen(false)] out TSource? obj,
            [MaybeNullWhen(true)] out Exception? exception,
            JsonSerializerSettings? options = default)
            where TSource : class
        {
            obj = default;
            exception = default;

            try
            {
                obj = JsonConvert.DeserializeObject<TSource>(value, options);
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is ArgumentNullException)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// De-serializes a JSON string from a stream.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A object of type <typeparamref name="T"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="stream"/> does not support reading.</exception>
        internal static T DeserializeJsonFromStream<T>(this Stream stream)
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
        internal static async Task<string> StreamToStringAsync(this Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            return content;
        }

        /// <summary>
        /// Determines whether the <see cref="HttpRestClientException"/> is <see cref="HttpRestClientValidation"/>.
        /// </summary>
        /// <param name="exception">The target exception.</param>
        /// <returns>An <see cref="HttpRestClientValidation"/> if true, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static HttpRestClientValidation? ContentIsHttpRestClientValidation(this HttpRestClientException exception)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));
            if (exception.Message.TryDeserializeObject<HttpRestClientValidation>(out var modelResult, out _))
                return modelResult;

            return default;
        }
    }
}
