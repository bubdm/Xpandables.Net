
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

namespace Xpandables.Net.Storage
{
    /// <summary>
    /// Defines the event action applying to the storage.
    /// </summary>
    public enum StorageAction
    {
        /// <summary>
        /// The action is a writing event.
        /// </summary>
        Writing,

        /// <summary>
        /// The action is a written event.
        /// </summary>
        Written,

        /// <summary>
        /// The action is a removing event.
        /// </summary>
        Removing,

        /// <summary>
        /// The action is a removed event.
        /// </summary>
        Removed
    }

    /// <summary>
    /// Provides with events for storage management.
    /// </summary>
    public interface ILocalStorageEvent
    {
        /// <summary>
        /// The event that will be asynchronously raised before the changes get applied.
        /// The event will contain values being processed.
        /// By setting the <see cref="StorageChangingEventArgs.IsCanceled"/> to <see langword="true"/>, you can cancel the process.
        /// </summary>
        event EventHandler<StorageChangingEventArgs>? Changing;

        /// <summary>
        /// The event that will be asynchronously raised after the changes applied.
        /// The event will contain values processed.
        /// </summary>
        event EventHandler<StorageChangedEventArgs>? Changed;
    }
}
