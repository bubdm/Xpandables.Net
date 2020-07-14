
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

using Xpandables.Net5.AspNetCore.Semantic.Enumerations;
using Xpandables.Net5.AspNetCore.Semantic.Enumerations.Labels;
using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.AspNetCore.Semantic.Elements
{
    /// <summary>
    /// A label displays content classification.
    /// </summary>
    public class Label : ComponentBase
    {
        /// <summary>
        /// A label can be active.
        /// </summary>
        [Parameter]
        public bool Active { get; set; }

        /// <summary>
        /// A label can attach to a content segment.
        /// </summary>
        [Parameter]
        public Side Attached { get; set; }

        /// <summary>
        /// A label can reduce its complexity.
        /// </summary>
        [Parameter]
        public bool Basic { get; set; }

        /// <summary>
        /// A label can be circular.
        /// </summary>
        [Parameter]
        public bool Circular { get; set; }

        /// <summary>
        /// Color of the label.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// A label can position itself in the corner of an element.
        /// </summary>
        [Parameter]
        public Corner Corner { get; set; }

        /// <summary>
        /// Formats the label as a dot.
        /// </summary>
        [Parameter]
        public bool Empty { get; set; }

        /// <summary>
        /// Float above another element in the upper right corner.
        /// </summary>
        [Parameter]
        public bool Floating { get; set; }

        /// <summary>
        /// A horizontal label is formatted to label content along-side it horizontally.
        /// </summary>
        [Parameter]
        public bool Horizontal { get; set; }

        /// <summary>
        /// A label can be formatted to emphasize an image.
        /// </summary>
        [Parameter]
        public bool Image { get; set; }

        /// <summary>
        /// A label can point to content next to it.
        /// </summary>
        [Parameter]
        public Direction Pointing { get; set; }

        /// <summary>
        /// A label can prompt for an error in your forms.
        /// </summary>
        [Parameter]
        public bool Prompt { get; set; }

        /// <summary>
        /// A label can appear as a ribbon attaching itself to an element.
        /// </summary>
        [Parameter]
        public Ribbon Ribbon { get; set; }

        /// <summary>
        /// A label can have different sizes.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <summary>
        /// A label can appear as a tag.
        /// </summary>
        [Parameter]
        public bool Tag { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "ui";

            ConfigureColor();

            ConfigureImage();

            ConfigureCorner();

            ConfigurePointing();

            ConfigureAttached();

            ConfigurePrompt();

            ConfigureTag();

            ConfigureRibbon();

            ConfigureSize();

            ConfigureHorizontal();

            ConfigureFloating();

            ConfigureCircular();

            ConfigureBasic();

            ElementClass = $"{ElementClass} label";
        }

        private void ConfigureBasic()
        {
            if (Basic)
            {
                ElementClass = $"{ElementClass} basic";
            }
        }

        private void ConfigureCircular()
        {
            if (Circular)
            {
                ElementClass = $"{ElementClass} circular";
            }
        }

        private void ConfigureFloating()
        {
            if (Floating)
            {
                ElementClass = $"{ElementClass} floating";
            }
        }

        private void ConfigureHorizontal()
        {
            if (Horizontal)
            {
                ElementClass = $"{ElementClass} horizontal";
            }
        }

        private void ConfigureTag()
        {
            if (Tag)
            {
                ElementClass = $"{ElementClass} tag";
            }
        }

        private void ConfigurePrompt()
        {
            if (Prompt)
            {
                ElementClass = $"{ElementClass} prompt";
            }
        }

        private void ConfigureImage()
        {
            if (Image)
            {
                ElementClass = $"{ElementClass} image";
            }
        }

        private void ConfigureSize()
        {
            if (Size != Size.None)
            {
                ElementClass = $"{ElementClass} {Size.GetDescription()}";
            }
        }

        private void ConfigureColor()
        {
            if (Color != Color.None)
            {
                ElementClass = $"{ElementClass} {Color.GetDescription()}";
            }
        }

        private void ConfigureCorner()
        {
            if (Corner != Corner.None)
            {
                ElementClass = $"{ElementClass} {Corner.GetDescription()} corner";
            }
        }

        private void ConfigureAttached()
        {
            if (Attached != Side.None)
            {
                ElementClass = $"{ElementClass} {Attached.GetDescription()} attached";
            }
        }

        private void ConfigureRibbon()
        {
            if (Ribbon == Ribbon.None) return;
            ElementClass = Ribbon == Ribbon.Left
                ? $"{ElementClass} ribbon"
                : $"{ElementClass} {Ribbon.GetDescription()} ribbon";
        }

        private void ConfigurePointing()
        {
            switch (Pointing)
            {
                case Direction.None:
                    return;
                case Direction.Left:
                case Direction.Right:
                    ElementClass = $"{ElementClass} {Pointing.GetDescription()} pointing";
                    break;
                default:
                    ElementClass = $"{ElementClass} pointing {Pointing.GetDescription()}";
                    break;
            }
        }
    }
}
