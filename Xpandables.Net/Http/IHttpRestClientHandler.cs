
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
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with methods to handle <see cref="IHttpRestClientRequest"/> 
    /// or <see cref="IHttpRestClientRequest{TResponse}"/> requests using a typed client HTTP Client.
    /// The request should implement one of the following interfaces :
    /// <see cref="IStringRequest"/>, <see cref="IStreamRequest"/>, <see cref="IByteArrayRequest"/>, 
    /// <see cref="IFormUrlEncodedRequest"/>,
    /// <see cref="IMultipartRequest"/>, <see cref="IQueryStringLocationRequest"/>, <see cref="ICookieLocationRequest"/>,
    /// <see cref="IHeaderLocationRequest"/>
    /// or <see cref="IPathStringLocationRequest"/>, and must be decorated with <see cref="HttpRestClientAttribute"/> 
    /// or implement <see cref="IHttpRestClientAttributeProvider"/>.
    /// </summary>
    public interface IHttpRestClientHandler : IDisposable
    {
        /// <summary>
        /// Contains the <see cref="System.Net.Http.HttpClient"/> instance for the current handler.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Sends the request that returns a collection that can be async-enumerated.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="request">The request to act with. The request must be decorated with 
        /// the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> SendAsync<TResult>(
            IHttpRestClientAsyncRequest<TResult> request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the request that does not return a response.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <param name="request">The request to act with. The request must be decorated with 
        /// the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        Task<HttpRestClientResponse> SendAsync(
            IHttpRestClientRequest request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the request that returns a response of <typeparamref name="TResult"/> type.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="request">The query to act with. The query must be decorated with
        /// the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        Task<HttpRestClientResponse<TResult>> SendAsync<TResult>(
            IHttpRestClientRequest<TResult> request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default);
    }
}
