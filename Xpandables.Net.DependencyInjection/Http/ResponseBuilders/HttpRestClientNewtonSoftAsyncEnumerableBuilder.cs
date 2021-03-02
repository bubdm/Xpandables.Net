
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

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The async-enumerable builder using Newtonsoft.
    /// </summary>
    public class HttpRestClientNewtonSoftAsyncEnumerableBuilder : HttpRestClientAsyncEnumerableBuilder
    {
        /// <summary>
        /// Returns an async-enumerable from stream used for asynchronous result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="stream">The stream source to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An enumerator of <typeparamref name="TResult" /> that can be asynchronously enumerated.</returns>
        public override async IAsyncEnumerable<TResult> AsyncEnumerableBuilderFromStreamAsync<TResult>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var jsonSerializer = JsonSerializer.CreateDefault();
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
}
