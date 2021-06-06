
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
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.RequestBuilders
{
    /// <summary>
    /// The <see cref="HttpRequestMessage"/> builder.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public class HttpRestClientRequestBuilder : IHttpRestClientRequestBuilder
    {
        ///<inheritdoc/>
        public virtual string AddPathString(string path, IDictionary<string, string> pathString)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));
            _ = pathString ?? throw new ArgumentNullException(nameof(pathString));

            if (pathString.Count == 0)
                return path;

            foreach (var parameter in pathString)
                path = path.Replace($"{{{parameter.Key}}}", parameter.Value, StringComparison.InvariantCultureIgnoreCase);

            return path;
        }

        ///<inheritdoc/>
        public virtual string AddQueryString(string path, IDictionary<string, string?>? queryString)
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

        ///<inheritdoc/>
        [return: MaybeNull]
        public virtual HttpContent ReadByteArrayContent<TSource>(TSource source) where TSource : class
        {
            ValidateInterfaceImplementation<IByteArrayRequest>(source);
            if (source is IByteArrayRequest byteArrayRequest && byteArrayRequest.GetByteContent() is { } byteContent)
            {
                return new ByteArrayContent(byteContent);
            }

            return default;
        }

        ///<inheritdoc/>
        [return: MaybeNull]
        public virtual HttpContent ReadFormUrlEncodedContent<TSource>(TSource source) where TSource : class
        {
            ValidateInterfaceImplementation<IFormUrlEncodedRequest>(source);
            if (source is IFormUrlEncodedRequest formUrlEncodedRequest && formUrlEncodedRequest.GetFormContent() is { } formContent)
            {
                return new FormUrlEncodedContent(formContent);
            }

            return default;
        }

        ///<inheritdoc/>
        public virtual HttpRestClientAttribute ReadHttpClientAttributeFromSource<TSource>(TSource source) where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.ReadHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        ///<inheritdoc/>
        [return: MaybeNull]
        public virtual HttpContent ReadMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute) where TSource : class
        {
            ValidateInterfaceImplementation<IMultipartRequest>(source);
            if (source is not IMultipartRequest multipartRequest) return default;

            var multipartContent = new MultipartFormDataContent();
            if (ReadByteArrayContent(multipartRequest) is { } byteContent)
                multipartContent.Add(byteContent);

            multipartContent.Add(ReadStringContent(multipartRequest, attribute));

            return multipartContent;
        }

        ///<inheritdoc/>
        public virtual async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source) where TSource : class
        {
            ValidateInterfaceImplementation<IStreamRequest>(source);
            object streamContent = source is IStreamRequest streamRequest ? streamRequest.GetStreamContent() : source;

            var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, streamContent).ConfigureAwait(false);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamContent(memoryStream);
        }

        ///<inheritdoc/>
        public virtual HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute) where TSource : class
        {
            ValidateInterfaceImplementation<IStringRequest>(source, true);
            if (source is IStringRequest stringRequest)
                return new StringContent(JsonSerializer.Serialize(stringRequest.GetStringContent()), Encoding.UTF8, attribute.ContentType);

            ValidateInterfaceImplementation<IPatchRequest>(source, true);
            if (source is IPatchRequest patchRequest)
                return new StringContent(JsonSerializer.Serialize(patchRequest.GetPatchDocument()), Encoding.UTF8, attribute.ContentType);

            return new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, attribute.ContentType);
        }

        ///<inheritdoc/>
        public virtual async Task<HttpRequestMessage> WriteHttpRequestMessageFromAttributeAsync<TSource>(HttpRestClientAttribute attribute, TSource source, HttpClient httpClient) where TSource : class
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

            if (!attribute.IsNullable && (attribute.In & ParameterLocation.Body) == ParameterLocation.Body)
            {
                httpRequestMessage.Content = attribute.BodyFormat switch
                {
                    BodyFormat.ByteArray => ReadByteArrayContent(source),
                    BodyFormat.FormUrlEncoded => ReadFormUrlEncodedContent(source),
                    BodyFormat.Multipart => ReadMultipartContent(source, attribute),
                    BodyFormat.Stream => await ReadStreamContentAsync(source).ConfigureAwait(false),
                    _ => ReadStringContent(source, attribute)
                };

                httpRequestMessage.Content!.Headers.ContentType = new MediaTypeHeaderValue(attribute.ContentType);
            }

            if (attribute.IsSecured)
                httpRequestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization ?? new AuthenticationHeaderValue(attribute.Scheme);

            return httpRequestMessage;
        }

        ///<inheritdoc/>
        public virtual async Task<HttpRequestMessage> WriteHttpRequestMessageFromSourceAsync<TSource>(TSource source, HttpClient httpClient) where TSource : class
        {
            var attribute = ReadHttpClientAttributeFromSource(source);
            return await WriteHttpRequestMessageFromAttributeAsync(attribute, source, httpClient).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual void WriteLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage) where TSource : class
        {
            if ((attribute.In & ParameterLocation.Cookie) != ParameterLocation.Cookie) return;
            ValidateInterfaceImplementation<ICookieLocationRequest>(source);
            if (source is not ICookieLocationRequest cookieLocationRequest) return;

            var cookieSource = cookieLocationRequest.GetCookieSource();
            foreach (var parameter in cookieSource)
                httpRequestMessage.Options.TryAdd(parameter.Key, parameter.Value);
        }

        ///<inheritdoc/>
        public virtual void WriteLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage) where TSource : class
        {
            if ((attribute.In & ParameterLocation.Header) != ParameterLocation.Header) return;

            ValidateInterfaceImplementation<IHeaderLocationRequest>(source);
            if (source is not IHeaderLocationRequest headerLocationRequest) return;

            var headerSource = headerLocationRequest.GetHeadersSource();
            foreach (var parameter in headerSource)
            {
                httpRequestMessage.Headers.Remove(parameter.Key);
                httpRequestMessage.Headers.Add(parameter.Key, parameter.Value);
            }
        }

        ///<inheritdoc/>
        public virtual void WriteLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute) where TSource : class
        {
            if ((attribute.In & ParameterLocation.Path) != ParameterLocation.Path) return;

            ValidateInterfaceImplementation<IPathStringLocationRequest>(source);
            if (source is not IPathStringLocationRequest pathStringRequest) return;

            var pathString = pathStringRequest.GetPathStringSource();
            attribute.Path = AddPathString(attribute.Path!, pathString);
        }

        ///<inheritdoc/>
        public virtual void WriteLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute) where TSource : class
        {
            if ((attribute.In & ParameterLocation.Query) != ParameterLocation.Query) return;
            ValidateInterfaceImplementation<IQueryStringLocationRequest>(source);
            if (source is not IQueryStringLocationRequest queryStringRequest) return;

            var queryString = queryStringRequest.GetQueryStringSource();
            attribute.Path = AddQueryString(attribute.Path!, queryString);
        }

        /// <summary>
        /// Checks whether the target source implements the specified interface.
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
    }
}
