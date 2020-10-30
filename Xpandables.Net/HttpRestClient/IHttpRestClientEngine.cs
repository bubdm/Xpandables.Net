
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Types;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Provides with helper methods for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public interface IHttpRestClientEngine
    {
        /// <summary>
        /// Returns an <see cref="HttpRequestMessage"/> from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A tack that represents an <see cref="HttpRequestMessage"/> object.</returns>
        public async Task<HttpRequestMessage> WriteHttpRequestMessageAsync<TSource>(TSource source, HttpClient httpClient, CancellationToken cancellationToken = default)
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
        public async Task<HttpRequestMessage> ReadHttpRequestMessageAsync<TSource>(
            HttpRestClientAttribute attribute, TSource source, HttpClient httpClient, CancellationToken cancellationToken = default)
            where TSource : notnull
        {
            _ = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            attribute.Path ??= "/";

            if (attribute.In == ParameterLocation.Path || attribute.In == ParameterLocation.Query)
            {
                WriteLocationPath(source, attribute);
                WriteLocationQuery(source, attribute);

                attribute.Uri = new Uri(attribute.Path, UriKind.Relative);
            }
            else
            {
                attribute.Uri = new Uri(attribute.Path, UriKind.Relative);
            }

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
        public string AddPathString(string path, IDictionary<string, string> pathString)
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
        public string AddQueryString(string path, IDictionary<string, string?>? queryString)
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
        public void WriteLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute)
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
        public void WriteLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute)
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
        public void WriteLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
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
        public void WriteLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
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
        public HttpContent ReadByteArrayContent<TSource>(TSource source)
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
        public HttpContent ReadFormUrlEncodedContent<TSource>(TSource source)
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
        public HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonSerializer.Serialize(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, attribute.ContentType);
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
            ValidateInterfaceImplementation<IStreamRequest>(source, false);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, streamContent, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        public HttpContent ReadMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute)
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
        public HttpRestClientAttribute ReadHttpClientAttribute<TSource>(TSource source)
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
        public NameValueCollection ReadHttpResponseHeaders(HttpResponseMessage httpResponse)
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
        public static void ValidateInterfaceImplementation<TInterface>(object source, bool implementationIsOptional = false)
        {
            if (!typeof(TInterface).IsAssignableFrom(source.GetType()) && !implementationIsOptional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
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
        /// <exception cref="InvalidOperationException">Reading stream failed. See inner exception.</exception> 
        public async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream, CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                if (!stream.CanRead) throw new ArgumentException($"{nameof(stream)} does not support reading.");

                var result = await JsonSerializer.DeserializeAsync<TResult>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

                return result!;
            }
            catch (Exception exception) when (exception is JsonException
                                            || exception is NotSupportedException
                                            || exception is ArgumentException)
            {
                throw new InvalidOperationException($"Stream deserialization failed. See inner exception.", exception);
            }
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
                result = JsonSerializer.Deserialize<TResult>(value, options);
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

        /// <summary>
        /// Determines whether the current exception message is <see cref="HttpRestClientValidation"/>.
        /// </summary>
        /// <param name="exception">The target exception.</param>
        /// <param name="validationException">The <see cref="HttpRestClientValidation"/> if OK.</param>
        /// <returns><see langword="true"/> if exception message is <see cref="HttpRestClientValidation"/>, otherwise <see langword="false"/>.</returns>
        public bool IsHttpRestClientValidation(HttpRestClientException exception, [MaybeNullWhen(false)] out HttpRestClientValidation validationException)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));

            if (TryDeserialize(exception.Message, out validationException, out _))
                return true;

            return false;
        }

        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        public IAsyncEnumerable<TResult> ReadAsyncEnumerableFromStreamAsync<TResult>(Stream stream, CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerable<TResult>(DoRead(stream));

            static IEnumerable<TResult> DoRead(Stream stream)
            {
                using var jsonReadStream = new Utf8JsonStreamReader(stream, 32 * 1024);
                while (jsonReadStream.Read())
                    if (jsonReadStream.TokenType == JsonTokenType.StartObject)
                        yield return jsonReadStream.Deserialise<TResult>()!;
            }

        }
    }
}
