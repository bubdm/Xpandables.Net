
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Xpandables.Net.Razors.TagHelpers
{
    /// <summary>
    /// A helper that uses the <see cref="DisplayAttribute.Name"/> as value tag and tool-tip tag for <see cref="DisplayAttribute.Description"/>.
    /// The behavior is available only for submit or button input type.
    /// </summary>
    [HtmlTargetElement("input", Attributes = aspForAttributeName)]
    public class ValueTagHelper : TagHelper
    {
        private static readonly IReadOnlyList<string> types = new[] { "submit", "button" };
        private const string aspForAttributeName = "asp-for";
        private readonly IStringLocalizer _localization;

        /// <summary>
        /// Initializes a new instance of <see cref="ValueTagHelper"/> with the localization view model source.
        /// </summary>
        /// <param name="localization">The localization view model source to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localization"/> is null.</exception>
        public ValueTagHelper(IStringLocalizer localization) => _localization = localization;

        ///<inheritdoc/>
        public override int Order => int.MaxValue;

        /// <summary>
        /// Gets or sets the asp-for expression.
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = default!;

        ///<inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (context.AllAttributes["type"]?.Value is string value && types.Contains(value))
            {
                var displayValue = For.ModelExplorer.Metadata.GetDisplayName();
                if (!string.IsNullOrWhiteSpace(displayValue))
                {
                    if (output.Attributes.TryGetAttribute("value", out var valueAttribute))
                        output.Attributes.Remove(valueAttribute);

                    output.Attributes.Add(new TagHelperAttribute("value", displayValue));
                }

                var descriptionValue = For.ModelExplorer.Metadata.Description;
                if (!string.IsNullOrWhiteSpace(descriptionValue))
                {
                    if (output.Attributes.TryGetAttribute("title", out var titleAttribute))
                        output.Attributes.Remove(titleAttribute);

                    descriptionValue = For.ModelExplorer.Metadata.Name is not null
                        ? _localization[For.ModelExplorer.Metadata.Name]
                        : descriptionValue;

                    output.Attributes.Add(new TagHelperAttribute("data-toggle", "tooltip"));
                    output.Attributes.Add(new TagHelperAttribute("data-placement", "top"));
                    output.Attributes.Add(new TagHelperAttribute("title", descriptionValue));
                }
            }
        }
    }
}
