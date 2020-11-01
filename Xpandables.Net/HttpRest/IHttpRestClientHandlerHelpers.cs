
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Xpandables.Net.HttpRest
{
    public partial interface IHttpRestClientHandler
    {
        /// <summary>
        /// Returns an <see cref="HttpRequestMessage"/> from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A tack that represents an <see cref="HttpRequestMessage"/> object.</returns>
        internal static async Task<HttpRequestMessage> WriteHttpRequestMessageAsync<TSource>(TSource source, HttpClient httpClient, CancellationToken cancellationToken = default)
            where TSource : class
        {
            var attribute = ReadHttpClientAttribute(source);
            return await ReadHttpRequestMessageAsync(attribute, source, httpClient, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the <see cref="HttpRequestMessage"/> from the attribute.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <param name="attribute">The <see cref="HttpRestClientAttribute"/> attribute.</param>
        /// <param name="source">The source of data.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<Pending>")]
        internal static async Task<HttpRequestMessage> ReadHttpRequestMessageAsync<TSource>(
            HttpRestClientAttribute attribute, TSource source, HttpClient httpClient, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            _ = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            attribute.Path ??= "/";

            if (attribute.In is ParameterLocation.Path or ParameterLocation.Query)
            {
                WriteLocationPath(source, attribute);
                WriteLocationQuery(source, attribute);
            }

            attribute.Uri = new Uri(attribute.Path, UriKind.Relative);

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(attribute.Method), attribute.Uri);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(attribute.Accept));
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(Thread.CurrentThread.CurrentCulture.Name));

            WriteLocationCookie(source, attribute, httpRequestMessage);
            WriteLocationHeader(source, attribute, httpRequestMessage);

            if (!attribute.IsNullable && attribute.In == ParameterLocation.Body)
            {
                httpRequestMessage.Content = attribute.BodyFormat switch
                {
                    BodyFormat.ByteArray => ReadByteArrayContent(source),
                    BodyFormat.FormUrlEncoded => ReadFormUrlEncodedContent(source),
                    BodyFormat.Multipart => ReadMultipartContent(source, attribute),
                    BodyFormat.Stream => await ReadStreamContentAsync(source, cancellationToken).ConfigureAwait(false),
                    _ => ReadStringContent(source, attribute)
                };

                httpRequestMessage.Content!.Headers.ContentType = new MediaTypeHeaderValue(attribute.ContentType);
            }

            if (attribute.IsSecured)
                httpRequestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization is not null
                    ? httpClient.DefaultRequestHeaders.Authorization
                    : new AuthenticationHeaderValue(attribute.Scheme);

            return httpRequestMessage;
        }

        /// <summary>
        /// Appends the given path keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="pathString">A collection of name value path pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pathString"/> is null.</exception>
        internal static string AddPathString(string path, IDictionary<string, string> pathString)
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
        internal static string AddQueryString(string path, IDictionary<string, string?>? queryString)
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
        /// Writes location path using <see cref="AddPathString(string, IDictionary{string, string})"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        internal static void WriteLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Path)
            {
                ValidateInterfaceImplementation<IPathStringLocationRequest>(source, false);
                if (source is IPathStringLocationRequest pathStringRequest)
                {
                    var pathString = pathStringRequest.GetPathStringSource();
                    attribute.Path = AddPathString(attribute.Path!, pathString);
                }
            }
        }

        /// <summary>
        /// Writes location query using <see cref="AddQueryString(string, IDictionary{string, string?}?)"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        internal static void WriteLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Query)
            {
                ValidateInterfaceImplementation<IQueryStringLocationRequest>(source, false);
                if (source is IQueryStringLocationRequest queryStringRequest)
                {
                    var queryString = queryStringRequest.GetQueryStringSource();
                    attribute.Path = AddQueryString(attribute.Path!, queryString);
                }
            }
        }

        /// <summary>
        /// Writes location cookies.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        internal static void WriteLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
              where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Cookie)
            {
                ValidateInterfaceImplementation<ICookieLocationRequest>(source, false);
                if (source is ICookieLocationRequest cookieLocationRequest)
                {
                    var cookieSource = cookieLocationRequest.GetCookieSource();
                    foreach (var parameter in cookieSource)
                    {
                        httpRequestMessage.Options.TryAdd(parameter.Key, parameter.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Writes location headers.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        internal static void WriteLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
                where TSource : notnull
        {
            if (attribute.In == ParameterLocation.Header)
            {
                ValidateInterfaceImplementation<IHeaderLocationRequest>(source, false);
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

        /// <summary>
        /// Returns the source as byte array content.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A byte array content.</returns>
        [return: MaybeNull]
        internal static HttpContent ReadByteArrayContent<TSource>(TSource source)
               where TSource : notnull
        {
            ValidateInterfaceImplementation<IByteArrayRequest>(source, false);
            if (source is IByteArrayRequest byteArrayRequest)
                if (byteArrayRequest.GetByteContent() is { } byteContent)
                    return new ByteArrayContent(byteContent);

            return default;
        }

        /// <summary>
        /// Returns the source as URL encoded content.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>An URL encoded content.</returns>
        [return: MaybeNull]
        internal static HttpContent ReadFormUrlEncodedContent<TSource>(TSource source)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IFormUrlEncodedRequest>(source, false);
            if (source is IFormUrlEncodedRequest formUrlEncodedRequest)
                if (formUrlEncodedRequest.GetFormContent() is { } formContent)
                    return new FormUrlEncodedContent(formContent);

            return default;
        }

        /// <summary>
        /// Returns the source as string content.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A string content.</returns>
        internal static HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
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
        internal static async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
          where TSource : notnull
        {
            ValidateInterfaceImplementation<IStreamRequest>(source, false);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

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

        /// <summary>
        /// Returns the source as multi part content.
        /// The default implementation used <see cref="ReadByteArrayContent{TSource}(TSource)"/> and <see cref="ReadStringContent{TSource}(TSource, HttpRestClientAttribute)"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A multi part content.</returns>
        [return: MaybeNull]
        internal static HttpContent ReadMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IMultipartRequest>(source, false);
            if (source is IMultipartRequest multipartRequest)
            {
                var multipartContent = new MultipartFormDataContent();
                if (ReadByteArrayContent(multipartRequest) is { } byteContent)
                    multipartContent.Add(byteContent);

                multipartContent.Add(ReadStringContent(multipartRequest, attribute));

                return multipartContent;
            }

            return default;
        }

        /// <summary>
        /// Returns the <see cref="HttpRestClientAttribute"/> found in the source.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A <see cref="HttpRestClientAttribute"/> attribute.</returns>
        internal static HttpRestClientAttribute ReadHttpClientAttribute<TSource>(TSource source)
                    where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.ReadHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                         ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        /// <summary>
        /// Returns headers from the HTTP response.
        /// </summary>
        /// <param name="httpResponse">The response to act on.</param>
        /// <returns>A collection of keys/values.</returns>
        internal static NameValueCollection ReadHttpResponseHeaders(HttpResponseMessage httpResponse)
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
        /// Checks whether if the target source implements the specified interface.
        /// Throws an exception if interface not found and it's not optional.
        /// </summary>
        /// <typeparam name="TInterface">The type interface to find.</typeparam>
        /// <param name="source">The source object to act on.</param>
        /// <param name="implementationIsOptional">The value indicating whether or not the interface implementation is mandatory.</param>
        /// <exception cref="ArgumentException">The <paramref name="source"/> must implement <typeparamref name="TInterface"/> interface.</exception>
        internal static void ValidateInterfaceImplementation<TInterface>(object source, bool implementationIsOptional = false)
        {
            if (!typeof(TInterface).IsAssignableFrom(source.GetType()) && !implementationIsOptional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse"/> for success response.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <typeparam name="TElement">The response content type model.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="streamConverter">The stream converter to act with.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        internal static async Task<HttpRestClientResponse<TResponse>> WriteResponseAsync<TResponse, TElement>(HttpResponseMessage httpResponse, Func<Stream, TResponse> streamConverter)
        {
            try
            {
                if (httpResponse.Content is { })
                {
                    var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    if (stream is { })
                    {
                        var results = streamConverter(stream);
                        return HttpRestClientResponse<TResponse>
                            .Success(results, httpResponse.StatusCode)
                            .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                            .AddVersion(httpResponse.Version)
                            .AddReasonPhrase(httpResponse.ReasonPhrase);
                    }
                }

                return HttpRestClientResponse<TResponse>
                    .Success(httpResponse.StatusCode)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResponse>
                    .Failure(exception, HttpStatusCode.BadRequest)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse"/> for a bad request.
        /// </summary>
        /// <param name="responseBuilder">The response content builder.</param>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        internal static async Task<HttpRestClientResponse> WriteBadResponseAsync(Func<Exception, HttpStatusCode, HttpRestClientResponse> responseBuilder, HttpResponseMessage httpResponse)
        {
            var response = httpResponse.Content switch
            {
                { } => await WriteBadResponseContentAsync().ConfigureAwait(false),
                null => responseBuilder(new HttpRestClientException(), httpResponse.StatusCode)
            };

            return response
                .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);

            async Task<HttpRestClientResponse> WriteBadResponseContentAsync()
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return responseBuilder(new HttpRestClientException(content), httpResponse.StatusCode);
            }
        }
    }
}
