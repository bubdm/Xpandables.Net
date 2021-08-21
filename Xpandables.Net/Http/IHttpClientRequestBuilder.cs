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
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text;
using System.Text.Json;

namespace Xpandables.Net.Http;

/// <summary>
/// Provides with methods to build <see cref="HttpRequestMessage"/> for use with <see cref="IHttpClientDispatcher"/>.
/// </summary>
public interface IHttpClientRequestBuilder
{
    /// <summary>
    /// The main method use to construct an <see cref="HttpRequestMessage"/> from the source.
    /// The <paramref name="source"/> may implement some interfaces such as <see cref="IHttpClientHeaderLocationRequest"/>, <see cref="IHttpClientStringRequest"/> and so on.
    /// </summary>
    /// <typeparam name="TSource">The type of the object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="httpClient">The target HTTP client.</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <returns>A task that represents an <see cref="HttpRequestMessage"/> object.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="source"/> must be decorated with
    /// <see cref="HttpClientAttribute"/> or implement <see cref="IHttpClientAttributeProvider"/>.</exception>
    public virtual async Task<HttpRequestMessage> WriteHttpRequestMessageAsync<TSource>(TSource source, HttpClient httpClient, JsonSerializerOptions? serializerOptions = default)
           where TSource : class
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));
        _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        var attribute = ReadHttpRestClientAttribute(source);

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
                BodyFormat.Stream => await ReadStreamContentAsync(source, serializerOptions).ConfigureAwait(false),
                _ => ReadStringContent(source, attribute, serializerOptions)
            };

            if (httpRequestMessage.Content is not null)
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(attribute.ContentType);
        }

        if (attribute.IsSecured)
        {
            httpRequestMessage.Headers.Authorization =
                httpClient.DefaultRequestHeaders.Authorization ?? new AuthenticationHeaderValue(attribute.Scheme);
        }

        return httpRequestMessage;

        static HttpClientAttribute ReadHttpRestClientAttribute(TSource source)
        {
            if (source is IHttpClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.ReadHttpClientAttribute();

            return source.GetType().GetCustomAttribute<HttpClientAttribute>()
                ?? throw new ArgumentNullException(
                    $"{nameof(HttpClientAttribute)} excepted from : {source.GetType().Name}");
        }
    }

    /// <summary>
    /// Appends the given path keys and values to the Uri.
    /// </summary>
    /// <param name="path">The base Uri.</param>
    /// <param name="pathString">A collection of name value path pairs to append.</param>
    /// <returns>The combined result.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="pathString"/> is null.</exception>
    public virtual string AddPathString(string path, IDictionary<string, string> pathString)
    {
        _ = path ?? throw new ArgumentNullException(nameof(path));
        _ = pathString ?? throw new ArgumentNullException(nameof(pathString));

        if (pathString.Count == 0)
            return path;

        foreach (var parameter in pathString)
            path = path.Replace(
                $"{{{parameter.Key}}}",
                parameter.Value,
                StringComparison.InvariantCultureIgnoreCase);

        return path;
    }

    /// <summary>
    /// Appends the given query keys and values to the Uri.
    /// </summary>
    /// <param name="path">The base Uri.</param>
    /// <param name="queryString">A collection of name value query pairs to append.</param>
    /// <returns>The combined result.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
    public virtual string AddQueryString(string path, IDictionary<string, string?>? queryString)
    {
        // From MS internal code
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
    /// Writes location path using <see cref="AddPathString(string, IDictionary{string, string})"/> 
    /// and <see cref="IHttpClientPathStringLocationRequest.GetPathStringSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual void WriteLocationPath<TSource>(TSource source, HttpClientAttribute attribute)
        where TSource : class
    {
        if ((attribute.In & ParameterLocation.Path) != ParameterLocation.Path)
            return;

        ValidateInterfaceImplementation<IHttpClientPathStringLocationRequest>(source);
        if (source is not IHttpClientPathStringLocationRequest pathStringRequest)
            return;

        var pathString = pathStringRequest.GetPathStringSource();
        attribute.Path = AddPathString(attribute.Path!, pathString);
    }

    /// <summary>
    /// Writes location query using <see cref="AddQueryString(string, IDictionary{string, string?}?)"/> 
    /// and <see cref="IHttpClientQueryStringLocationRequest.GetQueryStringSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual void WriteLocationQuery<TSource>(TSource source, HttpClientAttribute attribute)
        where TSource : class
    {
        if ((attribute.In & ParameterLocation.Query) != ParameterLocation.Query)
            return;

        ValidateInterfaceImplementation<IHttpClientQueryStringLocationRequest>(source);
        if (source is not IHttpClientQueryStringLocationRequest queryStringRequest)
            return;

        var queryString = queryStringRequest.GetQueryStringSource();
        attribute.Path = AddQueryString(attribute.Path!, queryString);
    }

    /// <summary>
    /// Writes location cookies (add elements to <see cref="HttpRequestMessage.Options"/>) 
    /// using <see cref="IHttpClientCookieLocationRequest.GetCookieSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <param name="httpRequestMessage">The target request message.</param>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual void WriteLocationCookie<TSource>(TSource source, HttpClientAttribute attribute, HttpRequestMessage httpRequestMessage)
        where TSource : class
    {
        if ((attribute.In & ParameterLocation.Cookie) != ParameterLocation.Cookie)
            return;

        ValidateInterfaceImplementation<IHttpClientCookieLocationRequest>(source);
        if (source is not IHttpClientCookieLocationRequest cookieLocationRequest)
            return;

        var cookieSource = cookieLocationRequest.GetCookieSource();
        foreach (var parameter in cookieSource)
            httpRequestMessage.Options.TryAdd(parameter.Key, parameter.Value);
    }

    /// <summary>
    /// Writes location headers using <see cref="IHttpClientHeaderLocationRequest.GetHeadersSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <param name="httpRequestMessage">The target request message.</param>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual void WriteLocationHeader<TSource>(TSource source, HttpClientAttribute attribute, HttpRequestMessage httpRequestMessage)
        where TSource : class
    {
        if ((attribute.In & ParameterLocation.Header) != ParameterLocation.Header) return;

        ValidateInterfaceImplementation<IHttpClientHeaderLocationRequest>(source);
        if (source is not IHttpClientHeaderLocationRequest headerLocationRequest) return;

        var headerSource = headerLocationRequest.GetHeadersSource();
        foreach (var parameter in headerSource)
        {
            httpRequestMessage.Headers.Remove(parameter.Key);
            httpRequestMessage.Headers.Add(parameter.Key, parameter.Value);
        }
    }

    /// <summary>
    /// Returns the source as byte array content using <see cref="IHttpClientByteArrayRequest.GetByteContent"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <returns>A byte array content.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual HttpContent? ReadByteArrayContent<TSource>(TSource source) where TSource : class
    {
        ValidateInterfaceImplementation<IHttpClientByteArrayRequest>(source);
        if (source is IHttpClientByteArrayRequest byteArrayRequest
            && byteArrayRequest.GetByteContent() is { } byteContent)
        {
            return new ByteArrayContent(byteContent);
        }

        return default;
    }

    /// <summary>
    /// Returns the source as URL encoded content using <see cref="IHttpClientFormUrlEncodedRequest.GetFormSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <returns>An URL encoded content.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual HttpContent? ReadFormUrlEncodedContent<TSource>(TSource source) where TSource : class
    {
        ValidateInterfaceImplementation<IHttpClientFormUrlEncodedRequest>(source);
        if (source is IHttpClientFormUrlEncodedRequest formUrlEncodedRequest
            && formUrlEncodedRequest.GetFormSource() is { } formContent)
        {
            return new FormUrlEncodedContent(formContent);
        }

        return default;
    }

    /// <summary>
    /// Returns the source as string content using <see cref="IHttpClientStringRequest.GetStringContent"/> 
    /// or <see cref="IHttpClientPatchRequest"/> if available, if not use the hole source.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <returns>A string content.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual HttpContent ReadStringContent<TSource>(TSource source, HttpClientAttribute attribute, JsonSerializerOptions? serializerOptions = default)
        where TSource : class
    {
        ValidateInterfaceImplementation<IHttpClientStringRequest>(source, true);
        if (source is IHttpClientStringRequest stringRequest)
        {
            return new StringContent(
                JsonSerializer.Serialize(stringRequest.GetStringContent(), serializerOptions),
                Encoding.UTF8,
                attribute.ContentType);
        }

        ValidateInterfaceImplementation<IHttpClientPatchRequest>(source, true);
        if (source is IHttpClientPatchRequest patchRequest)
        {
            return new StringContent(
                JsonSerializer.Serialize(patchRequest.GetPatchDocument(), serializerOptions),
                Encoding.UTF8,
                attribute.ContentType);
        }

        return new StringContent(
            JsonSerializer.Serialize(source, source.GetType(), serializerOptions),
            Encoding.UTF8,
            attribute.ContentType);
    }

    /// <summary>
    /// Returns the source as stream content using <see cref="IHttpClientStreamRequest.GetStreamContent"/> 
    /// if available, if not use the hole source.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <returns>A stream content.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual async Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source, JsonSerializerOptions? serializerOptions = default)
        where TSource : class
    {
        ValidateInterfaceImplementation<IHttpClientStreamRequest>(source);
        object streamContent = source is IHttpClientStreamRequest streamRequest
            ? streamRequest.GetStreamContent()
            : source;

        var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, streamContent, serializerOptions).ConfigureAwait(false);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new StreamContent(memoryStream);
    }

    /// <summary>
    /// Returns the source as multi part content using <see cref="IHttpClientMultipartRequest"/> interface,
    /// using <see cref="ReadByteArrayContent{TSource}(TSource)"/> 
    /// and <see cref="ReadStringContent{TSource}(TSource, HttpClientAttribute, JsonSerializerOptions)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of source object.</typeparam>
    /// <param name="source">The source object instance.</param>
    /// <param name="attribute">The target attribute.</param>
    /// <returns>A multi part content.</returns>
    /// <remarks>Used by <see cref="WriteHttpRequestMessageAsync{TSource}(TSource, HttpClient, JsonSerializerOptions?)"/></remarks>
    public virtual HttpContent? ReadMultipartContent<TSource>(TSource source, HttpClientAttribute attribute)
        where TSource : class
    {
        ValidateInterfaceImplementation<IHttpClientMultipartRequest>(source);
        if (source is not IHttpClientMultipartRequest multipartRequest) return default;

        var multipartContent = new MultipartFormDataContent();
        if (ReadByteArrayContent(multipartRequest) is { } byteContent)
            multipartContent.Add(byteContent);

        multipartContent.Add(ReadStringContent(multipartRequest, attribute));

        return multipartContent;
    }

    /// <summary>
    /// Checks whether the target source implements the specified interface.
    /// Throws an exception if interface not found and it's not optional.
    /// </summary>
    /// <typeparam name="TInterface">The type interface to find.</typeparam>
    /// <param name="source">The source object to act on.</param>
    /// <param name="implementationIsOptional">The value indicating whether 
    /// or not the interface implementation is mandatory.</param>
    /// <exception cref="ArgumentException">The <paramref name="source"/> 
    /// must implement <typeparamref name="TInterface"/> interface.</exception>
    protected virtual void ValidateInterfaceImplementation<TInterface>(object source, bool implementationIsOptional = false)
    {
        if (!(source is TInterface) && !implementationIsOptional)
        {
            throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }
    }
}

/// <summary>
/// The default <see cref="HttpRequestMessage"/> builder.
/// You must derive from this class in order to customize its behaviors.
/// </summary>
public class HttpClientRequestBuilder : IHttpClientRequestBuilder { }