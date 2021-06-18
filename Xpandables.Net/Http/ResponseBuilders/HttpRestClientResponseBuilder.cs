
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The <see cref="HttpRestClientResponse"/> builder.
    /// You must derive from this class in order to customize its behaviors.
    /// </summary>
    public class HttpRestClientResponseBuilder : IHttpRestClientResponseBuilder
    {
        ///<inheritdoc/>
        public virtual async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream)
        {
            var result = await JsonSerializer.DeserializeAsync<TResult>(
                   stream, new JsonSerializerOptions
                   {
                       AllowTrailingCommas = false,
                       WriteIndented = false,
                       PropertyNameCaseInsensitive = true
                   })
                   .ConfigureAwait(false);
            return result!;
        }

        ///<inheritdoc/>
        public virtual async Task<HttpRestClientResponse> WriteBadResultResponseAsync(
            Func<Exception, HttpStatusCode, HttpRestClientResponse> badResponseBuilder,
            HttpResponseMessage httpResponse)
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

        ///<inheritdoc/>
        public virtual async Task<HttpRestClientResponse<TResult>> WriteSuccessAsyncEnumerableResponseAsync<TResult>(
            HttpResponseMessage httpResponse,
            Func<Stream, TResult> streamToResponseConverter)
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

        ///<inheritdoc/>
        public virtual async Task<HttpRestClientResponse<TResult>> WriteSuccessResultResponseAsync<TResult>(
            HttpResponseMessage httpResponse,
            Func<Stream, Task<TResult>> streamToResponseConverter)
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public virtual async IAsyncEnumerable<TResult> AsyncEnumerableBuilderFromStreamAsync<TResult>(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var blockingCollection = new BlockingCollection<TResult>();
            await using var iterator = new AsyncEnumerable<TResult>(
                blockingCollection.GetConsumingEnumerable(cancellationToken))
                .GetAsyncEnumerator(cancellationToken);

            var enumerateStreamElementToBlockingCollectionThread = new Thread(
                () => EnumerateStreamElementToBlockingCollection(stream, blockingCollection, cancellationToken));

            enumerateStreamElementToBlockingCollectionThread.Start();

            while (await iterator.MoveNextAsync().ConfigureAwait(false))
                yield return iterator.Current;
        }

        /// <summary>
        /// Enumerates the stream content to the blocking collection used to return <see cref="IAsyncEnumerable{T}"/>.
        /// the method makes use of <see cref="Utf8JsonStreamReader"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the collection item.</typeparam>
        /// <param name="stream">The target stream.</param>
        /// <param name="resultCollection">The collection result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        private static void EnumerateStreamElementToBlockingCollection<TResult>(
            Stream stream,
            BlockingCollection<TResult> resultCollection,
            CancellationToken cancellationToken)
        {
            using var jsonStreamReader = new Utf8JsonStreamReader(stream, 32 * 1024);

            jsonStreamReader.Read();
            while (jsonStreamReader.Read())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (jsonStreamReader.TokenType != JsonTokenType.StartObject)
                    continue;

                if (jsonStreamReader.Deserialise<TResult>(new JsonSerializerOptions
                {
                    AllowTrailingCommas = false,
                    WriteIndented = false,
                    PropertyNameCaseInsensitive = true
                }) is { } result)
                {
                    resultCollection.Add(result, cancellationToken);
                }
            }

            resultCollection.CompleteAdding();
        }
    }
}
