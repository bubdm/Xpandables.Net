
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
    /// Button groups can contain conditionals.
    /// </summary>
    public class ButtonOr : ComponentBase
    {
        /// <summary>
        /// Or buttons can have their text localized, or adjusted by using the text prop.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        private const string DataTextKey = "data-text";

        /// <inheritdoc />
        protected override void ConfigureComponent()
        {
            ElementClass = "or";

            if (string.IsNullOrEmpty(Text)) return;

            if (!ElementAttributes.ContainsKey(DataTextKey))
            {
                ElementAttributes.Add(DataTextKey, Text);
            }
            ElementAttributes[DataTextKey] = Text;
        }
    }
}
