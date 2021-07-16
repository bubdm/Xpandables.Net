
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
    public readonly struct Alert
    {
        /// <summary>
        /// Gets the unique identifier of the alert.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Gets the alert title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets the alert header.
        /// </summary>
        public string Header { get; init; }

        /// <summary>
        /// Gets the created date, used for ordering alert.
        /// </summary>
        public DateTime CreatedOn { get; init; }

        /// <summary>
        /// Gets the alert level.
        /// </summary>
        public AlertLevel Level { get; init; }

        /// <summary>
        /// Gets the alert icon.
        /// </summary>
        public NotificationIcon Icon { get; init; }

        /// <summary>
        /// Gets the message content of th alert.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Determines whether the alert auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool AutoClose { get; init; }

        /// <summary>
        /// Determines whether the alert keep after route change.
        /// The default value is false.
        /// </summary>
        public bool KeepAfterRouteChange { get; init; }

        /// <summary>
        /// Determines whether the alert fade out or not.
        /// The default value is false.
        /// </summary>
        public bool Fade { get; init; }

        /// <summary>
        /// Activate the Fade out of the alert.
        /// </summary>
        public Alert FadeOut() => this with { Fade = true };
    }
}
