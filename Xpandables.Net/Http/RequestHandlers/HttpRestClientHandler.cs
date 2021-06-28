
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

namespace Xpandables.Net.Http.RequestHandlers
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IHttpRestClientHandler"/> interface.
    /// </summary>
    public sealed class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        private readonly IHttpRestClientRequestBuilder _httpRestClientRequestBuilder;
        private readonly IHttpRestClientResponseBuilder _httpRestClientResponseBuilder;

        /// <summary>
        /// Gets the target <see cref="System.Net.Http.HttpClient"/> instance.
        /// </summary>
        public HttpClient HttpClient { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpRestClientRequestBuilder">The request builder.</param>
        /// <param name="httpRestClientResponseBuilder">The response builder.</param>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientRequestBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientResponseBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        public HttpRestClientHandler(
            IHttpRestClientRequestBuilder httpRestClientRequestBuilder,
            IHttpRestClientResponseBuilder httpRestClientResponseBuilder,
            HttpClient httpClient)
        {
            _httpRestClientRequestBuilder = httpRestClientRequestBuilder ?? throw new ArgumentNullException(nameof(httpRestClientRequestBuilder));
            _httpRestClientResponseBuilder = httpRestClientResponseBuilder ?? throw new ArgumentNullException(nameof(httpRestClientResponseBuilder));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        ///<inheritdoc/>
        public async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> SendAsync<TResult>(
            IHttpRestClientAsyncRequest<TResult> request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder
                    .WriteHttpRequestMessageAsync(request, HttpClient)
                    .ConfigureAwait(false);

                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await _httpRestClientResponseBuilder.WriteAsyncEnumerableResponseAsync<TResult>(
                        response,
                        serializerOptions,
                        cancellationToken)
                        .ConfigureAwait(false);
                }

                return await response
                    .WriteBadHttpRestClientResponse<IAsyncEnumerable<TResult>>()
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

        ///<inheritdoc/>
        public async Task<HttpRestClientResponse> SendAsync(
            IHttpRestClientRequest request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder
                    .WriteHttpRequestMessageAsync(request, HttpClient, serializerOptions)
                    .ConfigureAwait(false);

                using var response = await HttpClient
                    .SendAsync(httpRequest, cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(response.ReadHttpResponseHeaders())
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await response
                    .WriteBadHttpRestClientResponse()
                    .ConfigureAwait(false);
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

        ///<inheritdoc/>
        public async Task<HttpRestClientResponse<TResult>> SendAsync<TResult>(
            IHttpRestClientRequest<TResult> request,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpRequest = await _httpRestClientRequestBuilder
                    .WriteHttpRequestMessageAsync(request, HttpClient, serializerOptions)
                    .ConfigureAwait(false);

                using var response = await HttpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await _httpRestClientResponseBuilder.WriteSuccessResultResponseAsync<TResult>(
                        response,
                        serializerOptions,
                        cancellationToken)
                        .ConfigureAwait(false);
                }

                return await response
                    .WriteBadHttpRestClientResponse<TResult>()
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
