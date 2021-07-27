
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
    /// Contains a <see cref="Notification"/> to be display.
    /// </summary>
    public sealed class Notification : NotifyPropertyChanged<Notification>
    {
        /// <summary>
        /// Returns a new instance of <see cref="Notification"/> class.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public static Notification With(string title, string message) => new(INotificationDispatcher.DefaultId, title, message);

        internal static Notification None(string id) => new(id, "NONE", "NONE");

        internal Notification(string id, string title, string message)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the unique identifier of the notification.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the notification title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the notification header.
        /// </summary>
        public string? Header { get; private set; }

        /// <summary>
        /// Gets the message content of th notification.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the helper message.
        /// </summary>
        public string? Helper { get; private set; }

        /// <summary>
        /// Gets the created date, used for ordering notification.
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// Gets the notification level. The default value is <see cref="NotificationLevel.Success"/>.
        /// </summary>
        public INotificationLevel Level { get; private set; } = NotificationLevel.Success;

        /// <summary>
        /// Gets the notification icon. The default value os <see cref="NotificationIcon.Success"/>.
        /// </summary>
        public INotificationIcon Icon { get; private set; } = NotificationIcon.Success;

        /// <summary>
        /// Gets the fade css class.
        /// </summary>
        [NotifyPropertyChangedFor(nameof(IsFade))]
        public string FadeClass => IsFade ? "fade show" : string.Empty;

        private bool _isAutoClose = true;
        /// <summary>
        /// Determines whether the notification auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool IsAutoClose { get => _isAutoClose; private set => SetProperty(ref _isAutoClose, value); }

        private bool _isKeepAfterRouteChange = false;
        /// <summary>
        /// Determines whether the notification keep after route change.
        /// The default value is false.
        /// </summary>
        public bool IsKeepAfterRouteChange { get => _isKeepAfterRouteChange; private set => SetProperty(ref _isKeepAfterRouteChange, value); }

        private bool _isFade = false;
        /// <summary>
        /// Determines whether the notification fade out or not.
        /// The default value is false.
        /// </summary>
        public bool IsFade { get => _isFade; private set => SetProperty(ref _isFade, value); }

        /// <summary>
        /// Activate the Fade out of the notification.
        /// </summary>
        /// <param name="isFade">The fade state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Notification Fade(bool isFade = true) => this.With(x => x.IsFade = isFade);

        /// <summary>
        /// Activate the AutoClose of the notification.
        /// </summary>
        /// <param name="isAutoClose">The autoclose state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Notification AutoClose(bool isAutoClose = true) => this.With(x => x.IsAutoClose = isAutoClose);

        /// <summary>
        /// Activate the KeepAfterRouteChange of the notification.
        /// </summary>
        /// <param name="isKeepAfterRoutechange">The keepAfterRouteChange state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Notification KeepAfterRouteChange(bool isKeepAfterRoutechange = true) => this.With(x => x.IsKeepAfterRouteChange = isKeepAfterRoutechange);

        /// <summary>
        /// Activate the header message of the notification.
        /// </summary>
        /// <param name="header">The header message.</param>
        /// <returns>The current instance.</returns>
        public Notification WithHeader(string header) => this.With(x => x.Header = header);

        /// <summary>
        /// Activate the helper message of the notification.
        /// </summary>
        /// <param name="helper">The helper message.</param>
        /// <returns>The current instance.</returns>
        public Notification WithHelper(string helper) => this.With(x => x.Helper = helper);

        /// <summary>
        /// Activate the level of the notification.
        /// </summary>
        /// <param name="level">The level notification.</param>
        /// <returns>The current instance.</returns>
        public Notification WithLevel(INotificationLevel level) => this.With(x => x.Level = level);

        /// <summary>
        /// Activate the icon of the notification.
        /// </summary>
        /// <param name="icon">The icon notification.</param>
        /// <returns>The current instance.</returns>
        public Notification WithIcon(INotificationIcon icon) => this.With(x => x.Icon = icon);
    }
}
