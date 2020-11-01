
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

using Xpandables.Net.Semantic.Enumerations;
using Xpandables.Net.Types;

namespace Xpandables.Net.Semantic.Elements
{
    /// <summary>
    /// Several icons can be used together as a group.
    /// </summary>
    public class IconGroup : ComponentBase
    {
        /// <summary>
        /// Size of the icon group.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementTag = "i";
            ElementClass = Size != Size.None ? $"{Size.GetDescription()} icons" : "icons";
        }
    }
}
