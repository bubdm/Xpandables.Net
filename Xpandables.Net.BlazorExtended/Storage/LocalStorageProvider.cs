
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

using Microsoft.JSInterop;

namespace Xpandables.Net.Storage
{
    internal class LocalStorageProvider : ILocalStorageProvider
    {
        private readonly IJSRuntime _jSRuntime;

        public LocalStorageProvider(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime ?? throw new ArgumentNullException(nameof(jSRuntime));
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeVoidAsync("localStorage.clear", cancellationToken);

        public async Task<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key, cancellationToken);

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeAsync<int>("eval", "localStorage.length", cancellationToken);

        public async Task<string?> ReadAsync(string key, CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key, cancellationToken);

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", key, cancellationToken);

        public async Task WriteAsync(string key, string value, CancellationToken cancellationToken = default)
            => await _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }
}
