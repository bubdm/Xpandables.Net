
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
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The async-enumerable builder.
    /// </summary>
    public class HttpRestClientAsyncEnumerableBuilder : IHttpRestClientAsyncEnumerableBuilder
    {
        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TResult" /> that can be asynchronously enumerated.</returns>
        public virtual async IAsyncEnumerable<TResult> AsyncEnumerableBuilderFromStreamAsync<TResult>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var blockingCollection = new BlockingCollection<TResult>();
            await using var iterator = new AsyncEnumerable<TResult>(blockingCollection.GetConsumingEnumerable(cancellationToken)).GetAsyncEnumerator(cancellationToken);

            var enumerateStreamElementToBlockingCollectionThread = new Thread(() => EnumerateStreamElementToBlockingCollection(stream, blockingCollection, cancellationToken));
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
        private static void EnumerateStreamElementToBlockingCollection<TResult>(Stream stream, BlockingCollection<TResult> resultCollection, CancellationToken cancellationToken)
        {
            using var jsonStreamReader = new Utf8JsonStreamReader(stream, 32 * 1024);

            jsonStreamReader.Read();
            while (jsonStreamReader.Read())
            {
                if (cancellationToken.IsCancellationRequested) break;
                if (jsonStreamReader.TokenType != JsonTokenType.StartObject)
                    continue;
                if (jsonStreamReader.Deserialise<TResult>(new JsonSerializerOptions { AllowTrailingCommas = false, WriteIndented = false, PropertyNameCaseInsensitive = true }) is { } result)
                    resultCollection.Add(result, cancellationToken);
            }

            resultCollection.CompleteAdding();
        }
    }
}
