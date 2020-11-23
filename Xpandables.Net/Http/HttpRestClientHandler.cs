
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

using Xpandables.Net.CQRS;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IHttpRestClientHandler"/> interface.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public partial class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        /// <summary>
        /// Gets the target <see cref="System.Net.Http.HttpClient"/> instance.
        /// </summary>
        public HttpClient HttpClient { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientHandler(HttpClient httpClient) => HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public virtual async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> HandleAsync<TResult>(
            IAsyncEnumerableQuery<TResult> query, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageFromSourceAsync(query, HttpClient, cancellationToken).ConfigureAwait(false);

                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteEnumerableResultSuccessResponseAsync(response, stream => AsyncEnumerableBuilderFromStreamAsync<TResult>(stream, cancellationToken, options));

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
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public virtual async Task<HttpRestClientResponse> HandleAsync(IAsyncCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageFromSourceAsync(command, HttpClient, cancellationToken).ConfigureAwait(false);
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
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public virtual async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IAsyncCommand<TResult> command, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageFromSourceAsync(command, HttpClient, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResultSuccessResponseAsync(response, stream => DeserializeJsonFromStreamAsync<TResult>(stream, options)).ConfigureAwait(false);

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
        /// <param name="options">The JSON serializer options used to customize deserialization.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public virtual async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default, JsonSerializerOptions? options = default)
        {
            try
            {
                using var request = await WriteHttpRequestMessageFromSourceAsync(query, HttpClient, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResultSuccessResponseAsync(response, stream => DeserializeJsonFromStreamAsync<TResult>(stream, options)).ConfigureAwait(false);

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

        private bool _isDisposed;

        /// <summary>
        /// Disposes the HTTP client instance.
        /// </summary>
        /// <param name="disposing">Determine whether the dispose has already been called.</param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                HttpClient?.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
