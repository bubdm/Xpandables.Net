
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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System.ComponentModel.DataAnnotations;

namespace Xpandables.Net.Razors.TagHelpers;

/// <summary>
/// A helper that uses the <see cref="RequiredAttribute"/> to add required tag.
/// </summary>
[HtmlTargetElement("input", Attributes = "asp-for")]
public class RequiredTagHelper : TagHelper
{
    ///<inheritdoc/>
    public override int Order
    {
        get { return int.MaxValue; }
    }

    /// <summary>
    /// Gets or sets the asp-for expression.
    /// </summary>
    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = default!;

    ///<inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        if (context.AllAttributes["required"] == null)
        {
            var isRequired = For.ModelExplorer.Metadata.ValidatorMetadata.Any(a => a is RequiredAttribute);
            if (isRequired)
            {
                var requiredAttribute = new TagHelperAttribute("required");
                output.Attributes.Add(requiredAttribute);
            }
        }
    }
}
