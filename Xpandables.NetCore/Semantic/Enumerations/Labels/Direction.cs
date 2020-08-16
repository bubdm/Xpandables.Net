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

namespace Xpandables.NetCore.Semantic.Enumerations.Labels
{
    /// <summary>
    /// Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("none")]
        None,
        /// <summary>
        /// Above direction
        /// </summary>
        [Description("above")]
        Above,
        /// <summary>
        /// Below direction
        /// </summary>
        [Description("below")]
        Below,
        /// <summary>
        /// Left direction
        /// </summary>
        [Description("left")]
        Left,
        /// <summary>
        /// Right direction
        /// </summary>
        [Description("right")]
        Right
    }
}
