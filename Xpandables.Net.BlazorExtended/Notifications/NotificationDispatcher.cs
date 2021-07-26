
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
    /// The default implementation of <see cref="INotificationDispatcher"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class NotificationDispatcher : INotificationDispatcher
    {
        ///<inheritdoc/>
        public event Action<Notification>? OnNotification;

        ///<inheritdoc/>
        public string ComponentId { get; set; } = default!;

        ///<inheritdoc/>
        public virtual void Clear(string id = INotificationDispatcher.DefaultId) => OnNotification?.Invoke(Notification.None(id));

        ///<inheritdoc/>
        public virtual void RaizeNotification(Notification notification)
        {
            if (notification.Id == INotificationDispatcher.DefaultId)
                notification.Id = ComponentId;

            OnNotification?.Invoke(notification);
        }
    }
}
