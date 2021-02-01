
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

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client.
    /// The queries or commands should implement one of the following interfaces :
    /// <see cref="IStringRequest"/>, <see cref="IStreamRequest"/>, <see cref="IByteArrayRequest"/>, <see cref="IFormUrlEncodedRequest"/>,
    /// <see cref="IMultipartRequest"/>, <see cref="IQueryStringLocationRequest"/>, <see cref="ICookieLocationRequest"/>, <see cref="IHeaderLocationRequest"/> or <see cref="IPathStringLocationRequest"/>.
    /// <para>When used with dependency injection, you should register the handler using one of the extension methods
    /// <see langword="AddHttpClient{TClient, TImplementation}(Action{IserviceProvider, HttpClient})"/> and you may add
    /// <see langword="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{HttpClient})"/> or other
    /// to customize the client behaviors.</para>
    /// </summary>
    public interface IHttpRestClientHandler : IDisposable
    {
        /// <summary>
        /// Contains the <see cref="System.Net.Http.HttpClient"/> instance for the current handler.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "<Pending>")]
        Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> HandleAsync<TResult>(
            IAsyncQuery<TResult> query, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default);

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        Task<HttpRestClientResponse> HandleAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "<Pending>")]
        Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default);

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "<Pending>")]
        Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default);
    }
}
