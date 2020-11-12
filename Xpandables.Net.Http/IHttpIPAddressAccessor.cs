
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
using System.Net;
using System.Threading.Tasks;

using Xpandables.Net.HttpRest;

namespace Xpandables.Net.Http.Network
{
    /// <summary>
    /// Provides with a method to request IP address.
    /// </summary>
    public interface IHttpIPAddressAccessor : IDisposable
    {
        internal IHttpRestClientHandler HttpRestClientHandler { get; }

        /// <summary>
        /// Asynchronously gets the IPAddress of the current caller.
        /// </summary>
        public virtual async Task<HttpRestClientResponse<IPAddress>> ReadIPAddressAsync()
        {
            var response = await HttpRestClientHandler.HandleAsync(new IPAddressRequest()).ConfigureAwait(false);
            return response.ConvertTo(IPAddress.TryParse(response.Result, out var ipAddress) ? ipAddress : IPAddress.None);
        }
    }
}
