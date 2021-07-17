
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
    public sealed class Alert
    {
        /// <summary>
        /// Gets the unique identifier of the alert.
        /// </summary>
        public string Id { get; internal set; } = default!;

        /// <summary>
        /// Gets the alert title.
        /// </summary>
        public string Title { get; init; } = default!;

        /// <summary>
        /// Gets the alert header.
        /// </summary>
        public string Header { get; init; } = default!;

        /// <summary>
        /// Gets the created date, used for ordering alert.
        /// </summary>
        public DateTime CreatedOn { get; init; }

        /// <summary>
        /// Gets the alert level.
        /// </summary>
        public IAlertLevel Level { get; init; } = AlertLevel.Success;

        /// <summary>
        /// Gets the alert icon.
        /// </summary>
        public IAlertIcon Icon { get; init; } = AlertIcon.Success;

        /// <summary>
        /// Gets the fade css class.
        /// </summary>
        public string FadeClass => Fade ? "fade show" : string.Empty;

        /// <summary>
        /// Gets the message content of th alert.
        /// </summary>
        public string Message { get; init; } = default!;

        /// <summary>
        /// Determines whether the alert auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool AutoClose { get; init; } = true;

        /// <summary>
        /// Determines whether the alert keep after route change.
        /// The default value is false.
        /// </summary>
        public bool KeepAfterRouteChange { get; internal set; }

        /// <summary>
        /// Determines whether the alert fade out or not.
        /// The default value is false.
        /// </summary>
        public bool Fade { get; internal set; } = false;

        /// <summary>
        /// Activate the Fade out of the alert.
        /// </summary>
        public void FadeOut() => Fade = true;
    }
}
