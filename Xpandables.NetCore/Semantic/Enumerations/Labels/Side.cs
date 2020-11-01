
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
using System.ComponentModel;

namespace Xpandables.Net.Semantic.Enumerations.Labels
{
    /// <summary>
    /// Side
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("none")]
        None,
        /// <summary>
        /// Top side
        /// </summary>
        [Description("top")]
        Top,
        /// <summary>
        /// Bottom side
        /// </summary>
        [Description("bottom")]
        Bottom,
        /// <summary>
        /// Top left side
        /// </summary>
        [Description("top left")]
        TopLeft,
        /// <summary>
        /// Top right side
        /// </summary>
        [Description("top right")]
        TopRight,
        /// <summary>
        /// Bottom left side
        /// </summary>
        [Description("bottom left")]
        BottomLeft,
        /// <summary>
        /// Bottom right side
        /// </summary>
        [Description("bottom right")]
        BottomRight
    }
}
