
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
using System.Collections.ObjectModel;

using Xpandables.Net.Alerts;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Provides the base authentication interface for client displaying alerts.
    /// </summary>
    public interface IAlertProvider
    {
        /// <summary>
        /// Defines the default alert identifier.
        /// </summary>
        public const string DefaultId = "default-alert";

        /// <summary>
        /// Defines the event raised when receive an alert.
        /// </summary>
        event Action<Alert> OnAlert;

        /// <summary>
        /// Gets the collection of registered alerts.
        /// </summary>
        ObservableCollection<Alert> Alerts { get; }

        /// <summary>
        /// Raised a <see cref="AlertLevel.Success"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Error"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Info"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Warning"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised the specified notification.
        /// </summary>
        /// <param name="notification">The notification to be raised.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        void Notify(Alert notification);

        /// <summary>
        /// Clears the notification matching the specified identifier.
        /// </summary>
        /// <param name="id">The target notification identifier.</param>
        void Clear(string id = DefaultId);
    }
}
