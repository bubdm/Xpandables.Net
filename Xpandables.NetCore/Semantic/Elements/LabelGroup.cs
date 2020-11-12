
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

namespace Xpandables.Net.Semantic.Elements
{
    /// <summary>
    /// A label can be grouped.
    /// </summary>
    public class LabelGroup : ComponentBase
    {
        /// <summary>
        /// Labels can share shapes.
        /// </summary>
        [Parameter]
        public bool Circular { get; set; }

        /// <summary>
        /// Label group can share colors together.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// Label group can share sizes together.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <summary>
        /// Label group can share tag formatting.
        /// </summary>
        [Parameter]
        public bool Tag { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "ui";

            if (Size != Size.None)
            {
                ElementClass = $"{ElementClass} {Size.GetDescription()}";
            }

            if (Color != Color.None)
            {
                ElementClass = $"{ElementClass} {Color.GetDescription()}";
            }

            if (Tag)
            {
                ElementClass = $"{ElementClass} tag";
            }

            if (Circular)
            {
                ElementClass = $"{ElementClass} circular";
            }

            ElementClass = $"{ElementClass} labels";
        }
    }
}
