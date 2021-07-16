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

using Microsoft.JSInterop;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Storage
{
    internal class InternalStorage : IStorage
    {
        private readonly IJSRuntime _jSRuntime;

        public InternalStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime ?? throw new ArgumentNullException(nameof(jSRuntime));
        }

        public ValueTask ClearAllAsync(string command, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeVoidAsync(command, cancellationToken);

        public ValueTask<bool> ContainKeyAsync(string command, string key, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeAsync<bool>(command, cancellationToken, key);

        public ValueTask<int> CountAsync(string command, string key, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeAsync<int>(key, cancellationToken, command);

        public ValueTask<TValue?> ReadAsync<TValue>(string command, string key, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeAsync<TValue?>(command, cancellationToken, key);

        public ValueTask RemoveAsync(string command, string key, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeVoidAsync(command, cancellationToken, key);

        public ValueTask WriteAsync<TValue>(string command, string key, TValue value, CancellationToken cancellationToken = default)
            => _jSRuntime.InvokeVoidAsync(command, cancellationToken, key, value);
    }
}
