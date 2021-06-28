
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
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The <see cref="HttpRestClientResponse"/> builder  using NewtonSoft.
    /// </summary>
    public class HttpRestClientNewtonsoftResponseBuilder : HttpRestClientResponseBuilder
    {
        ///<inheritdoc/>
        public override async Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>>
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
                        .Success(AsyncEnumerableExtensions.Empty<TResult>(), httpResponse.StatusCode)
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
                var jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                while (await jsonTextReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (jsonTextReader.TokenType != JsonToken.StartObject) continue;
                    if (jsonSerializer.Deserialize<TResult>(jsonTextReader) is { } result)
                        yield return result;
                }
            }
        }

        ///<inheritdoc/>
        public override async Task<HttpRestClientResponse<TResult>> WriteSuccessResultResponseAsync<TResult>(
            HttpResponseMessage httpResponse,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                if (stream is null)
                {
                    return HttpRestClientResponse<TResult>
                        .Success(httpResponse.StatusCode)
                        .AddHeaders(httpResponse.ReadHttpResponseHeaders())
                        .AddVersion(httpResponse.Version)
                        .AddReasonPhrase(httpResponse.ReasonPhrase);
                }

                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var result = Newtonsoft.Json.JsonSerializer.CreateDefault().Deserialize<TResult>(jsonTextReader);

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
}
