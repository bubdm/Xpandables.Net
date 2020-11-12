
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
    /// Buttons can be grouped.
    /// </summary>
    public class ButtonGroup : ComponentBase
    {
        /// <summary>
        /// Groups can be attached to other content.
        /// </summary>
        [Parameter]
        public Position Attached { get; set; }

        /// <summary>
        /// Groups can be less pronounced.
        /// </summary>
        [Parameter]
        public bool Basic { get; set; }

        /// <summary>
        /// Groups can have a shared color.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// Groups can reduce their padding to fit into tighter spaces.
        /// </summary>
        [Parameter]
        public bool Compact { get; set; }

        /// <summary>
        /// Groups can be aligned to the left or right of its container.
        /// </summary>
        [Parameter]
        public HorizontalPosition Floated { get; set; }

        /// <summary>
        /// Groups can take the width of their container.
        /// </summary>
        [Parameter]
        public bool Fluid { get; set; }

        /// <summary>
        /// Groups can be formatted as icons.
        /// </summary>
        [Parameter]
        public bool Icon { get; set; }

        /// <summary>
        /// Groups can be formatted to appear on dark backgrounds.
        /// </summary>
        [Parameter]
        public bool Inverted { get; set; }

        /// <summary>
        /// Groups can be formatted as labeled icon buttons.
        /// </summary>
        [Parameter]
        public bool Labeled { get; set; }

        /// <summary>
        /// Groups can hint towards a negative consequence.
        /// </summary>
        [Parameter]
        public bool Negative { get; set; }

        /// <summary>
        /// Groups can hint towards a positive consequence.
        /// </summary>
        [Parameter]
        public bool Positive { get; set; }

        /// <summary>
        /// Groups can be formatted to show different levels of emphasis.
        /// </summary>
        [Parameter]
        public bool Primary { get; set; }

        /// <summary>
        /// Groups can be formatted to show different levels of emphasis.
        /// </summary>
        [Parameter]
        public bool Secondary { get; set; }

        /// <summary>
        /// Groups can have different sizes.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <summary>
        /// Groups can be formatted to toggle on and off.
        /// </summary>
        [Parameter]
        public bool Toggle { get; set; }

        /// <summary>
        /// Groups can be formatted to appear vertically.
        /// </summary>
        [Parameter]
        public bool Vertical { get; set; }

        /// <summary>
        /// Groups can have their widths divided evenly.
        /// </summary>
        [Parameter]
        public Width Widths { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "ui";

            if (Attached != Position.None)
            {
                ElementClass = $"{ElementClass} {Attached.GetDescription()} attached";
            }

            ConfigureSize();

            ConfigureIcon();

            ConfigureLabeled();

            ConfigureFloated();

            ConfigureColor();

            ConfigureBasic();

            ConfigureVertical();

            ConfigureWidth();

            ElementClass = $"{ElementClass} buttons";
        }

        private void ConfigureSize()
        {
            if (Size != Size.None)
            {
                ElementClass = $"{ElementClass} {Size.GetDescription()}";
            }
        }

        private void ConfigureIcon()
        {
            if (Icon)
            {
                ElementClass = $"{ElementClass} icon";
            }
        }

        private void ConfigureLabeled()
        {
            if (Labeled)
            {
                ElementClass = $"{ElementClass} labeled";
            }
        }

        private void ConfigureFloated()
        {
            if (Floated != HorizontalPosition.None)
            {
                ElementClass = $"{ElementClass} {Floated.GetDescription()} floated";
            }
        }

        private void ConfigureBasic()
        {
            if (Basic)
            {
                ElementClass = $"{ElementClass} basic";
            }
        }

        private void ConfigureVertical()
        {
            if (Vertical)
            {
                ElementClass = $"{ElementClass} vertical";
            }
        }

        private void ConfigureColor()
        {
            if (Color != Color.None)
            {
                ElementClass = $"{ElementClass} {Color.GetDescription()}";
            }
        }

        private void ConfigureWidth()
        {
            if (Widths != Width.None)
            {
                ElementClass = $"{ElementClass} {Widths.GetDescription()}";
            }
        }
    }
}
