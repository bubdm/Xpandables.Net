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

namespace Xpandables.Net.BlazorExtended.Storage
{
    /// <summary>
    /// Represents a base class to handle <see cref="ILocalStorageEngine"/> events.
    /// </summary>
    public abstract class LocalStorageEventHandler
    {
        /// <summary>
        /// When overridden, this method will handle the <see cref="ILocalStorageEvent.Changed"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public abstract void OnChanged(object? sender, StorageChangedEventArgs eventArgs);

        /// <summary>
        /// When overridden, this method will handle the <see cref="ILocalStorageEvent.Changing"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments to act on.</param>       
        public abstract void OnChanging(object? sender, StorageChangingEventArgs eventArgs);
    }
}
