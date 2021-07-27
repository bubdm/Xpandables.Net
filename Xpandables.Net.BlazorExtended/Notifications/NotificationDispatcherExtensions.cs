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

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Provides with extension methods for <see cref="INotificationDispatcher"/>.
    /// </summary>
    public static class NotificationDispatcherExtensions
    {
        /// <summary>
        /// Raizes a success notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Success(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Success, NotificationIcon.Success, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes a warning notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Warning(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Warning, NotificationIcon.Warning, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes an error notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Error(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Error, NotificationIcon.Error, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes an info notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Info(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Info, NotificationIcon.Info, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes a dark notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Dark(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Dark, NotificationIcon.Dark, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes a light notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Light(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Light, NotificationIcon.Light, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes a primary notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Primary(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Primary, NotificationIcon.Primary, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

        /// <summary>
        /// Raizes a secondary notification with specified information.
        /// </summary>
        /// <param name="dispatcher">The taergt dispatcher.</param>
        /// <param name="title">the title of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="header">The header message of the notification.</param>
        /// <param name="helper">The helper message of the notification.</param>
        /// <param name="isAutoClose">is notification auto closed.</param>
        /// <param name="isKeepAfterRouteChange">is notification kept after route change.</param>
        /// <param name="isFade">is notification fade.</param>
        public static void Secondary(
            this INotificationDispatcher dispatcher,
            string title,
            string message,
            string? header = default,
            string? helper = default,
            bool isAutoClose = true,
            bool isKeepAfterRouteChange = false,
            bool isFade = true)
            => dispatcher.Notify(title, message, NotificationLevel.Secondary, NotificationIcon.Secondary, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);
    }
}
