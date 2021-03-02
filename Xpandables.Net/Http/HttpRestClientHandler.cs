
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

namespace Xpandables.Net.Http
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IHttpRestClientHandler"/> interface.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public sealed class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        private readonly IHttpRestClientAsyncEnumerableBuilder _httpRestClientAsyncEnumerableBuilder;
        private readonly IHttpRestClientRequestBuilder _httpRestClientRequestBuilder;
        private readonly IHttpRestClientResponseBuilder _httpRestClientResponseBuilder;

        /// <summary>
        /// Gets the target <see cref="System.Net.Http.HttpClient"/> instance.
        /// </summary>
        public HttpClient HttpClient { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpRestClientAsyncEnumerableBuilder">The async-enumerable builder.</param>
        /// <param name="httpRestClientRequestBuilder">The request builder.</param>
        /// <param name="httpRestClientResponseBuilder">The response builder.</param>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientAsyncEnumerableBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientRequestBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientResponseBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientHandler(
            IHttpRestClientAsyncEnumerableBuilder httpRestClientAsyncEnumerableBuilder,
            IHttpRestClientRequestBuilder httpRestClientRequestBuilder,
            IHttpRestClientResponseBuilder httpRestClientResponseBuilder,
            HttpClient httpClient)
        {
            _httpRestClientAsyncEnumerableBuilder = httpRestClientAsyncEnumerableBuilder ?? throw new ArgumentNullException(nameof(httpRestClientAsyncEnumerableBuilder));
            _httpRestClientRequestBuilder = httpRestClientRequestBuilder ?? throw new ArgumentNullException(nameof(httpRestClientRequestBuilder));
            _httpRestClientResponseBuilder = httpRestClientResponseBuilder ?? throw new ArgumentNullException(nameof(httpRestClientResponseBuilder));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Handles the request that returns a collection that can be async-enumerated.
        /// Make use of <see langword="using"/> key work when call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="request">The request to act with. The request must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> HandleAsync<TResult>(IHttpRestClientAsyncRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder.WriteHttpRequestMessageFromSourceAsync(request, HttpClient).ConfigureAwait(false);

                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await _httpRestClientResponseBuilder.WriteSuccessAsyncEnumerableResponseAsync(
                        response,
                        stream => _httpRestClientAsyncEnumerableBuilder.AsyncEnumerableBuilderFromStreamAsync<TResult>(stream, cancellationToken)).ConfigureAwait(false);
                }

                return (HttpRestClientResponse<IAsyncEnumerable<TResult>>)await _httpRestClientResponseBuilder.WriteBadResultResponseAsync(
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
        /// Handles the request that does not return a response.
        /// </summary>
        /// <param name="request">The request to act with. The request must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse> HandleAsync(IHttpRestClientRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder.WriteHttpRequestMessageFromSourceAsync(request, HttpClient).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(_httpRestClientResponseBuilder.ReadHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await _httpRestClientResponseBuilder.WriteBadResultResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
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
        /// Handles the request that returns a response of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="request">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        public async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IHttpRestClientRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder.WriteHttpRequestMessageFromSourceAsync(request, HttpClient).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await _httpRestClientResponseBuilder.WriteSuccessResultResponseAsync(
                        response,
                        stream => _httpRestClientResponseBuilder.DeserializeJsonFromStreamAsync<TResult>(stream)).ConfigureAwait(false);
                }

                return (HttpRestClientResponse<TResult>)await _httpRestClientResponseBuilder.WriteBadResultResponseAsync(
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
                HttpClient.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
