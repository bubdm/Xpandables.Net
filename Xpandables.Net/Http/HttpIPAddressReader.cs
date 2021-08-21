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
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

[assembly: InternalsVisibleTo("Xpandables.Net.DependencyInjection, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]

namespace Xpandables.Net.Http;

/// <summary>
/// Provides with a handler that is used with <see cref="HttpClient"/> to format IpLocation result before returning response.
/// </summary>
internal sealed class HttpIPAddressDelegateHandler : HttpClientHandler
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
        if (response.StatusCode != HttpStatusCode.OK) return response;

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        content = $"'{ content.Replace("\n", "", StringComparison.InvariantCulture)}'";
        response.Content = new StringContent(content, Encoding.UTF8);

        return response;
    }
}

/// <summary>
/// Default implementation for <see cref="IHttpIPAddressReader"/>.
/// </summary>
public class HttpIPAddressReader : IHttpIPAddressReader
{
    private readonly IHttpClientDispatcher _httpClientDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpIPAddressReader"/> class that 
    /// uses the https://ipinfo.io/ip to retrieve the user IP address.
    /// </summary>
    /// <param name="httpClientDispatcher">The target handler.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="httpClientDispatcher"/> is null.</exception>
    public HttpIPAddressReader(IHttpClientDispatcher httpClientDispatcher)
    {
        _httpClientDispatcher = httpClientDispatcher ?? throw new ArgumentNullException(nameof(httpClientDispatcher));
    }

    ///<inheritdoc/>
    public virtual async Task<HttpRestClientResponse<IPAddress>> ReadIPAddressAsync(
        JsonSerializerOptions? serializerOptions = default,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientDispatcher.SendAsync(
            new IPAddressRequest(),
            serializerOptions,
            cancellationToken)
            .ConfigureAwait(false);

        return response.ConvertTo(IPAddress.TryParse(response.Result, out var ipAddress) ? ipAddress : IPAddress.None);
    }
}
