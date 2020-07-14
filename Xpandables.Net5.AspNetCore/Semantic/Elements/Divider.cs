
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

namespace Xpandables.Net5.AspNetCore.Semantic.Elements
{
    /// <summary>
    /// A divider visually segments content into groups.
    /// </summary>
    public class Divider : ComponentBase
    {
        /// <summary>
        /// Divider can clear the content above it.
        /// </summary>
        [Parameter]
        public bool Clearing { get; set; }

        /// <summary>
        /// Divider can be fitted without any space above or below it.
        /// </summary>
        [Parameter]
        public bool Fitted { get; set; }

        /// <summary>
        /// Divider can divide content without creating a dividing line.
        /// </summary>
        [Parameter]
        public bool Hidden { get; set; }

        /// <summary>
        /// Divider can segment content horizontally.
        /// </summary>
        [Parameter]
        public bool Horizontal { get; set; }

        /// <summary>
        /// Divider can have its colors inverted.
        /// </summary>
        [Parameter]
        public bool Inverted { get; set; }

        /// <summary>
        /// Divider can provide greater margins to divide sections of content.
        /// </summary>
        [Parameter]
        public bool Section { get; set; }

        /// <summary>
        /// Divider can segment content vertically.
        /// </summary>
        [Parameter]
        public bool Vertical { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "ui";

            if (Vertical)
            {
                ElementClass = $"{ElementClass} vertical";
            }

            if (Horizontal)
            {
                ElementClass = $"{ElementClass} horizontal";
            }

            if (Fitted)
            {
                ElementClass = $"{ElementClass} fitted";
            }

            if (Hidden)
            {
                ElementClass = $"{ElementClass} hidden";
            }

            if (Section)
            {
                ElementClass = $"{ElementClass} section";
            }

            if (Clearing)
            {
                ElementClass = $"{ElementClass} clearing";
            }

            if (Inverted)
            {
                ElementClass = $"{ElementClass} inverted";
            }

            ElementClass = $"{ElementClass} divider";
        }
    }
}
