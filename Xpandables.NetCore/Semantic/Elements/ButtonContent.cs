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
using Microsoft.AspNetCore.Components;

namespace Xpandables.NetCore.Semantic.Elements
{
    /// <summary>
    /// Used in some Button types, such as `animated`.
    /// </summary>
    public class ButtonContent : ComponentBase
    {
        /// <summary>
        /// Initially visible, hidden on hover.
        /// </summary>
        [Parameter]
        public bool Visible { get; set; }

        /// <summary>
        /// Initially hidden, visible on hover.
        /// </summary>
        [Parameter]
        public bool Hidden { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            if (Visible)
            {
                ElementClass = "visible content";
            }
            else if (Hidden)
            {
                ElementClass = "hidden content";
            }
        }
    }
}
