
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

using Xpandables.Net.Notifications;

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// Determines the alert level.
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// Success level.
        /// </summary>
        Success,

        /// <summary>
        /// Error level.
        /// </summary>
        Error,

        /// <summary>
        /// Information level.
        /// </summary>
        Info,

        /// <summary>
        /// Warning level.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Determines the alert position.
    /// </summary>
    public enum AlertPosition
    {
        /// <summary>
        /// Top Left.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Top right.
        /// </summary>
        TopRight,

        /// <summary>
        /// Top center.
        /// </summary>
        TopCenter,

        /// <summary>
        /// Bottom Left.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Bottom right.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Bottom center.
        /// </summary>
        BottomCenter
    }

    /// <summary>
    /// Determines the alert icon.
    /// </summary>
    public enum NotificationIcon
    {
        /// <summary>
        /// Success level.
        /// </summary>
        Success,

        /// <summary>
        /// Error level.
        /// </summary>
        Error,

        /// <summary>
        /// Information level.
        /// </summary>
        Info,

        /// <summary>
        /// Warning level.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Contains a <see cref="Alert"/> to be display.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Alert"/> class.
        /// </summary>
        public Alert() { }

        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        public string Id { get; internal set; } = IAlertProvider.DefaultId;

        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the notification header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the created date, used for ordering notification.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the notification level.
        /// </summary>
        public AlertLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the notification icon.
        /// </summary>
        public NotificationIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the message content of th notification.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Determines whether the notification auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// Determines whether the notification keep after route change.
        /// The default value is false.
        /// </summary>
        public bool KeepAfterRouteChange { get; set; } = false;

        /// <summary>
        /// Determines whether the notification fade out or not.
        /// The default value is false.
        /// </summary>
        public bool Fade { get; private set; } = false;

        /// <summary>
        /// Activate the Fade out of the notification.
        /// </summary>
        public void FadeOut() => Fade = true;
    }
}
