
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
using Microsoft.AspNetCore.Components.Web;

using Xpandables.AspNetCore.Semantic.Enumerations;
using Xpandables.Net5.AspNetCore.Semantic.Enumerations;
using Xpandables.Net5.AspNetCore.Semantic.Enumerations.Buttons;
using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.AspNetCore.Semantic.Elements
{
    /// <summary>
    /// A Button indicates a possible user action.
    /// </summary>
    public class Button : ComponentBase
    {
        /// <summary>
        /// A button can show it is currently the active user selection.
        /// </summary>
        [Parameter]
        public bool Active { get; set; }

        /// <summary>
        /// A button can animate to show hidden content.
        /// </summary>
        [Parameter]
        public Animation Animated { get; set; }

        /// <summary>
        /// A button can be attached to other content.
        /// </summary>
        [Parameter]
        public Position Attached { get; set; }

        /// <summary>
        /// A basic button is less pronounced.
        /// </summary>
        [Parameter]
        public bool Basic { get; set; }

        /// <summary>
        /// A button can be circular.
        /// </summary>
        [Parameter]
        public bool Circular { get; set; }

        /// <summary>
        /// A button can have different colors
        /// </summary>
        [Parameter]
        public Enumerations.Buttons.Color Color { get; set; }

        /// <summary>
        /// A button can reduce its padding to fit into tighter spaces.
        /// </summary>
        [Parameter]
        public bool Compact { get; set; }

        /// <summary>
        /// A button can show it is currently unable to be interacted with.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// A button can be aligned to the left or right of its container.
        /// </summary>
        [Parameter]
        public HorizontalPosition Floated { get; set; }

        /// <summary>
        /// A button can take the width of its container.
        /// </summary>
        [Parameter]
        public bool Fluid { get; set; }

        /// <summary>
        /// Add an Icon.
        /// </summary>
        [Parameter]
        public bool Icon { get; set; }

        /// <summary>
        /// A button can be formatted to appear on dark backgrounds.
        /// </summary>
        [Parameter]
        public bool Inverted { get; set; }

        /// <summary>
        /// Add a Label.
        /// </summary>
        [Parameter]
        public Label? Label { get; set; }

        /// <summary>
        /// A labeled button can format a Label or Icon to appear on the left or right.
        /// </summary>
        [Parameter]
        public HorizontalPosition LabelPosition { get; set; }

        /// <summary>
        /// A button can show a loading indicator.
        /// </summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// A button can hint towards a negative consequence.
        /// </summary>
        [Parameter]
        public bool Negative { get; set; }

        /// <summary>
        /// Called after user's click.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// A button can hint towards a positive consequence.
        /// </summary>
        [Parameter]
        public bool Positive { get; set; }

        /// <summary>
        /// A button can be formatted to show different levels of emphasis.
        /// </summary>
        [Parameter]
        public bool Primary { get; set; }

        /// <summary>
        /// The role of the HTML element.
        /// </summary>
        [Parameter]
        public string? Role { get; set; }

        /// <summary>
        /// A button can be formatted to show different levels of emphasis.
        /// </summary>
        [Parameter]
        public bool Secondary { get; set; }

        /// <summary>
        /// A button can have different sizes.
        /// </summary>
        [Parameter]
        public Size Size { get; set; }

        /// <summary>
        /// A button can receive focus.
        /// </summary>
        [Parameter]
        public int? TabIndex { get; set; }

        /// <summary>
        /// A button can be formatted to toggle on and off.
        /// </summary>
        [Parameter]
        public bool Toggle { get; set; }

        /// <summary>
        /// Groups can have their widths divided evenly.
        /// </summary>
        [Parameter]
        public Width Widths { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementTag = "button";
            ElementClass = "ui";

            ConfigureAnimation();

            ConfigureSize();

            ConfigureAttached();

            ConfigureTabIndex();

            ConfigureRole();

            ConfigureFloated();

            ConfigureLabelPosition();

            ConfigureColor();

            ConfigureBasic();

            ConfigureCompact();

            ConfigureActive();

            ConfigureToggle();

            ConfigureCircular();

            ConfigureLoading();

            ConfigureEmphasis();

            ConfigureIcon();

            ConfigurePositive();

            ConfigureNegative();

            ConfigureFluid();

            ConfigureDisabled();

            ConfigureInverted();

            ElementClass = $"{ElementClass} button";

            if (!ElementAttributes.ContainsKey("onclick"))
            {
                ElementAttributes.Add("onclick", OnClick);
            }
        }

        private void ConfigureInverted()
        {
            if (Inverted)
            {
                ElementClass = $"{ElementClass} inverted";
            }
        }

        private void ConfigureDisabled()
        {
            if (Disabled)
            {
                ElementClass = $"{ElementClass} disabled";
            }
        }

        private void ConfigureFluid()
        {
            if (Fluid)
            {
                ElementClass = $"{ElementClass} fluid";
            }
        }

        private void ConfigureNegative()
        {
            if (Negative)
            {
                ElementClass = $"{ElementClass} negative";
            }
        }

        private void ConfigurePositive()
        {
            if (Positive)
            {
                ElementClass = $"{ElementClass} positive";
            }
        }

        private void ConfigureIcon()
        {
            if (Icon)
            {
                ElementClass = $"{ElementClass} icon";
            }
        }

        private void ConfigureEmphasis()
        {
            if (Primary)
            {
                ElementClass = $"{ElementClass} primary";
            }
            else if (Secondary)
            {
                ElementClass = $"{ElementClass} secondary";
            }
        }

        private void ConfigureLoading()
        {
            if (Loading)
            {
                ElementClass = $"{ElementClass} loading";
            }
        }

        private void ConfigureCircular()
        {
            if (Circular)
            {
                ElementClass = $"{ElementClass} circular";
            }
        }

        private void ConfigureToggle()
        {
            if (Toggle)
            {
                ElementClass = $"{ElementClass} toggle";
                if (!ElementAttributes.ContainsKey("aria-pressed"))
                {
#pragma warning disable CA1308 // Normalize strings to uppercase
                    ElementAttributes.Add("aria-pressed", $"{Toggle}".ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
                }
            }
            else
            {
                if (ElementAttributes.ContainsKey("aria-pressed"))
                {
#pragma warning disable CA1308 // Normalize strings to uppercase
                    ElementAttributes["aria-pressed"] = Toggle.ToString().ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
                }
            }
        }

        private void ConfigureActive()
        {
            if (Active)
            {
                ElementClass = $"{ElementClass} active";
            }
        }

        private void ConfigureCompact()
        {
            if (Compact)
            {
                ElementClass = $"{ElementClass} compact";
            }
        }

        private void ConfigureBasic()
        {
            if (Basic)
            {
                ElementClass = $"{ElementClass} basic";
            }
        }

        private void ConfigureColor()
        {
            if (Color != Enumerations.Buttons.Color.None)
            {
                ElementClass = $"{ElementClass} {Color.GetDescription()}";
            }
        }

        private void ConfigureLabelPosition()
        {
            if (LabelPosition != HorizontalPosition.None)
            {
                ElementClass = $"{ElementClass} {LabelPosition.GetDescription()} labeled";
            }
        }

        private void ConfigureFloated()
        {
            if (Floated != HorizontalPosition.None)
            {
                ElementClass = $"{ElementClass} {Floated.GetDescription()} floated";
            }
        }

        private void ConfigureRole()
        {
            if (!string.IsNullOrWhiteSpace(Role))
            {
                if (!ElementAttributes.ContainsKey("role"))
                {
                    ElementAttributes.Add("role", Role);
                }
            }
        }

        private void ConfigureAttached()
        {
            if (Attached != Position.None)
            {
                ElementTag = "div";
                Role = "button";
                ElementClass = $"{ElementClass} {Attached.GetDescription()} attached";
                TabIndex = 0;
            }
        }

        private void ConfigureSize()
        {
            if (Size != Size.None)
            {
                ElementClass = $"{ElementClass} {Size.GetDescription()}";
            }
        }

        private void ConfigureAnimation()
        {
            if (Animated != Animation.None)
            {
                if (Animated != Animation.Horizontal)
                {
#pragma warning disable CA1308 // Normalize strings to uppercase
                    ElementClass = $"{ElementClass} {Animated.ToString().ToLowerInvariant()}";
#pragma warning restore CA1308 // Normalize strings to uppercase
                }

                ElementClass = $"{ElementClass} animated";
            }
        }

        private void ConfigureTabIndex()
        {
            const string tabIndexKey = "tabindex";

            if (ElementTag == "button") return;

            if (!ElementAttributes.ContainsKey(tabIndexKey))
            {
                ElementAttributes.Add(tabIndexKey, 0);
            }

            if (TabIndex.HasValue)
            {
                ElementAttributes[tabIndexKey] = TabIndex.Value;
                return;
            }

            if (Disabled)
            {
                ElementAttributes[tabIndexKey] = -1;
                return;
            }

            if (ElementTag == "div")
            {
                ElementAttributes[tabIndexKey] = 0;
            }
        }
    }
}
