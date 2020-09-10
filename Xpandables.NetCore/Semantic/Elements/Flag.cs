
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

using Xpandables.Net.Types;

namespace Xpandables.NetCore.Semantic.Elements
{
    /// <summary>
    /// A flag is used to represent a political state.
    /// </summary>
    public class Flag : ComponentBase
    {
        /// <summary>
        /// Flag name, can use the two digit country code, the full name, or a common alias.
        /// </summary>
        [Parameter]
        public Enumerations.Elements.Flag Name { get; set; }

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementTag = "i";
            ElementClass = $"{Name.GetDescription()} flag";
        }
    }
}
