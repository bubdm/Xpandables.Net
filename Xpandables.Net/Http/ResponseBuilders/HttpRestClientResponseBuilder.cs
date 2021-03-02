
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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The <see cref="HttpRestClientResponse"/> builder.
    /// </summary>
    public class HttpRestClientResponseBuilder : IHttpRestClientResponseBuilder
    {
        /// <summary>
        /// De-serializes a JSON string from stream.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Reading stream failed. See inner exception.</exception>
        public virtual async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream)
        {
            var result = await JsonSerializer.DeserializeAsync<TResult>(
                   stream, new JsonSerializerOptions { AllowTrailingCommas = false, WriteIndented = false, PropertyNameCaseInsensitive = true })
                   .ConfigureAwait(false);
#nullable disable
            return result;
#nullable enable
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse" /> for bad response.
        /// </summary>
        /// <param name="badResponseBuilder">The bad response content builder.</param>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse" />.</returns>
        public virtual async Task<HttpRestClientResponse> WriteBadResultResponseAsync(Func<Exception, HttpStatusCode, HttpRestClientResponse> badResponseBuilder, HttpResponseMessage httpResponse)
        {
            var response = httpResponse.Content switch
            {
                { } => await WriteBadResponseContentAsync().ConfigureAwait(false),
                null => badResponseBuilder(new HttpRestClientException(), httpResponse.StatusCode)
            };

            return response
                .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);

            async Task<HttpRestClientResponse> WriteBadResponseContentAsync()
            {
                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return badResponseBuilder(new HttpRestClientException(content), httpResponse.StatusCode);
            }
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse" /> for success response that contains an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="streamToResponseConverter">The converter to be used from stream to <typeparamref name="TResult" />.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse" />.</returns>
        public virtual async Task<HttpRestClientResponse<TResult>> WriteSuccessAsyncEnumerableResponseAsync<TResult>(HttpResponseMessage httpResponse, Func<Stream, TResult> streamToResponseConverter)
        {
            try
            {
                var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (stream is null)
                {
                    return HttpRestClientResponse<TResult>
                        .Success(httpResponse.StatusCode)
                        .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                        .AddVersion(httpResponse.Version)
                        .AddReasonPhrase(httpResponse.ReasonPhrase);
                }

                var results = streamToResponseConverter(stream);
                return HttpRestClientResponse<TResult>
                    .Success(results, httpResponse.StatusCode)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResult>
                    .Failure(exception)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        /// <summary>
        /// Returns an <see cref="HttpRestClientResponse" /> for success response that contains a result of <typeparamref name="TResult" /> type.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="streamToResponseConverter">The converter to be used from stream to <typeparamref name="TResult" />.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse" />.</returns>
        public virtual async Task<HttpRestClientResponse<TResult>> WriteSuccessResultResponseAsync<TResult>(HttpResponseMessage httpResponse, Func<Stream, Task<TResult>> streamToResponseConverter)
        {
            try
            {
                var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (stream is null)
                {
                    return HttpRestClientResponse<TResult>
                        .Success(httpResponse.StatusCode)
                        .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                        .AddVersion(httpResponse.Version)
                        .AddReasonPhrase(httpResponse.ReasonPhrase);
                }

                var results = await streamToResponseConverter(stream).ConfigureAwait(false);
                return HttpRestClientResponse<TResult>
                    .Success(results, httpResponse.StatusCode)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
            catch (Exception exception)
            {
                return HttpRestClientResponse<TResult>
                    .Failure(exception)
                    .AddHeaders(ReadHttpResponseHeaders(httpResponse))
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }
        }

        /// <summary>
        /// Returns headers from the HTTP response when return <see cref="HttpRestClientResponse"/>.
        /// </summary>
        /// <param name="httpResponse">The response to act on.</param>
        /// <returns>A collection of keys/values.</returns>
        public virtual NameValueCollection ReadHttpResponseHeaders(HttpResponseMessage httpResponse)
            => Enumerable
                .Empty<(string Name, string Value)>()
                .Concat(
                    httpResponse.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            )
                        )
                .Concat(
                    httpResponse.Content.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                        )
                        )
                .Aggregate(
                    seed: new NameValueCollection(),
                    func: (nvc, pair) =>
                    {
                        var (name, value) = pair;
                        nvc.Add(name, value); return nvc;
                    },
                    resultSelector: nvc => nvc
                    );
    }
}
