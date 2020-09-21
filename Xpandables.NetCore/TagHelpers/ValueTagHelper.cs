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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;

using Xpandables.Net.Localization;

namespace Xpandables.NetCore.TagHelpers
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
        private readonly ILocalizationResourceProvider? _localization;

        /// <summary>
        /// Initializes a new instance of <see cref="ValueTagHelper"/> with the localization view model source.
        /// </summary>
        /// <param name="localization">The localization view model source to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localization"/> is null.</exception>
        public ValueTagHelper(ILocalizationResourceProvider? localization) => _localization = localization;

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

            if (context.AllAttributes["type"]?.Value is string value && types.Contains(value))
            {
                var displayValue = For.ModelExplorer.Metadata.GetDisplayName();
                if (!string.IsNullOrWhiteSpace(displayValue))
                {
                    if (output.Attributes.TryGetAttribute("value", out var valueAttribute))
                        output.Attributes.Remove(valueAttribute);

                    output.Attributes.Add(new TagHelperAttribute("value", displayValue));
                }

                try
                {
                    var descriptionValue = For.ModelExplorer.Metadata.Description;
                    if (!string.IsNullOrWhiteSpace(descriptionValue))
                    {
                        if (output.Attributes.TryGetAttribute("title", out var titleAttribute))
                            output.Attributes.Remove(titleAttribute);

                        if (_localization is { })
                        {
                            var pageName = GetModelPageName(_localization, For.Metadata.ModelType);

                            if (_localization.ViewModelResourceTypeCollection.TryGetValue(pageName, out var resourceType))
                            {
                                var resourceManager = new ResourceManager(resourceType.FullName!, resourceType.Assembly);
                                descriptionValue = resourceManager.GetString($"Description{For.ModelExplorer.Metadata.Name}", CultureInfo.InvariantCulture) ?? descriptionValue;
                            }
                        }

                        output.Attributes.Add(new TagHelperAttribute("data-toggle", "tooltip"));
                        output.Attributes.Add(new TagHelperAttribute("data-placement", "top"));
                        output.Attributes.Add(new TagHelperAttribute("title", descriptionValue));
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private static string GetModelPageName(ILocalizationResourceProvider localizationResourceAccessor, Type modelType)
        => localizationResourceAccessor.IsSingleFileUsed && localizationResourceAccessor.ViewModelResourceTypes.Any()
            ? localizationResourceAccessor.ViewModelResourceTypeCollection.First().Key
            : modelType.Name.EndsWith("Model", StringComparison.InvariantCulture)
                ? $"{modelType.Name.Remove(modelType.Name.IndexOf("Model", StringComparison.InvariantCulture))}Localization"
                : $"{modelType.Name}Localization";
    }
}
