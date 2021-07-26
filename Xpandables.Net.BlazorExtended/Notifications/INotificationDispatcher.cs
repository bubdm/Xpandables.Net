
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

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Provides the base notification interface for client displaying notifications.
    /// </summary>
    public interface INotificationDispatcher
    {
        /// <summary>
        /// Defines the default notification identifier.
        /// </summary>
        public const string DefaultId = "default-notification";

        /// <summary>
        /// Gets or sets the current component idenfifier.
        /// </summary>
        string ComponentId { get; set; }

        /// <summary>
        /// Defines the event raised when receive an notification.
        /// </summary>
        event Action<Notification>? OnNotification;

        /// <summary>
        /// Raised the specified notification.
        /// </summary>
        /// <param name="notification">The notification to be raised.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        void RaizeNotification(Notification notification);

        /// <summary>
        /// Clears the notification matching the specified identifier.
        /// </summary>
        /// <param name="id">The target notification identifier.</param>
        void Clear(string id = DefaultId);
    }
}
