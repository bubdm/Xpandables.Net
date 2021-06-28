
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
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with <see cref="IHttpRestClientHandler"/> response builder.
    /// </summary>
    public interface IHttpRestClientResponseBuilder
    {
        /// <summary>
        /// The main method to build an <see cref="HttpRestClientResponse"/> for response 
        /// that contains an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        Task<HttpRestClientResponse<IAsyncEnumerable<TResult>>> WriteAsyncEnumerableResponseAsync<TResult>(
            HttpResponseMessage httpResponse,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// The main method to build an <see cref="HttpRestClientResponse"/> for success response 
        /// that contains a result of <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">The response content type.</typeparam>
        /// <param name="httpResponse">The target HTTP response.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An instance of <see cref="HttpRestClientResponse"/>.</returns>
        Task<HttpRestClientResponse<TResult>> WriteSuccessResultResponseAsync<TResult>(
            HttpResponseMessage httpResponse,
            JsonSerializerOptions? serializerOptions = default,
            CancellationToken cancellationToken = default);
    }
}
