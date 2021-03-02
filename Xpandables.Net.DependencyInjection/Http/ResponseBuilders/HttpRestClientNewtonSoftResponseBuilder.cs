
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
using System.IO;
using System.Threading.Tasks;

namespace Xpandables.Net.Http.ResponseBuilders
{
    /// <summary>
    /// The <see cref="HttpRestClientResponse"/> builder  using NewtonSoft.
    /// </summary>
    public class HttpRestClientNewtonSoftResponseBuilder : HttpRestClientResponseBuilder
    {
        /// <summary>
        /// De-serializes a JSON string from stream.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized object.</typeparam>
        /// <param name="stream">The stream to act on.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Reading stream failed. See inner exception.</exception>
        public override async Task<TResult> DeserializeJsonFromStreamAsync<TResult>(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var result = JsonSerializer.CreateDefault().Deserialize<TResult>(jsonTextReader);
#nullable disable
            return await Task.FromResult(result).ConfigureAwait(false);
#nullable enable
        }
    }
}
