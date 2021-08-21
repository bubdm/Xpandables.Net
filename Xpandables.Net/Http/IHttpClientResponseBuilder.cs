
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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Xpandables.Net.Http;

/// <summary>
/// Provides with <see cref="IHttpClientDispatcher"/> response builder.
/// </summary>
public interface IHttpClientResponseBuilder
{
    /// <summary>
    /// The main method to build an <see cref="HttpClientResponse"/> for response 
    /// that contains an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TResult">The response content type.</typeparam>
    /// <param name="httpResponse">The target HTTP response.</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An instance of <see cref="HttpClientResponse"/>.</returns>
    public virtual async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>>
        WriteAsyncEnumerableResponseAsync<TResult>(
        HttpResponseMessage httpResponse,
        JsonSerializerOptions? serializerOptions = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await httpResponse.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            if (stream is null)
            {
                return HttpRestClientResponse<IAsyncEnumerable<TResult>>
                    .Success(AsyncEnumerable<TResult>.Empty(), httpResponse.StatusCode)
                    .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }

            var results = AsyncEnumerableBuilderAsync(stream, serializerOptions, cancellationToken);

            return HttpRestClientResponse<IAsyncEnumerable<TResult>>
                .Success(results, httpResponse.StatusCode)
                .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }
        catch (Exception exception)
        {
            return HttpRestClientResponse<IAsyncEnumerable<TResult>>
                .Failure(exception)
                .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }

        async static IAsyncEnumerable<TResult> AsyncEnumerableBuilderAsync(
            Stream stream,
            JsonSerializerOptions? serializerOptions = default,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var blockingCollection = new BlockingCollection<TResult>();
            await using var blockingCollectionIterator = new AsyncEnumerable<TResult>(
                blockingCollection.GetConsumingEnumerable(cancellationToken))
                .GetAsyncEnumerator(cancellationToken);

            await Task.Run(() =>
                EnumerateStreamElementToBlockingCollection(
                    stream,
                    blockingCollection,
                    serializerOptions,
                    cancellationToken), cancellationToken);

            while (await blockingCollectionIterator.MoveNextAsync().ConfigureAwait(false))
                yield return blockingCollectionIterator.Current;
        }

        static void EnumerateStreamElementToBlockingCollection(
            Stream stream,
            BlockingCollection<TResult> resultCollection,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default)
        {
            using var jsonStreamReader = new Utf8JsonStreamReader(stream, 32 * 1024);

            jsonStreamReader.Read();
            while (jsonStreamReader.Read())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (jsonStreamReader.TokenType != JsonTokenType.StartObject)
                    continue;

                if (jsonStreamReader.Deserialise<TResult>(serializerOptions) is { } result)
                {
                    resultCollection.Add(result, cancellationToken);
                }
            }

            resultCollection.CompleteAdding();
        }
    }

    /// <summary>
    /// The main method to build an <see cref="HttpClientResponse"/> for success response 
    /// that contains a result of <typeparamref name="TResult"/> type.
    /// </summary>
    /// <typeparam name="TResult">The response content type.</typeparam>
    /// <param name="httpResponse">The target HTTP response.</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An instance of <see cref="HttpClientResponse"/>.</returns>
    public virtual async Task<HttpRestClientResponse<TResult>> WriteSuccessResultResponseAsync<TResult>(
       HttpResponseMessage httpResponse,
       JsonSerializerOptions? serializerOptions = default,
       CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

            if (stream is null)
            {
                return HttpRestClientResponse<TResult>
                    .Success(httpResponse.StatusCode)
                    .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                    .AddVersion(httpResponse.Version)
                    .AddReasonPhrase(httpResponse.ReasonPhrase);
            }

            var result = await JsonSerializer.DeserializeAsync<TResult>(
                stream,
                serializerOptions,
                cancellationToken)
                .ConfigureAwait(false);

            return HttpRestClientResponse<TResult>
                .Success(result, httpResponse.StatusCode)
                .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }
        catch (Exception exception)
        {
            return HttpRestClientResponse<TResult>
                .Failure(exception)
                .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                .AddVersion(httpResponse.Version)
                .AddReasonPhrase(httpResponse.ReasonPhrase);
        }
    }
}

/// <summary>
/// The default <see cref="HttpClientResponse"/> builder.
/// You must derive from this class in order to customize its behaviors.
/// </summary>
public class HttpClientResponseBuilder : IHttpClientResponseBuilder { }