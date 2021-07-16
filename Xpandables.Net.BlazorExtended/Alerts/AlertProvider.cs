
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
using System.Collections.ObjectModel;

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// The default implementation of <see cref="IAlertProvider"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class AlertProvider : IAlertProvider
    {
        ///<inheritdoc/>
        public event Action<Alert>? OnAlert;

        ///<inheritdoc/>
        public ObservableCollection<Alert> Alerts { get; } = new();

        ///<inheritdoc/>
        public virtual void Clear(string id = IAlertProvider.DefaultId) => OnAlert?.Invoke(new Alert { Id = id });

        ///<inheritdoc/>
        public virtual void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var alert = CreateAlert(
                title,
                header,
                message,
                AlertLevel.Error,
                NotificationIcon.Error,
                autoClose,
                keepAfterRouteChange);

            RaizeAlert(alert);
        }

        ///<inheritdoc/>
        public virtual void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var alert = CreateAlert(
                title,
                header,
                message,
                AlertLevel.Info,
                NotificationIcon.Info,
                autoClose,
                keepAfterRouteChange);

            RaizeAlert(alert);
        }

        ///<inheritdoc/>
        public virtual void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var alert = CreateAlert(
            title,
            header,
            message,
            AlertLevel.Success,
            NotificationIcon.Success,
            autoClose,
            keepAfterRouteChange);

            RaizeAlert(alert);
        }

        ///<inheritdoc/>
        public virtual void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var alert = CreateAlert(
              title,
              header,
              message,
              AlertLevel.Warning,
              NotificationIcon.Warning,
              autoClose,
              keepAfterRouteChange);

            RaizeAlert(alert);
        }

        /// <summary>
        /// Creates a alert with arguments.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <param name="level">The alert level.</param>
        /// <param name="icon">The alert icon.</param>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        /// <returns>A new instance of <see cref="Alert"/>.</returns>
        protected static Alert CreateAlert(
            string title,
            string header,
            string message,
            AlertLevel level,
            NotificationIcon icon,
            bool autoClose = true,
            bool keepAfterRouteChange = false)
        {
            var alert = new Alert
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

            return alert;
        }

        ///<inheritdoc/>
        public virtual void RaizeAlert(Alert alert)
        {
            if (alert.Id is null)
                alert = alert with { Id = IAlertProvider.DefaultId };

            OnAlert?.Invoke(alert);
        }
    }
}
