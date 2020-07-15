﻿
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

namespace Xpandables.Net5.AspNetCore.Semantic.Enumerations
{
    /// <summary>
    /// Position
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("none")]
        None,
        /// <summary>
        /// Left position
        /// </summary>
        [Description("left")]
        Left,
        /// <summary>
        /// Right position
        /// </summary>
        [Description("right")]
        Right,
        /// <summary>
        /// Top Position
        /// </summary>
        [Description("top")]
        Top,
        /// <summary>
        /// Bottom position
        /// </summary>
        [Description("bottom")]
        Bottom,
    }
}