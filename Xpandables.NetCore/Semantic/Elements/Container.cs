
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

using Xpandables.Net.Semantic.Enumerations.Containers;

namespace Xpandables.Net.Semantic.Elements
{
    /// <summary>
    /// A container limits content to a maximum width.
    /// </summary>
    public class Container : ComponentBase
    {
        /// <summary>
        /// Container has no maximum width.
        /// </summary>
        [Parameter]
        public bool Fluid { get; set; }

        /// <summary>
        /// Reduce maximum width to more naturally accommodate text.
        /// </summary>
        [Parameter]
        public bool Text { get; set; }

        /// <summary>
        /// Align container text.
        /// </summary>
        [Parameter]
        public TextAlign TextAlign { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "ui";

            if (Fluid)
            {
                ElementClass = $"{ElementClass} fluid";
            }

            if (Text)
            {
                ElementClass = $"{ElementClass} text";
            }

            if (TextAlign != TextAlign.None)
            {
                ElementClass = $"{ElementClass} {TextAlign.GetDescription()} aligned";
            }

            ElementClass = $"{ElementClass} container";
        }
    }
}
