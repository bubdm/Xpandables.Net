
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

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// Defines the <see cref="IAlertProvider"/> options.
    /// </summary>
    public sealed class AlertOptions
    {
        /// <summary>
        /// Gets or sets the alert delay. The default value is 5000sec.
        /// </summary>
        public int Delay { get; set; } = 5000;

        /// <summary>
        /// Determines whether Fade is out or not.
        /// The default value is true.
        /// </summary>
        public bool FadeOut { get; set; } = true;

        /// <summary>
        /// Gets or sets the alert delay after fade out. The default value is 250sec.
        /// </summary>
        public int FadeOutDelay { get; set; } = 250;

        /// <summary>
        /// Gets the position of alert.
        /// </summary>
        public IAlertPosition Position { get; set; } = AlertPosition.TopRight;
    }
}
