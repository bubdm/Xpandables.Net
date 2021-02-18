
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http
{
    public partial class HttpRestClientHandler
    {
        #region HttpRestClientResponse Builder

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse"/> for success response on async enumerable.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="streamToResponseConverter">The converter to be used from stream to <typeparamref name="TResult"/>.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        protected virtual async Task<HttpRestClientResponse<TResult>> WriteEnumerableResultSuccessResponseAsync<TResult>(HttpResponseMessage httpResponse, Func<Stream, TResult> streamToResponseConverter)
        {
            try
            {
                var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (stream is null)
                {
                    return HttpRestClientResponse<TResult>
                        .Success(httpResponse.StatusCode)
                        .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                        .AddVersion(httpResponse.Version)
                        .AddReasonPhrase(httpResponse.ReasonPhrase);
                }

                var results = streamToResponseConverter(stream);
                return HttpRestClientResponse<TResult>
                    .Success(results, httpResponse.StatusCode)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResult>
                    .Failure(exception)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse"/> for success response on result.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="streamToResponseConverter">The converter to be used from stream to <typeparamref name="TResult"/>.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        protected virtual async Task<HttpRestClientResponse<TResult>> WriteResultSuccessResponseAsync<TResult>(HttpResponseMessage httpResponse, Func<Stream, Task<TResult>> streamToResponseConverter)
        {
            try
            {
                var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (stream is null)
                {
                    return HttpRestClientResponse<TResult>
                        .Success(httpResponse.StatusCode)
                        .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                        .AddVersion(httpResponse.Version)
                        .AddReasonPhrase(httpResponse.ReasonPhrase);
                }

                var results = await streamToResponseConverter(stream).ConfigureAwait(false);
                return HttpRestClientResponse<TResult>
                    .Success(results, httpResponse.StatusCode)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResult>
                    .Failure(exception)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse"/> for a bad request.
        /// </summary>
        /// <param name="badResponseBuilder">The bad response content builder.</param>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        protected virtual async Task<HttpRestClientResponse> WriteBadResponseAsync(Func<Exception, HttpStatusCode, HttpRestClientResponse> badResponseBuilder, HttpResponseMessage httpResponse)
        {
            var response = httpResponse.Content switch
            {
                { } => await WriteBadResponseContentAsync().ConfigureAwait(false),
                null => badResponseBuilder(new HttpRestClientException(), httpResponse.StatusCode)
            };

            return response
                .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);

            async Task<HttpRestClientResponse> WriteBadResponseContentAsync()
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return badResponseBuilder(new HttpRestClientException(content), httpResponse.StatusCode);
            }
        }

        /// <summary>
        /// Returns headers from the HTTP response when return <see cref="HttpRestClientResponse"/>.
        /// </summary>
        /// <param name="httpResponse">The response to act on.</param>
        /// <returns>A collection of keys/values.</returns>
        protected virtual NameValueCollection ReadHttpResponseHeaders(HttpResponseMessage httpResponse)
            => Enumerable
                .Empty<(string Name, string Value)>()
                .Concat(
                    httpResponse.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            )
                        )
                .Concat(
                    httpResponse.Content.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                        )
                        )
                .Aggregate(
                    seed: new NameValueCollection(),
                    func: (nvc, pair) => {
                        var (name, value) = pair;
                        nvc.Add(name, value); return nvc; },
                    resultSelector: nvc => nvc
                    );

        #endregion

        #region HttpRequestMessage Builder

        /// <summary>
        /// Returns an <see cref="HttpRequestMessage"/> from the source .
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an <see cref="HttpRequestMessage"/> object.</returns>
        protected virtual async Task<HttpRequestMessage> WriteHttpRequestMessageFromSourceAsync<TSource>(TSource source, HttpClient httpClient, CancellationToken cancellationToken = default)
            where TSource : class
        {
            var attribute = ReadHttpClientAttributeFromSource(source);
            return await BuildHttpRequestMessageFromAttributeAsync(attribute, source, httpClient, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Builds the <see cref="HttpRequestMessage"/> from the attribute using the source.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <param name="attribute">The <see cref="HttpRestClientAttribute"/> attribute to act on.</param>
        /// <param name="source">The source of data.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <returns>A task that represents an <see cref="HttpRequestMessage"/> object.</returns>
        protected virtual async Task<HttpRequestMessage> BuildHttpRequestMessageFromAttributeAsync<TSource>(
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

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(attribute.Method), attribute.Uri);
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
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
                httpRequestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization ?? new AuthenticationHeaderValue(attribute.Scheme);

            return httpRequestMessage;
        }

        /// <summary>
        /// Returns the <see cref="HttpRestClientAttribute"/> found in the source as decorated attribute or <see cref="IHttpRestClientAttributeProvider"/> implementation.
        /// If not found, throws an exception.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A <see cref="HttpRestClientAttribute"/> attribute.</returns>
        /// <exception cref="ArgumentNullException"><see cref="HttpRestClientAttribute"/> expected or <see cref="IHttpRestClientAttributeProvider"/> implementation.</exception>
        protected virtual HttpRestClientAttribute ReadHttpClientAttributeFromSource<TSource>(TSource source)
                    where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.ReadHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        /// <summary>
        /// Checks whether if the target source implements the specified interface.
        /// Throws an exception if interface not found and it's not optional.
        /// </summary>
        /// <typeparam name="TInterface">The type interface to find.</typeparam>
        /// <param name="source">The source object to act on.</param>
        /// <param name="implementationIsOptional">The value indicating whether or not the interface implementation is mandatory.</param>
        /// <exception cref="ArgumentException">The <paramref name="source"/> must implement <typeparamref name="TInterface"/> interface.</exception>
        protected virtual void ValidateInterfaceImplementation<TInterface>(object source, bool implementationIsOptional = false)
        {
            if (!(source is TInterface) && !implementationIsOptional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }

        /// <summary>
        /// Appends the given path keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="pathString">A collection of name value path pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pathString"/> is null.</exception>
        protected virtual string AddPathString(string path, IDictionary<string, string> pathString)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));
            _ = pathString ?? throw new ArgumentNullException(nameof(pathString));

            if (pathString.Count == 0)
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
        protected virtual string AddQueryString(string path, IDictionary<string, string?>? queryString)
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
        /// Writes location path using <see cref="AddPathString(string, IDictionary{string, string})"/> and <see cref="IPathStringLocationRequest.GetPathStringSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        protected virtual void WriteLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if ((attribute.In & ParameterLocation.Path) == 0) return;

            ValidateInterfaceImplementation<IPathStringLocationRequest>(source);
            if (source is not IPathStringLocationRequest pathStringRequest) return;

            var pathString = pathStringRequest.GetPathStringSource();
            attribute.Path = AddPathString(attribute.Path!, pathString);
        }

        /// <summary>
        /// Writes location query using <see cref="AddQueryString(string, IDictionary{string, string?}?)"/> and <see cref="IQueryStringLocationRequest.GetQueryStringSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        protected virtual void WriteLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            if ((attribute.In & ParameterLocation.Query) == 0) return;
            ValidateInterfaceImplementation<IQueryStringLocationRequest>(source);
            if (source is not IQueryStringLocationRequest queryStringRequest) return;

            var queryString = queryStringRequest.GetQueryStringSource();
            attribute.Path = AddQueryString(attribute.Path!, queryString);
        }

        /// <summary>
        /// Writes location cookies (add elements to <see cref="HttpRequestMessage.Options"/>) using <see cref="ICookieLocationRequest.GetCookieSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        protected virtual void WriteLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
              where TSource : notnull
        {
            if ((attribute.In & ParameterLocation.Cookie) == 0) return;
            ValidateInterfaceImplementation<ICookieLocationRequest>(source);
            if (source is not ICookieLocationRequest cookieLocationRequest) return;

            var cookieSource = cookieLocationRequest.GetCookieSource();
            foreach (var parameter in cookieSource)
                httpRequestMessage.Options.TryAdd(parameter.Key, parameter.Value);
        }

        /// <summary>
        /// Writes location headers using <see cref="IHeaderLocationRequest.GetHeadersSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        protected virtual void WriteLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
                where TSource : notnull
        {
            if ((attribute.In & ParameterLocation.Header) == 0) return;

            ValidateInterfaceImplementation<IHeaderLocationRequest>(source);
            if (source is not IHeaderLocationRequest headerLocationRequest) return;

            var headerSource = headerLocationRequest.GetHeadersSource();
            foreach (var parameter in headerSource)
            {
                httpRequestMessage.Headers.Remove(parameter.Key);
                httpRequestMessage.Headers.Add(parameter.Key, parameter.Value);
            }
        }

        #endregion

        #region HttpRequestMessage Content

        /// <summary>
        /// Returns the source as byte array content using <see cref="IByteArrayRequest.GetByteContent"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A byte array content.</returns>
        [return: MaybeNull]
        protected virtual HttpContent ReadByteArrayContent<TSource>(TSource source)
               where TSource : notnull
        {
            ValidateInterfaceImplementation<IByteArrayRequest>(source);
            if (source is IByteArrayRequest byteArrayRequest && byteArrayRequest.GetByteContent() is { } byteContent)
            {
                return new ByteArrayContent(byteContent);
            }

            return default;
        }

        /// <summary>
        /// Returns the source as URL encoded content using <see cref="IFormUrlEncodedRequest.GetFormContent"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>An URL encoded content.</returns>
        [return: MaybeNull]
        protected virtual HttpContent ReadFormUrlEncodedContent<TSource>(TSource source)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IFormUrlEncodedRequest>(source);
            if (source is IFormUrlEncodedRequest formUrlEncodedRequest && formUrlEncodedRequest.GetFormContent() is { } formContent)
            {
                return new FormUrlEncodedContent(formContent);
            }

            return default;
        }

        /// <summary>
        /// Returns the source as string content using <see cref="IStringRequest.GetStringContent"/> if available, if not use the hole source.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A string content.</returns>
        protected virtual HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonSerializer.Serialize(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, attribute.ContentType);
        }

        /// <summary>
        /// Returns the source as stream content using <see cref="IStreamRequest.GetStreamContent"/> if available, if not use the hole source.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A stream content.</returns>
        protected virtual async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, CancellationToken cancellationToken)
          where TSource : notnull
        {
            ValidateInterfaceImplementation<IStreamRequest>(source);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, streamContent, cancellationToken: cancellationToken).ConfigureAwait(false);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        /// <summary>
        /// Returns the source as multi part content using <see cref="IMultipartRequest"/> interface.
        /// The default implementation used <see cref="ReadByteArrayContent{TSource}(TSource)"/> and <see cref="ReadStringContent{TSource}(TSource, HttpRestClientAttribute)"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A multi part content.</returns>
        [return: MaybeNull]
        protected virtual HttpContent ReadMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : notnull
        {
            ValidateInterfaceImplementation<IMultipartRequest>(source);
            if (source is not IMultipartRequest multipartRequest) return default;

            var multipartContent = new MultipartFormDataContent();
            if (ReadByteArrayContent(multipartRequest) is { } byteContent)
                multipartContent.Add(byteContent);

            multipartContent.Add(ReadStringContent(multipartRequest, attribute));

            return multipartContent;
        }

        #endregion

        #region IAsyncEnumerable Builder

        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>An enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        [SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "<Pending>")]
        protected virtual async IAsyncEnumerable<TResult> AsyncEnumerableBuilderFromStreamAsync<TResult>(
            Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default, JsonSerializerOptions? options = default)
        {
            using var blockingCollection = new BlockingCollection<TResult>();
            await using var iterator = new AsyncEnumerable<TResult>(blockingCollection.GetConsumingEnumerable(cancellationToken)).GetAsyncEnumerator(cancellationToken);

            var enumerateStreamElementToBlockingCollectionThread = new Thread(() => EnumerateStreamElementToBlockingCollection(stream, blockingCollection, cancellationToken, options));
            enumerateStreamElementToBlockingCollectionThread.Start();

            while (await iterator.MoveNextAsync().ConfigureAwait(false))
                yield return iterator.Current;
        }

        /// <summary>
        /// Enumerates the stream content to the blocking collection used to return <see cref="IAsyncEnumerable{T}"/>.
        /// the method makes use of <see cref="Utf8JsonStreamReader"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the collection item.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="resultCollection">The collection result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="options">The JSON serializer options.</param>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        protected void EnumerateStreamElementToBlockingCollection<TResult>(
            Stream stream, BlockingCollection<TResult> resultCollection, CancellationToken cancellationToken, JsonSerializerOptions? options = default)
        {
            using var jsonStreamReader = new Utf8JsonStreamReader(stream, 32 * 1024);

            jsonStreamReader.Read();
            while (jsonStreamReader.Read())
            {
                if (cancellationToken.IsCancellationRequested) break;
                if (jsonStreamReader.TokenType != JsonTokenType.StartObject)
                    continue;
                if (jsonStreamReader.Deserialise<TResult>(options ?? new JsonSerializerOptions { AllowTrailingCommas = false, WriteIndented = false, PropertyNameCaseInsensitive = true }) is { } result)
                    resultCollection.Add(result, cancellationToken);
            }

            resultCollection.CompleteAdding();
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// De-serializes a JSON string from stream using <see cref="System.Text.Json"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Reading stream failed. See inner exception.</exception>
        protected virtual async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream, JsonSerializerOptions? options = default)
        {
            var result = await JsonSerializer.DeserializeAsync<TResult>(
                stream, options ?? new JsonSerializerOptions { AllowTrailingCommas = false, WriteIndented = false, PropertyNameCaseInsensitive = true })
                .ConfigureAwait(false);
#nullable disable
            return result;
#nullable enable
        }

        #endregion
    }
}
