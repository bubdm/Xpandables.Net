
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.HttpRest
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client.
    /// The queries and commands should implement one of the following interfaces :
    /// <see cref="IStringRequest"/>, <see cref="IStreamRequest"/>, <see cref="IByteArrayRequest"/>, <see cref="IFormUrlEncodedRequest"/>,
    /// <see cref="IMultipartRequest"/> and <see cref="IQueryStringLocationRequest"/>.
    /// <para>You should register the handler using one of the extension methods 
    /// <see langword="AddHttpClient{TClient, TImplementation}(IServiceCollection)"/> and you may add 
    /// <see langword="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{HttpClient})"/> or other 
    /// to customize the client behaviors.</para>
    /// </summary>
    public partial interface IHttpRestClientHandler : IDisposable
    {
        /// <summary>
        /// Contains the <see cref="HttpContent"/> instance for the current handler.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> HandleAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageAsync(query, HttpClient, cancellationToken).ConfigureAwait(false);

                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResponseAsync<IAsyncEnumerable<TResult>, TResult>(response, stream =>
                        stream.ReadAsyncEnumerableFromStreamAsync<TResult>(cancellationToken));

                return (HttpRestClientResponse<IAsyncEnumerable<TResult>>)await WriteBadResponseAsync(
                    HttpRestClientResponse<IAsyncEnumerable<TResult>>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is ArgumentException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse<IAsyncEnumerable<TResult>>.Failure(exception);
            }
        }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> HandleAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : class, IAsyncQuery<TResult>
            => await HandleAsync(query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public async Task<HttpRestClientResponse> HandleAsync(IAsyncCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageAsync(command, HttpClient, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(ReadHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await WriteBadResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is ArgumentException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse.Failure(exception);
            }
        }

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IAsyncCommand<TResult> command, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageAsync(command, HttpClient, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResponseAsync<TResult, TResult>(response, stream => stream.DeserializeJsonFromStream<TResult>()).ConfigureAwait(false);

                return (HttpRestClientResponse<TResult>)await WriteBadResponseAsync(
                    HttpRestClientResponse<TResult>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse<TResult>.Failure(exception);
            }
        }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageAsync(query, HttpClient, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResponseAsync<TResult, TResult>(response, stream => stream.DeserializeJsonFromStream<TResult>()).ConfigureAwait(false);

                return (HttpRestClientResponse<TResult>)await WriteBadResponseAsync(
                    HttpRestClientResponse<TResult>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse<TResult>.Failure(exception);
            }
        }
    }
}
