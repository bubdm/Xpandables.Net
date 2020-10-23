
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IHttpRestClientHandler"/> interface.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public sealed class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        private readonly IHttpRestClientEngine _httpRestClientEngine;

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> current instance.
        /// </summary>
        public HttpClient HttpClient { get; }

        IHttpRestClientEngine IHttpRestClientHandler.HttpRestClientEngine => _httpRestClientEngine;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientHandler"/> class with the HTTP typed client.
        /// </summary>
        /// <param name="httpClient">The HTTP client type to be used.</param>
        /// <param name="httpRestClientEngine">The HTTP rest client engine.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClient"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRestClientEngine"/> is null.</exception>/// 
        public HttpRestClientHandler(HttpClient httpClient, IHttpRestClientEngine httpRestClientEngine)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpRestClientEngine = httpRestClientEngine ?? throw new ArgumentNullException(nameof(httpRestClientEngine));
        }

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
            _ = HttpClient ?? throw new InvalidOperationException($"The HTTP client needs to be initialized.");

            try
            {
                using var request = await _httpRestClientEngine.WriteHttpRequestMessageAsync(query, cancellationToken).ConfigureAwait(false);
                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResponseAsync<IAsyncEnumerable<TResult>, TResult>(response, stream =>
                        new AsyncEnumerable<TResult>(cancellation => _httpRestClientEngine.ReadAsyncEnumerableFromStreamAsync<TResult>(stream, cancellationToken).GetAsyncEnumerator(cancellationToken)));

                return (HttpRestClientResponse<IAsyncEnumerable<TResult>>)await WriteBadResponseAsync(
                    HttpRestClientResponse<IAsyncEnumerable<TResult>>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
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
            _ = HttpClient ?? throw new InvalidOperationException($"The HTTP client needs to be initialized.");

            try
            {
                using var request = await _httpRestClientEngine.WriteHttpRequestMessageAsync(command, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(_httpRestClientEngine.ReadHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await WriteBadResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse.Failure(exception);
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
            _ = HttpClient ?? throw new InvalidOperationException($"The HTTP client needs to be initialized.");

            try
            {
                using var request = await _httpRestClientEngine.WriteHttpRequestMessageAsync(query, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await WriteResponseAsync<TResult, TResult>(response, stream => _httpRestClientEngine.DeserializeJsonFromStreamAsync<TResult>(stream, cancellationToken).RunSync()).ConfigureAwait(false);

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

        private async Task<HttpRestClientResponse<TResponse>> WriteResponseAsync<TResponse, TElement>(HttpResponseMessage httpResponse, Func<Stream, TResponse> streamConverter)
        {
            try
            {
                if (httpResponse.Content is { })
                {
                    var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    if (stream is { })
                    {
                        var results = streamConverter(stream);
                        return HttpRestClientResponse<TResponse>
                            .Success(results, httpResponse.StatusCode)
                            .AddHeaders(_httpRestClientEngine.ReadHttpResponseHeaders(httpResponse))
                            .AddVersion(httpResponse.Version)
                            .AddReasonPhrase(httpResponse.ReasonPhrase);
                    }
                }

                return HttpRestClientResponse<TResponse>
                    .Success(httpResponse.StatusCode)
                    .AddHeaders(_httpRestClientEngine.ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResponse>
                    .Failure(exception, HttpStatusCode.BadRequest)
                    .AddHeaders(_httpRestClientEngine.ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        private async Task<HttpRestClientResponse> WriteBadResponseAsync(Func<Exception, HttpStatusCode, HttpRestClientResponse> responseBuilder, HttpResponseMessage httpResponse)
        {
            var response = httpResponse.Content switch
            {
                { } => await WriteBadResponseContentAsync().ConfigureAwait(false),
                null => responseBuilder(new HttpRestClientException(), httpResponse.StatusCode)
            };

            return response
                .AddHeaders(_httpRestClientEngine.ReadHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);

            async Task<HttpRestClientResponse> WriteBadResponseContentAsync()
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return responseBuilder(new HttpRestClientException(content), httpResponse.StatusCode);
            }
        }
    }
}
