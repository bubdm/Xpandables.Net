
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

namespace Xpandables.Net.Storage
{
    internal class InternalSessionStorage : ISessionStorage
    {
        private readonly IStorage _storage;

        public event EventHandler<StorageChangingEventArgs>? Changing;
        public event EventHandler<StorageChangedEventArgs>? Changed;

        public InternalSessionStorage(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public ValueTask ClearAllAsync(CancellationToken cancellationToken = default)
            => _storage.ClearAllAsync("sessionStorage.clear", cancellationToken);

        public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
            => _storage.ContainKeyAsync("sessionStorage.hasOwnProperty", key, cancellationToken);

        public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
            => _storage.CountAsync("sessionStorage.length", "eval", cancellationToken);

        public ValueTask<TValue?> ReadAsync<TValue>(string key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));
            return _storage.ReadAsync<TValue>("sessionStorage.getItem", key, cancellationToken); ;
        }

        public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var oldValue = await ReadAsync<object>(key, cancellationToken).ConfigureAwait(false);
            var eventArgs = new StorageChangingEventArgs(key, oldValue, null, StorageAction.Removing);

            Changing?.Invoke(this, eventArgs);
            if (eventArgs.IsCanceled)
                return;

            await _storage.RemoveAsync("sessionStorage.removeItem", key, cancellationToken).ConfigureAwait(false);

            OnStorageChangedEventArgs(key, oldValue, default, StorageAction.Removed);
        }

        public async ValueTask WriteAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var oldValue = await ReadAsync<TValue>(key, cancellationToken).ConfigureAwait(false);
            var eventArgs = new StorageChangingEventArgs(key, oldValue, value, StorageAction.Writing);

            Changing?.Invoke(this, eventArgs);
            if (eventArgs.IsCanceled)
                return;

            await _storage.WriteAsync("sessionStorage.setItem", key, value, cancellationToken).ConfigureAwait(false);

            OnStorageChangedEventArgs(key, oldValue, value, StorageAction.Written);
        }

        void OnStorageChangedEventArgs(string key, object? oldValue, object? newValue, StorageAction storageAction)
        {
            var eventArgs = new StorageChangedEventArgs(key, oldValue, newValue, storageAction);

            Changed?.Invoke(this, eventArgs);
        }
    }
}
