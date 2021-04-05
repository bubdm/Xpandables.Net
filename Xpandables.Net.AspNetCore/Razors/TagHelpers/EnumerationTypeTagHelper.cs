﻿
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Xpandables.Net.Razors.TagHelpers
{
    /// <summary>
    /// A helper that uses the <see cref="DisplayAttribute.Name"/> of an <see cref="EnumerationType.Name"/> with localization
    /// value for value tag.
    /// The behavior is available only for label
    /// </summary>
    [HtmlTargetElement("label", Attributes = aspEnumAttributeName)]
    public sealed class EnumerationTypeTagHelper : TagHelper
    {
        private const string aspEnumAttributeName = "asp-enum";
        private readonly IStringLocalizer _localization;

        /// <summary>
        /// Initializes a new instance of <see cref="EnumerationTypeTagHelper"/> with the localization view model source.
        /// </summary>
        /// <param name="localization">The localization view-model source to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localization"/> is null.</exception>
        public EnumerationTypeTagHelper(IStringLocalizer localization) => _localization = localization;

        /// <summary>
        /// When a set of <see cref="ITagHelper" />s are executed, their <see cref="M:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper.Init(Microsoft.AspNetCore.Razor.TagHelpers.TagHelperContext)" />'s
        /// are first invoked in the specified <see cref="P:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper.Order" />; then their
        /// <see cref="M:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper.ProcessAsync(Microsoft.AspNetCore.Razor.TagHelpers.TagHelperContext,Microsoft.AspNetCore.Razor.TagHelpers.TagHelperOutput)" />'s are invoked in the specified
        /// <see cref="P:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper.Order" />. Lower values are executed first.
        /// </summary>
        /// <remarks>Default order is <c>0</c>.</remarks>
        public override int Order => int.MaxValue;

        /// <summary>
        /// Gets or sets the asp-enum type expression
        /// </summary>
        [HtmlAttributeName(aspEnumAttributeName)]
        public EnumerationType? EnumType { get; set; }

        /// <summary>
        /// Gets or sets the view context.
        /// </summary>
        [ViewContext]
        public ViewContext ViewContext { get; set; } = default!;

        /// <summary>
        /// Synchronously executes the <see cref="TagHelper" /> with the given <paramref name="context" /> and
        /// <paramref name="output" />.
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag.</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output).ConfigureAwait(false);

            if (EnumType is null) throw new ArgumentException($"{nameof(EnumType)} is null");
            var enumTranslated = _localization[EnumType.Name];

            var childContentTag = await output.GetChildContentAsync().ConfigureAwait(false);
            childContentTag.Append(enumTranslated);

            output.Content.SetHtmlContent(childContentTag.GetContent());
        }
    }
}
