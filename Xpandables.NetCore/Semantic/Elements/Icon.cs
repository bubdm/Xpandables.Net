
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
using Xpandables.Net.Semantic.Enumerations.Icons;

namespace Xpandables.Net.Semantic.Elements
{
    /// <summary>
    /// An icon is a glyph used to represent something else.
    /// </summary>
    public class Icon : ComponentBase
    {
        /// <summary>
        /// Icon can have an aria label.
        /// </summary>
        [Parameter]
        public string? AriaHidden { get; set; }

        /// <summary>
        /// Icon can have an aria label.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Formatted to appear bordered.
        /// </summary>
        [Parameter]
        public bool Bordered { get; set; }

        /// <summary>
        /// Icon can formatted to appear circular.
        /// </summary>
        [Parameter]
        public bool Circular { get; set; }

        /// <summary>
        /// Color of the icon.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// Icons can display a smaller corner icon.
        /// </summary>
        [Parameter]
        public Corner Corner { get; set; }

        /// <summary>
        /// Show that the icon is inactive.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Fitted, without space to left or right of Icon.
        /// </summary>
        [Parameter]
        public bool Fitted { get; set; }

        /// <summary>
        /// Icon can be flipped.
        /// </summary>
        [Parameter]
        public FlipDirection Flipped { get; set; }

        /// <summary>
        /// Formatted to have its colors inverted for contrast.
        /// </summary>
        [Parameter]
        public bool Inverted { get; set; }

        /// <summary>
        /// Icon can be formatted as a link.
        /// </summary>
        [Parameter]
        public bool Link { get; set; }

        /// <summary>
        /// Icon can be used as a simple loader.
        /// </summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// Name of the icon.
        /// </summary>
        [Parameter]
        public Enumerations.Elements.Icon Name { get; set; }

        /// <summary>
        /// Icon can rotated.
        /// </summary>
        [Parameter]
        public Rotation Rotated { get; set; }

        /// <summary>
        /// Size of the icon.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            string color = "";
            string options = "";
            ElementTag = "i";

            if (Disabled)
            {
                options = $"{options}disabled ";
            }

            if (Loading)
            {
                options = $"{options}loading ";
            }

            if (Fitted)
            {
                options = $"{options}fitted ";
            }

            if (Link)
            {
                options = $"{options}link ";
            }

            if (Flipped != FlipDirection.None)
            {
                options = $"{options}{Flipped.GetDescription()} flipped ";
            }

            if (Rotated != Rotation.None)
            {
                options = $"{options}{Rotated.GetDescription()} rotated ";
            }

            if (Corner != Corner.None)
            {
                options = Corner == Corner.Default ? $"{options} corner " : $"{options}{Corner.GetDescription()} corner ";
            }

            if (Circular)
            {
                options = $"{options}circular ";
            }

            if (Bordered)
            {
                options = $"{options}bordered ";
            }

            if (Inverted)
            {
                options = $"{options}inverted ";
            }

            if (Color != Color.None)
            {
                color = $"{Color.GetDescription()} ";
            }

            if (Size != Size.None && Size != Size.Medium)
            {
                options = $"{options}{Size.GetDescription()} ";
            }

            ElementClass = $"{color}{Name.GetDescription()} {options}icon";
        }
    }
}
