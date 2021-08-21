
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
using System.Text.Json;

namespace Xpandables.Net.Http;

/// <summary>
/// Provides with a method to read IP address of the caller.
/// </summary>
public interface IHttpIPAddressReader
{
    /// <summary>
    /// Asynchronously gets the IPAddress of the current caller.
    /// </summary>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    Task<HttpRestClientResponse<IPAddress>> ReadIPAddressAsync(
        JsonSerializerOptions? serializerOptions = default, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a query to request an <see cref="IPAddress"/>.
/// </summary>
[HttpClient(Path = "", IsNullable = true, IsSecured = false, Method = HttpMethodVerbs.Get)]
public sealed class IPAddressRequest : IHttpClientRequest<string> { }
