
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

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Xpandables.Net.TagHelpers
{
    /// <summary>
    /// A helper that uses the <see cref="RequiredAttribute"/> to add required tag.
    /// </summary>
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class RequiredTagHelper : TagHelper
    {
        /// <summary>
        /// When a set of <see cref="ITagHelper" />s are executed, their <see cref="TagHelper.Init(TagHelperContext)" />'s
        /// are first invoked in the specified <see cref="TagHelper.Order" />; then their
        /// <see cref="TagHelper.ProcessAsync(TagHelperContext,TagHelperOutput)" />'s are invoked in the specified
        /// <see cref="TagHelper.Order" />. Lower values are executed first.
        /// </summary>
        /// <remarks>Default order is <c>0</c>.</remarks>
        public override int Order => int.MaxValue;

        /// <summary>
        /// Gets or sets the asp-for expression.
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = default!;

        /// <summary>
        /// Synchronously executes the <see cref="TagHelper" /> with the given <paramref name="context" /> and
        /// <paramref name="output" />.
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag.</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = output ?? throw new ArgumentNullException(nameof(output));

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
}
