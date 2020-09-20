
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Xpandables.Net.Commands;
using Xpandables.Net.Optionals;
using Xpandables.Net.Queries;

using static Xpandables.Net.HttpRestClient.HttpRestClientExtensions;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client.
    /// The queries and commands should implement one of the following interfaces :
    /// <see cref="IStringRequest"/>, <see cref="IStreamRequest"/>, <see cref="IByteArrayRequest"/>, <see cref="IFormUrlEncodedRequest"/>,
    /// <see cref="IMultipartRequest"/> and <see cref="IQueryStringRequest"/>.
    /// <para>You should register the handler using one of the extension methods 
    /// <see langword="AddHttpClient{TClient, TImplementation}(IServiceCollection)"/> and you may add 
    /// <see langword="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{HttpClient})"/> or other 
    /// to customize the client behaviors.</para>
    /// </summary>
    public interface IHttpRestClientHandler : IDisposable
    {
        /// <summary>
        /// Contains the <see cref="HttpContent"/> instance for the current handler.
        /// </summary>
        internal HttpClient HttpClient { get; }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// Be aware of the fact that the target must return an <see cref="IAsyncEnumerable{T}"/>, otherwise you will face an exception.
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
                using var request = await GetHttpRequestMessageAsync(query, cancellationToken).ConfigureAwait(false);
                // Due to the fact that the result is an IAsyncEnumerable, the response can not be disposed before.
                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await GetResponseAsync<IAsyncEnumerable<TResult>, TResult>(response, stream =>
                        System.Linq.AsyncEnumerable.Create(cancellation => GetEnumerableFromStreamAsync<TResult>(stream, cancellation).GetAsyncEnumerator(cancellationToken)));

                return (HttpRestClientResponse<IAsyncEnumerable<TResult>>)await GetBadResponseAsync(
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
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        public async Task<HttpRestClientResponse<Optional<TResult>>> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            _ = HttpClient ?? throw new InvalidOperationException($"The HTTP client needs to be initialized.");

            try
            {
                using var request = await GetHttpRequestMessageAsync(query, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await GetResponseAsync<Optional<TResult>, TResult>(response, stream => stream.DeserializeJsonFromStream<TResult>()).ConfigureAwait(false);

                return (HttpRestClientResponse<Optional<TResult>>)await GetBadResponseAsync(
                    HttpRestClientResponse<Optional<TResult>>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is InvalidOperationException
                                            || exception is OperationCanceledException
                                            || exception is HttpRequestException
                                            || exception is TaskCanceledException)
            {
                return HttpRestClientResponse<Optional<TResult>>.Failure(exception);
            }
        }

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
                using var request = await GetHttpRequestMessageAsync(command, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(GetHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await GetBadResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
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
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/> or implements the <see cref="IHttpRestClientAttributeProvider"/> interface.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns a task <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public async Task<HttpRestClientResponse> HandleAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _ = HttpClient ?? throw new InvalidOperationException($"The HTTP client needs to be initialized.");

            try
            {
                using var request = await GetHttpRequestMessageAsync(command, cancellationToken).ConfigureAwait(false);
                using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(GetHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await GetBadResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
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

        private static async Task<HttpRestClientResponse<TResponse>> GetResponseAsync<TResponse, TElement>(HttpResponseMessage httpResponse, Func<Stream, TResponse> streamConverter)
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
                            .AddHeaders(GetHttpResponseHeaders(httpResponse))
                            .AddVersion(httpResponse.Version)
                            .AddReasonPhrase(httpResponse.ReasonPhrase);
                    }
                }

                return HttpRestClientResponse<TResponse>
                    .Success(httpResponse.StatusCode)
                    .AddHeaders(GetHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResponse>
                    .Failure(exception, HttpStatusCode.BadRequest)
                    .AddHeaders(GetHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        private async static Task<HttpRestClientResponse> GetBadResponseAsync(Func<Exception, HttpStatusCode, HttpRestClientResponse> responseBuilder, HttpResponseMessage httpResponse)
        {
            var response = httpResponse.Content switch
            {
                { } => await GetBadResponseContentAsync().ConfigureAwait(false),
                null => responseBuilder(new HttpRestClientException(), httpResponse.StatusCode)
            };

            return response
                .AddHeaders(GetHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);

            async Task<HttpRestClientResponse> GetBadResponseContentAsync()
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return responseBuilder(new HttpRestClientException(content), httpResponse.StatusCode);
            }
        }

        private static async IAsyncEnumerable<TResult> GetEnumerableFromStreamAsync<TResult>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var jsonSerializer = JsonSerializer.CreateDefault();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (jsonTextReader.TokenType == JsonToken.StartObject)
                    yield return jsonSerializer.Deserialize<TResult>(jsonTextReader)!;
            }
        }
    }
}
