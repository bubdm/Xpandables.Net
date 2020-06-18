
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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// Default implementation for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public sealed class HttpRestClientHandler : Disposable, IHttpRestClientHandler
    {
        private HttpClient? _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientHandler"/> class with the HTTP client factory.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpClientFactory"/> is null.</exception>
        public HttpRestClientHandler(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        /// <summary>
        /// Initializes the HttpClient with the configuration matching the name.
        /// This method must to be called before any use of handlers.
        /// </summary>
        /// <param name="configurationName">The name of the configuration used for registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationName"/> is null.</exception>
        public IHttpRestClientHandler Initialize(string configurationName)
        {
            _ = configurationName ?? throw new ArgumentNullException(nameof(configurationName));
            _httpClient = _httpClientFactory.CreateClient(configurationName);
            return this;
        }

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns an <see cref="HttpRestClientResponse{TResult}" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query" /> is null.</exception>
        public async Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            if (_httpClient is null)
                throw new InvalidOperationException($"The handler needs to be initializes by calling the {nameof(Initialize)} method.");

            try
            {
                using var request = GetHttpRequestMessage(query);
                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return await GetHttpRestClientResponseAsync<TResult>(response).ConfigureAwait(false);

                return (HttpRestClientResponse<TResult>)await GetHttpRestClientBadResponseAsync(
                    HttpRestClientResponse<TResult>.Failure, response)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is InvalidOperationException
                                            || exception is HttpRequestException)
            {
                return HttpRestClientResponse<TResult>.Failure(exception);
            }
        }

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns an <see cref="HttpRestClientResponse" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        public async Task<HttpRestClientResponse> HandleAsync(ICommand command, CancellationToken cancellationToken)
        {
            if (_httpClient is null)
                throw new InvalidOperationException($"The handler needs to be initializes by calling the {nameof(Initialize)} method.");

            try
            {
                using var request = GetHttpRequestMessage(command);
                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return HttpRestClientResponse
                        .Success()
                        .AddHeaders(GetHttpResponseHeaders(response))
                        .AddVersion(response.Version)
                        .AddReasonPhrase(response.ReasonPhrase);
                }

                return await GetHttpRestClientBadResponseAsync(HttpRestClientResponse.Failure, response).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is InvalidOperationException
                                       || exception is HttpRequestException)
            {
                return HttpRestClientResponse.Failure(exception);
            }
        }

        private async Task<HttpRestClientResponse<TResult>> GetHttpRestClientResponseAsync<TResult>(HttpResponseMessage httpResponse)
        {
            try
            {
                if (httpResponse.Content is { })
                {
                    var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (content is { })
                    {
                        var result = JsonSerializer.Deserialize<TResult>(content)!;
                        return HttpRestClientResponse<TResult>
                            .Success(result, httpResponse.StatusCode)
                            .AddHeaders(GetHttpResponseHeaders(httpResponse))
                            .AddVersion(httpResponse.Version)
                            .AddReasonPhrase(httpResponse.ReasonPhrase);
                    }
                }

                return HttpRestClientResponse<TResult>
                    .Success(httpResponse.StatusCode)
                    .AddHeaders(GetHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResult>
                    .Failure(exception, HttpStatusCode.BadRequest)
                    .AddHeaders(GetHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private async static Task<HttpRestClientResponse> GetHttpRestClientBadResponseAsync(
               Func<Exception, HttpStatusCode, HttpRestClientResponse> responseBuilder, HttpResponseMessage httpResponse)
        {
            HttpRestClientResponse? response = default;
            if (httpResponse.Content is { })
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (content.TryDeserializeObject<HttpRestClientValidationException>(out var validationException, out _))
                {
                    if (validationException is { })
                        response = responseBuilder(validationException, httpResponse.StatusCode);
                }
                else if (content.TryDeserializeObject<InvalidOperationException>(out var invalidException, out _))
                {
                    if (invalidException is { })
                        response = responseBuilder(invalidException, httpResponse.StatusCode);
                }
                else if (content.TryDeserializeObject<Exception>(out var exception, out _))
                {
                    if (exception is { })
                        response = responseBuilder(exception, httpResponse.StatusCode);
                }

                if (response is null)
                {
                    var otherException = new InvalidOperationException($"{content}{Environment.NewLine}{httpResponse.ReasonPhrase}");
                    response = responseBuilder(otherException, httpResponse.StatusCode);
                }
            }
            else
            {
                response = responseBuilder(new NotSupportedException("Null content"), httpResponse.StatusCode);
            }

            return response
                .AddHeaders(GetHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }

        private static HttpRequestMessage GetHttpRequestMessage<TSource>(TSource source)
            where TSource : class
        {
            var attribute = GetHttpClientDescriptionAttribute(source);
            return attribute.GetHttpRequestMessage(source);
        }

        private static HttpRestClientAttribute GetHttpClientDescriptionAttribute<TSource>(TSource source)
            where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.GetHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                         ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        private static NameValueCollection GetHttpResponseHeaders(HttpResponseMessage httpResponse)
            => Enumerable
                .Empty<(string Name, string Value)>()
                .Concat(
                    httpResponse.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            )
                        )
                .Concat(
                    httpResponse.Content?.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            ) ?? Enumerable.Empty<(string, string)>()
                        )
                .Aggregate(
                    seed: new NameValueCollection(),
                    func: (nvc, pair) => { nvc.Add(pair.Name, pair.Value); return nvc; },
                    resultSelector: nvc => nvc
                    );

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
                _httpClient?.Dispose();

            _isDisposed = true;
        }
    }
}
