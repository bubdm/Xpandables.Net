
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

namespace Xpandables.Net.Alerts
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
        event Action<Alert>? OnAlert;

        /// <summary>
        /// Gets or sets the alert options.
        /// </summary>
        AlertOptions AlertOptions{ get; set; }

        /// <summary>
        /// Gets the collection of registered alerts.
        /// </summary>
        List<Alert> Alerts { get; }

        /// <summary>
        /// Raised a <see cref="AlertLevel.Success"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Error"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Info"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Warning"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Primary"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Primary(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Secondary"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Secondary(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Dark"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Dark(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="AlertLevel.Light"/> alert.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="header">The alert header.</param>
        /// <param name="message">The message of the alert.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the alert auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the alert keeps after route change.</param>
        void Light(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised the specified alert.
        /// </summary>
        /// <param name="alert">The alert to be raised.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="alert"/> is null.</exception>
        void RaizeAlert(Alert alert);

        /// <summary>
        /// Clears the alert matching the specified identifier.
        /// </summary>
        /// <param name="id">The target alert identifier.</param>
        void Clear(string id = DefaultId);
    }
}
