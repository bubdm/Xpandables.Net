
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net5.Commands;
using Xpandables.Net5.Queries;

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Provides with methods to handle HTTP Rest client queries and commands.
    /// The queries and commands should implement one of the following interfaces :
    /// <see cref="IStringRequest"/>, <see cref="IStreamRequest"/>, <see cref="IByteArrayRequest"/>, <see cref="IFormUrlEncodedRequest"/>,
    /// <see cref="IMultipartRequest"/> and <see cref="IQueryStringRequest"/>.
    /// </summary>
    public interface IHttpRestClientHandler : IDisposable
    {
        /// <summary>
        /// Initializes the HttpClient with the configuration matching the name.
        /// This method must to be called before any use of handlers.
        /// </summary>
        /// <param name="configurationName">The name of the configuration used for registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationName"/> is null.</exception>
        IHttpRestClientHandler Initialize(string configurationName);

        /// <summary>
        /// Handles the query as asynchronous operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to act with. The query must be decorated with the <see cref="HttpRestClientAttribute"/>.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns an <see cref="HttpRestClientResponse{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        Task<HttpRestClientResponse<TResult>> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the command as asynchronous operation.
        /// </summary>
        /// <param name="command">The command to act with. The command must be decorated with the <see cref="HttpRestClientAttribute"/></param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Returns an <see cref="HttpRestClientResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        Task<HttpRestClientResponse> HandleAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
