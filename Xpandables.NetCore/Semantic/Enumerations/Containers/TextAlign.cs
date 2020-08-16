
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

namespace Xpandables.NetCore.Semantic.Enumerations.Containers
{
    /// <summary>
    /// Text align
    /// </summary>
    public enum TextAlign
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("none")]
        None,

        /// <summary>
        /// Text align left
        /// </summary>
        [Description("left")]
        Left,

        /// <summary>
        /// Text align right
        /// </summary>
        [Description("right")]
        Right,

        /// <summary>
        /// Text align center
        /// </summary>
        [Description("center")]
        Center,

        /// <summary>
        /// Text align justified
        /// </summary>
        [Description("justified")]
        Justified
    }
}
