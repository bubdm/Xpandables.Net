
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Helpers for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientExtensions
    {
        /// <summary>
        /// Appends the given path keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="pathString">A collection of name value path pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pathString"/> is null.</exception>
        public static string AddPathString(this string path, IDictionary<string, string> pathString)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));
            _ = pathString ?? throw new ArgumentNullException(nameof(pathString));

            if (pathString.Count <= 0)
                return path;

            foreach (var parameter in pathString)
                path = path.Replace($"{{{parameter.Key}}}", parameter.Value, StringComparison.InvariantCultureIgnoreCase);

            return path;
        }

        /// <summary>
        /// Appends the given query keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="queryString">A collection of name value query pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        public static string AddQueryString(this string path, IDictionary<string, string?>? queryString)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            if (queryString is null)
                return path;

            var anchorIndex = path.IndexOf('#', StringComparison.InvariantCulture);
            var uriToBeAppended = path;
            var anchorText = "";

            // If there is an anchor, then the query string must be inserted before its first occurrence.
            if (anchorIndex != -1)
            {
                anchorText = path[anchorIndex..];
                uriToBeAppended = path.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?', StringComparison.InvariantCulture);
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(parameter.Value is null ? null : UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }

        /// <summary>
        /// Returns the <see cref="HttpRequestMessage"/> from the attribute.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <param name="attribute">The <see cref="HttpRestClientAttribute"/> attribute.</param>
        /// <param name="source">The source of data.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        internal static async Task<HttpRequestMessage> GetHttpRequestMessageAsync<TSource>(
            this HttpRestClientAttribute attribute, TSource source, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            _ = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _ = source ?? throw new ArgumentNullException(nameof(source));

            attribute.Path ??= "/";

            if (attribute.In == ParameterLocation.Path || attribute.In == ParameterLocation.Query)
            {
                ApplyLocationPath(source, attribute);
                ApplyLocationQuery(source, attribute);

                attribute.Uri = new Uri(attribute.Path, UriKind.Relative);
            }
            else
            {
                attribute.Uri = new Uri(attribute.Path, UriKind.Relative);
            }

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(attribute.Method), attribute.Uri);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(attribute.Accept));
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(Thread.CurrentThread.CurrentCulture.Name));

            ApplyLocationCookie(source, attribute, httpRequestMessage);
            ApplyLocationHeader(source, attribute, httpRequestMessage);

            if (!attribute.IsNullable && attribute.In == ParameterLocation.Body)
            {
                httpRequestMessage.Content = attribute.BodyFormat switch
                {
                    BodyFormat.ByteArray => GetByteArrayContent(source),
                    BodyFormat.FormUrlEncoded => GetFormUrlEncodedContent(source),
                    BodyFormat.Multipart => GetMultipartContent(source, attribute),
                    BodyFormat.Stream => await GetStreamContentAsync(source, cancellationToken).ConfigureAwait(false),
                    _ => GetStringContent(source, attribute)
                };

                httpRequestMessage.Content!.Headers.ContentType = new MediaTypeHeaderValue(attribute.ContentType);
            }

            if (attribute.IsSecured)
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(attribute.Scheme);

            return httpRequestMessage;
        }

        private static HttpContent? GetMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateImplementation<IMultipartRequest>(source, false);
            if (source is IMultipartRequest multipartRequest)
            {
                var multipartContent = new MultipartFormDataContent();
                if (GetByteArrayContent(multipartRequest) is { } byteContent)
                    multipartContent.Add(byteContent);

                multipartContent.Add(GetStringContent(multipartRequest, attribute));

                return multipartContent;
            }

            return default;
        }

        private static async Task<HttpContent?> GetStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
            where TSource : notnull
        {
            ValidateImplementation<IStreamRequest>(source, false);
            object streamContent;
            if (source is IStreamRequest streamRequest)
                streamContent = streamRequest.GetStreamContent();
            else
                streamContent = source;

            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 1028, true))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
            {
                JsonSerializer.CreateDefault().Serialize(jsonTextWriter, streamContent);
                await jsonTextWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        private static HttpContent GetStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonConvert.SerializeObject(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonConvert.SerializeObject(source), Encoding.UTF8, attribute.ContentType);
        }

        private static HttpContent? GetFormUrlEncodedContent<TSource>(TSource source)
            where TSource : notnull
        {
            ValidateImplementation<IFormUrlEncodedRequest>(source, false);
            if (source is IFormUrlEncodedRequest formUrlEncodedRequest)
                if (formUrlEncodedRequest.GetFormContent() is { } formContent)
                    return new FormUrlEncodedContent(formContent);

            return default;
        }

        private static HttpContent? GetByteArrayContent<TSource>(TSource source)
            where TSource : notnull
        {
            ValidateImplementation<IByteArrayRequest>(source, false);
            if (source is IByteArrayRequest byteArrayRequest)
                if (byteArrayRequest.GetByteContent() is { } byteContent)
                    return new ByteArrayContent(byteContent);

            return default;
        }

        internal static void ApplyLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Header)
            {
                ValidateImplementation<IHeaderLocationRequest>(source, false);
                if (source is IHeaderLocationRequest headerLocationRequest)
                {
                    var headerSource = headerLocationRequest.GetHeadersSource();
                    foreach (var parameter in headerSource)
                    {
                        httpRequestMessage.Headers.Remove(parameter.Key);
                        httpRequestMessage.Headers.Add(parameter.Key, parameter.Value);
                    }
                }
            }
        }

        internal static void ApplyLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Cookie)
            {
                ValidateImplementation<ICookieLocationRequest>(source, false);
                if (source is ICookieLocationRequest cookieLocationRequest)
                {
                    var cookieSource = cookieLocationRequest.GetCookieSource();
                    foreach (var parameter in cookieSource)
                    {
                        httpRequestMessage.Options.Remove(parameter.Key, out _);
                        httpRequestMessage.Options.TryAdd(parameter.Key, parameter.Value);
                    }
                }
            }
        }

        internal static void ApplyLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Query)
            {
                ValidateImplementation<IQueryStringLocationRequest>(source, false);
                if (source is IQueryStringLocationRequest queryStringRequest)
                {
                    var queryString = queryStringRequest.GetQueryStringSource();
                    attribute.Path = attribute.Path!.AddQueryString(queryString);
                }
            }
        }

        internal static void ApplyLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Path)
            {
                ValidateImplementation<IPathStringLocationRequest>(source, false);
                if (source is IPathStringLocationRequest pathStringRequest)
                {
                    var pathString = pathStringRequest.GetPathStringSource();
                    attribute.Path = attribute.Path!.AddPathString(pathString);
                }
            }
        }

        internal static void ValidateImplementation<TInterface>(object source, bool optional = false)
        {
            if (!typeof(TInterface).IsAssignableFrom(source.GetType()) && !optional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }

        internal static async Task<HttpRequestMessage> GetHttpRequestMessageAsync<TSource>(TSource source, CancellationToken cancellationToken = default)
            where TSource : class
        {
            var attribute = GetHttpClientDescriptionAttribute(source);
            return await attribute.GetHttpRequestMessageAsync(source, cancellationToken).ConfigureAwait(false);
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
