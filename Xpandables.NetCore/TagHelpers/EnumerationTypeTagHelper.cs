
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

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

using Xpandables.Net.Enumerations;
using Xpandables.Net.Localization;

namespace Xpandables.Net.TagHelpers
{
    /// <summary>
    /// A helper that uses the <see cref="DisplayAttribute.Name"/> of an <see cref="EnumerationType.DisplayName"/> with localization
    /// value for value tag. You must provide an implementation of <see cref="ILocalizationResourceProvider"/> for localization.
    /// The behavior is available only for label
    /// </summary>
    [HtmlTargetElement("label", Attributes = aspEnumAttributeName)]
    public sealed class EnumerationTypeTagHelper : TagHelper
    {
        private const string aspEnumAttributeName = "asp-enum";
        private readonly ILocalizationResourceProvider? _localization;

        /// <summary>
        /// Initializes a new instance of <see cref="EnumerationTypeTagHelper"/> with the localization view model source.
        /// </summary>
        /// <param name="localization">The localization view-model source to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localization"/> is null.</exception>
        public EnumerationTypeTagHelper(ILocalizationResourceProvider? localization) => _localization = localization;

        /// <summary>
        /// When a set of <see cref="ITagHelper" />s are executed, their <see cref="TagHelper.Init(TagHelperContext)" />'s
        /// are first invoked in the specified <see cref="TagHelper.Order" />; then their
        /// <see cref="TagHelper.ProcessAsync(TagHelperContext,TagHelperOutput)" />'s are invoked in the specified
        /// <see cref="TagHelper.Order" />. Lower values are executed first.
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
            _ = output ?? throw new ArgumentNullException(nameof(output));

            await base.ProcessAsync(context, output).ConfigureAwait(false);

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            if (EnumType is null) throw new ArgumentNullException(nameof(EnumType));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            var enumTranslated = EnumType.DisplayName;
            if (_localization is { })
            {
                var pageName = GetModelPageName(_localization, ViewContext.ViewData.Model.GetType());

                if (_localization.ViewModelResourceTypeCollection.TryGetValue(pageName, out var resourceType))
                {
                    var resourceManager = new ResourceManager(resourceType.FullName!, resourceType.Assembly);
                    enumTranslated = resourceManager.GetString($"Enum{EnumType.DisplayName}", CultureInfo.InvariantCulture) ?? enumTranslated;
                }
            }

            var childContentTag = await output.GetChildContentAsync().ConfigureAwait(false);
            childContentTag.Append(enumTranslated);

            output.Content.SetHtmlContent(childContentTag.GetContent());
        }

        private static string GetModelPageName(ILocalizationResourceProvider localizationResourceAccessor, Type modelType)
        => localizationResourceAccessor.IsSingleFileUsed && localizationResourceAccessor.ViewModelResourceTypes.Any()
            ? localizationResourceAccessor.ViewModelResourceTypeCollection.First().Key
            : modelType.Name.EndsWith("Model", StringComparison.InvariantCulture)
                ? $"{modelType.Name.Remove(modelType.Name.IndexOf("Model", StringComparison.InvariantCulture))}Localization"
                : $"{modelType.Name}Localization";
    }
}
