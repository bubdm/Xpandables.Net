
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
using System.Net.Http;
using System.Threading.Tasks;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with <see cref="IHttpRestClientHandler"/> request builder.
    /// </summary>
    public interface IHttpRestClientRequestBuilder
    {
        /// <summary>
        /// Returns an <see cref="HttpRequestMessage"/> from the source .
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <returns>A task that represents an <see cref="HttpRequestMessage"/> object.</returns>
        Task<HttpRequestMessage> WriteHttpRequestMessageFromSourceAsync<TSource>(TSource source, HttpClient httpClient)
            where TSource : class;

        /// <summary>
        /// Builds the <see cref="HttpRequestMessage"/> from the attribute using the source.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <param name="attribute">The <see cref="HttpRestClientAttribute"/> attribute to act on.</param>
        /// <param name="source">The source of data.</param>
        /// <param name="httpClient">The target HTTP client.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <returns>A task that represents an <see cref="HttpRequestMessage"/> object.</returns>
        Task<HttpRequestMessage> WriteHttpRequestMessageFromAttributeAsync<TSource>(HttpRestClientAttribute attribute, TSource source, HttpClient httpClient)
            where TSource : class;

        /// <summary>
        /// Returns the <see cref="HttpRestClientAttribute"/> found in the source as decorated attribute or <see cref="IHttpRestClientAttributeProvider"/> implementation.
        /// If not found, throws an exception.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A <see cref="HttpRestClientAttribute"/> attribute.</returns>
        /// <exception cref="ArgumentNullException"><see cref="HttpRestClientAttribute"/> expected or <see cref="IHttpRestClientAttributeProvider"/> implementation.</exception>
        HttpRestClientAttribute ReadHttpClientAttributeFromSource<TSource>(TSource source)
            where TSource : class;

        /// <summary>
        /// Appends the given path keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="pathString">A collection of name value path pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pathString"/> is null.</exception>
        string AddPathString(string path, IDictionary<string, string> pathString);

        /// <summary>
        /// Appends the given query keys and values to the Uri.
        /// </summary>
        /// <param name="path">The base Uri.</param>
        /// <param name="queryString">A collection of name value query pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
        string AddQueryString(string path, IDictionary<string, string?>? queryString);

        /// <summary>
        /// Writes location path using <see cref="AddPathString(string, IDictionary{string, string})"/> and <see cref="IPathStringLocationRequest.GetPathStringSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        void WriteLocationPath<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : class;

        /// <summary>
        /// Writes location query using <see cref="AddQueryString(string, IDictionary{string, string?}?)"/> and <see cref="IQueryStringLocationRequest.GetQueryStringSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        void WriteLocationQuery<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : class;

        /// <summary>
        /// Writes location cookies (add elements to <see cref="HttpRequestMessage.Options"/>) using <see cref="ICookieLocationRequest.GetCookieSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        void WriteLocationCookie<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
              where TSource : class;

        /// <summary>
        /// Writes location headers using <see cref="IHeaderLocationRequest.GetHeadersSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <param name="httpRequestMessage">The target request message.</param>
        void WriteLocationHeader<TSource>(TSource source, HttpRestClientAttribute attribute, HttpRequestMessage httpRequestMessage)
            where TSource : class;

        /// <summary>
        /// Returns the source as byte array content using <see cref="IByteArrayRequest.GetByteContent"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A byte array content.</returns>
        [return: MaybeNull]
        HttpContent ReadByteArrayContent<TSource>(TSource source)
            where TSource : class;

        /// <summary>
        /// Returns the source as URL encoded content using <see cref="IFormUrlEncodedRequest.GetFormContent"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>An URL encoded content.</returns>
        [return: MaybeNull]
        HttpContent ReadFormUrlEncodedContent<TSource>(TSource source)
            where TSource : class;

        /// <summary>
        /// Returns the source as string content using <see cref="IStringRequest.GetStringContent"/> or <see cref="IPatchRequest"/> if available, if not use the hole source.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A string content.</returns>
        HttpContent ReadStringContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : class;

        /// <summary>
        /// Returns the source as stream content using <see cref="IStreamRequest.GetStreamContent"/> if available, if not use the hole source.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <returns>A stream content.</returns>
        Task<HttpContent?> ReadStreamContentAsync<TSource>(TSource source)
          where TSource : class;

        /// <summary>
        /// Returns the source as multi part content using <see cref="IMultipartRequest"/> interface.
        /// The default implementation used <see cref="ReadByteArrayContent{TSource}(TSource)"/> and <see cref="ReadStringContent{TSource}(TSource, HttpRestClientAttribute)"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of source object.</typeparam>
        /// <param name="source">The source object instance.</param>
        /// <param name="attribute">The target attribute.</param>
        /// <returns>A multi part content.</returns>
        [return: MaybeNull]
        HttpContent ReadMultipartContent<TSource>(TSource source, HttpRestClientAttribute attribute)
            where TSource : class;
    }
}
