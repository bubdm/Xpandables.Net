
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

namespace Xpandables.Net.BlazorExtended.Storage
{
    /// <summary>
    /// Represents storage information before changes apply.
    /// This event allows cancellation of the process.
    /// </summary>
    public sealed class StorageChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new event for storage changing.
        /// </summary>
        /// <param name="key">The target key in the storage.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="storageAction">The action being processed.</param>
        internal StorageChangingEventArgs(string key, object? oldValue, object? newValue, StorageAction storageAction)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            OldValue = oldValue;
            NewValue = newValue;
            StorageAction = storageAction;
        }

        /// <summary>
        /// Gets the storage key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the storage old value before change.
        /// </summary>
        public object? OldValue { get; }

        /// <summary>
        /// Gets the storage new value after change.
        /// </summary>
        public object? NewValue { get; }

        /// <summary>
        /// Gets the action being processed.
        /// </summary>
        public StorageAction StorageAction { get; }

        /// <summary>
        /// Determines whether or not the change should be canceled.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
