
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
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Provides with a handler that can be used with <see cref="HttpClient"/> to format IpLocation result before returning response.
    /// </summary>
    public sealed class HttpRestClientIPGeoAddressMessage : HttpClientHandler
    {
        /// <summary>
        /// Creates an instance of System.Net.Http.HttpResponseMessage based on the information
        /// provided in the System.Net.Http.HttpRequestMessage as an operation that will not block.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The token is not available. See inner exception.</exception>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content!.ReadAsStringAsync().ConfigureAwait(false);
                content = $"'{ content.Replace("\n", "")}'";
                response.Content = new StringContent(content, Encoding.UTF8);
            }

            return response;
        }
    }
}
