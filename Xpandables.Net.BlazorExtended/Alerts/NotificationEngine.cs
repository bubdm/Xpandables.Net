
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net.Alerts;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// The default implementation of <see cref="IAlertProvider"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class NotificationEngine : IAlertProvider
    {
        ///<inheritdoc/>
        public event Action<Alert> OnAlert;

        ///<inheritdoc/>
        public List<Alert> Notifications { get; } = new List<Alert>();

        ///<inheritdoc/>
        public virtual void Clear(string id = "default-notification") => OnAlert(new Alert { Id = id });

        ///<inheritdoc/>
        public virtual void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
                title,
                header,
                message,
                AlertLevel.Error,
                NotificationIcon.Error,
                autoClose,
                keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
                title,
                header,
                message,
                AlertLevel.Info,
                NotificationIcon.Info,
                autoClose,
                keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Notify(Alert notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            notification.Id ??= INotificationService.DefaultId;
            OnAlert?.Invoke(notification);
        }

        ///<inheritdoc/>
        public virtual void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
            title,
            header,
            message,
            AlertLevel.Success,
            NotificationIcon.Success,
            autoClose,
            keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
              title,
              header,
              message,
              AlertLevel.Warning,
              NotificationIcon.Warning,
              autoClose,
              keepAfterRouteChange);

            Notify(notification);
        }

        /// <summary>
        /// Creates a notification with arguments.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="level">The notification level.</param>
        /// <param name="icon">The notification icon.</param>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        /// <returns>A new instance of <see cref="Alert"/>.</returns>
        protected static Alert CreateNotification(
            string title,
            string header,
            string message,
            AlertLevel level,
            NotificationIcon icon,
            bool autoClose = true,
            bool keepAfterRouteChange = false)
        {
            var notification = new Alert
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Header = header,
                Level = level,
                Icon = icon,
                Message = message,
                AutoClose = autoClose,
                KeepAfterRouteChange = keepAfterRouteChange
            };

            return notification;

        }
    }
}
