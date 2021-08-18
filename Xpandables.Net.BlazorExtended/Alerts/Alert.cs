
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

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// Contains a <see cref="Alert"/> to be display.
    /// </summary>
    public sealed class Alert : NotifyPropertyChanged<Alert>
    {
        /// <summary>
        /// Returns a new instance of <see cref="Alert"/> class.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public static Alert With(string title, string message) => new(IAlertDispatcher.DefaultId, title, message);

        internal static Alert None(string id) => new(id, "NONE", "NONE");

        internal Alert(string id, string title, string message)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the unique identifier of the alert.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the alert title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the alert header.
        /// </summary>
        public string? Header { get; private set; }

        /// <summary>
        /// Gets the message content of th alert.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the helper message.
        /// </summary>
        public string? Helper { get; private set; }

        /// <summary>
        /// Gets the created date, used for ordering alert.
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// Gets the alert level. The default value is <see cref="AlertLevel.Success"/>.
        /// </summary>
        public IAlertLevel Level { get; private set; } = AlertLevel.Success;

        /// <summary>
        /// Gets the alert icon. The default value os <see cref="AlertIcon.Success"/>.
        /// </summary>
        public IAlertIcon Icon { get; private set; } = AlertIcon.Success;

        /// <summary>
        /// Gets the fade css class.
        /// </summary>
        [NotifyPropertyChangedFor(nameof(IsFade))]
        public string FadeClass => IsFade ? "fade show" : string.Empty;

        private bool _isAutoClose = true;
        /// <summary>
        /// Determines whether the alert auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool IsAutoClose { get => _isAutoClose; private set => SetProperty(ref _isAutoClose, value); }

        private bool _isKeepAfterRouteChange = false;
        /// <summary>
        /// Determines whether the alert keep after route change.
        /// The default value is false.
        /// </summary>
        public bool IsKeepAfterRouteChange { get => _isKeepAfterRouteChange; private set => SetProperty(ref _isKeepAfterRouteChange, value); }

        private bool _isFade = false;
        /// <summary>
        /// Determines whether the alert fade out or not.
        /// The default value is false.
        /// </summary>
        public bool IsFade { get => _isFade; private set => SetProperty(ref _isFade, value); }

        /// <summary>
        /// Activate the Fade out of the alert.
        /// </summary>
        /// <param name="isFade">The fade state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Alert Fade(bool isFade = true) => this.With(x => x.IsFade = isFade);

        /// <summary>
        /// Activate the AutoClose of the alert.
        /// </summary>
        /// <param name="isAutoClose">The autoclose state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Alert AutoClose(bool isAutoClose = true) => this.With(x => x.IsAutoClose = isAutoClose);

        /// <summary>
        /// Activate the KeepAfterRouteChange of the alert.
        /// </summary>
        /// <param name="isKeepAfterRoutechange">The keepAfterRouteChange state, default value is true.</param>
        /// <returns>The current instance.</returns>
        public Alert KeepAfterRouteChange(bool isKeepAfterRoutechange = true) => this.With(x => x.IsKeepAfterRouteChange = isKeepAfterRoutechange);

        /// <summary>
        /// Activate the header message of the alert.
        /// </summary>
        /// <param name="header">The header message.</param>
        /// <returns>The current instance.</returns>
        public Alert WithHeader(string header) => this.With(x => x.Header = header);

        /// <summary>
        /// Activate the helper message of the alert.
        /// </summary>
        /// <param name="helper">The helper message.</param>
        /// <returns>The current instance.</returns>
        public Alert WithHelper(string helper) => this.With(x => x.Helper = helper);

        /// <summary>
        /// Activate the level of the alert.
        /// </summary>
        /// <param name="level">The level alert.</param>
        /// <returns>The current instance.</returns>
        public Alert WithLevel(IAlertLevel level) => this.With(x => x.Level = level);

        /// <summary>
        /// Activate the icon of the alert.
        /// </summary>
        /// <param name="icon">The icon alert.</param>
        /// <returns>The current instance.</returns>
        public Alert WithIcon(IAlertIcon icon) => this.With(x => x.Icon = icon);
    }
}
