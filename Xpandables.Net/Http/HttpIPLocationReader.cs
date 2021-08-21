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
using System.Text.Json;

namespace Xpandables.Net.Http;

/// <summary>
/// Default implementation for <see cref="IHttpIPLocationReader"/>.
/// </summary>
public class HttpIPLocationReader : IHttpIPLocationReader
{
    private readonly IHttpClientDispatcher _httpClientDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpIPLocationReader"/> class 
    /// that uses the http://api.ipstack.com to retrieve the user location.
    /// </summary>
    /// <param name="httpClientDispatcher">The target handler.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="httpClientDispatcher"/> is null.</exception>
    public HttpIPLocationReader(IHttpClientDispatcher httpClientDispatcher)
    {
        _httpClientDispatcher = httpClientDispatcher ?? throw new ArgumentNullException(nameof(httpClientDispatcher));
    }

    ///<inheritdoc/>
    public virtual async Task<HttpRestClientResponse<IPLocationResponse>> ReadLocationAsync(
        IPLocationRequest request,
        JsonSerializerOptions? serializerOptions = default,
        CancellationToken cancellationToken = default)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        return await _httpClientDispatcher.SendAsync(
            request,
            serializerOptions,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
