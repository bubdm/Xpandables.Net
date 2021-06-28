
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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Extension methods for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientExtensions
    {
        /// <summary>
        /// Returns the headers found in the specified response.
        /// </summary>
        /// <param name="httpResponse">The response to act on.</param>
        /// <returns>An instance of <see cref="NameValueCollection"/>.</returns>
        public static NameValueCollection ReadHttpResponseHeaders(this HttpResponseMessage httpResponse)
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

        /// <summary>
        /// Returns a bad <see cref="HttpRestClientResponse"/> from the specified message.
        /// </summary>
        /// <param name="httpResponse">The message to act on.</param>
        /// <returns>Returns a task of <see cref="HttpRestClientResponse"/>.</returns>
        public static async Task<HttpRestClientResponse> WriteBadHttpRestClientResponse(this HttpResponseMessage httpResponse)
            => await CreateBadHttpRestClientResponseAsync(httpResponse, HttpRestClientResponse.Failure);

        /// <summary>
        /// Returns a bad <see cref="HttpRestClientResponse{TResult}"/> from the specified message.
        /// </summary>
        /// <param name="httpResponse">The message to act on.</param>
        /// <returns>Returns a task of <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        public static async Task<HttpRestClientResponse<TResult>> WriteBadHttpRestClientResponse<TResult>(
            this HttpResponseMessage httpResponse)
            => await CreateBadHttpRestClientResponseAsync(httpResponse, HttpRestClientResponse<TResult>.Failure);

        static async Task<TResponse> CreateBadHttpRestClientResponseAsync<TResponse>(
            HttpResponseMessage httpResponse,
            Func<Exception, HttpStatusCode, TResponse> builder)
            where TResponse : HttpRestClientResponse
        {
            _ = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));

            var httpRestClientException = await CreateHttpRestClientExceptionAsync(httpResponse).ConfigureAwait(false);

            return (TResponse)builder(httpRestClientException, httpResponse.StatusCode)
                .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }

        static async Task<HttpRestClientException> CreateHttpRestClientExceptionAsync(HttpResponseMessage httpResponse)
            => httpResponse.Content switch
            {
                { } => new HttpRestClientException(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false)),
                null => new HttpRestClientException()
            };
    }
}
