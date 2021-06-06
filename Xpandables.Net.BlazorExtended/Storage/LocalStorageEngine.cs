
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

using Newtonsoft.Json.Linq;

namespace Xpandables.Net.BlazorExtended.Storage
{
    internal partial class LocalStorageEngine : ILocalStorageEngine
    {
        private readonly ILocalStorageProvider _localStorageProvider;
        private readonly ILocalStorageSerializer _localStorageSerializer;

        public LocalStorageEngine(ILocalStorageProvider localStorageProvider, ILocalStorageSerializer localStorageSerializer)
        {
            _localStorageProvider = localStorageProvider ?? throw new ArgumentNullException(nameof(localStorageProvider));
            _localStorageSerializer = localStorageSerializer ?? throw new ArgumentNullException(nameof(localStorageSerializer));
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken = default)
            => await _localStorageProvider.ClearAllAsync(cancellationToken).ConfigureAwait(false);

        public async Task<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
            => await _localStorageProvider.ContainKeyAsync(key, cancellationToken).ConfigureAwait(false);

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _localStorageProvider.CountAsync(cancellationToken).ConfigureAwait(false);

        public async Task<T?> ReadAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            if (await _localStorageProvider.ReadAsync(key, cancellationToken).ConfigureAwait(false) is string value)
                return _localStorageSerializer.Deserialize<T>(value);

            return default;
        }

        public async Task<string?> ReadAsync(string key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            return await _localStorageProvider.ReadAsync(key, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var removeEventAgrs = await RaiseStorageChangingEventArgsAsync(
                key,
                default,
                StorageAction.Removing,
                cancellationToken).ConfigureAwait(false);

            if (removeEventAgrs.IsCanceled)
                return;

            var oldValue = await _localStorageProvider.ReadAsync(key, cancellationToken);

            await _localStorageProvider.RemoveAsync(key, cancellationToken).ConfigureAwait(false);

            RaiseStorageChangedEventArgs(key, oldValue, default, StorageAction.Removed);
        }

        public async Task WriteAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var writeEventAgrs = await RaiseSerializedStorageChangingEventArgsAsync(
                key,
                value,
                StorageAction.Writing,
                cancellationToken).ConfigureAwait(false);

            if (writeEventAgrs.IsCanceled)
                return;

            var json = _localStorageSerializer.Serialize(value)
                ?? throw new ArgumentNullException($"The serialized value is null : {value}");

            var oldValue = await _localStorageProvider.ReadAsync(key, cancellationToken).ConfigureAwait(false) is string foundValue
                ? _localStorageSerializer.Deserialize<T>(foundValue)
                : default;

            await _localStorageProvider.WriteAsync(key, json, cancellationToken).ConfigureAwait(false);

            RaiseStorageChangedEventArgs(key, oldValue, value, StorageAction.Written);
        }

        public async Task WriteAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var writeEventAgrs = await RaiseStorageChangingEventArgsAsync(key,
                value,
                StorageAction.Writing,
                cancellationToken).ConfigureAwait(false);

            if (writeEventAgrs.IsCanceled)
                return;

            var oldValue = await _localStorageProvider.ReadAsync(key, cancellationToken).ConfigureAwait(false);

            await _localStorageProvider.WriteAsync(key, value, cancellationToken).ConfigureAwait(false);

            RaiseStorageChangedEventArgs(key, oldValue, value, StorageAction.Written);
        }
    }
}
