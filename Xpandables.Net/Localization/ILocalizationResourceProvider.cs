
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;

using Xpandables.Net.Correlation;
using Xpandables.Net.Enumerations;

namespace Xpandables.Net.Localization
{
    /// <summary>
    /// Gives access to properties resources types to be used for localization.
    /// The resource type is identified by its string type name and behave as the data annotations attributes localization.
    /// </summary>
    public interface ILocalizationResourceProvider
    {
        /// <summary>
        /// Contains the collection of resource types used to add localization for application page models (razor style).
        /// Each page is associated with a resource type name that matches the <see langword="{PageName}Localization"/> following the framework name convention.
        /// if a localization file is not defined, the default data annotation behavior is used.
        /// <para></para>
        /// Example :
        /// pageName.cshtml.cs contains the pageNameModel and the localization file is named pageNameLocalization.resx.
        /// <para></para>
        /// This behavior is available for the following attributes :
        /// <para><see cref="DisplayAttribute"/> :</para>
        /// <see cref="DisplayAttribute.Name"/> (just set to any value) will be bounded to the <see langword="Display{PropertyName}"/>
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Prompt"/> (just set to any value) will be bounded to the <see langword="Prompt{PropertyName}"/>
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Description"/> (just set to any value) will be bound to the <see langword="Description{PropertyName}"/>
        /// as key in the resource file.
        /// <para><see cref="LocalizedDisplayFormatAttribute"/> :</para>
        /// <see cref="LocalizedDisplayFormatAttribute.DataFormatString"/> (just set to any value) will be bounded to the <see langword="Format{PropertyName}"/>
        /// as key in the resource file.
        /// <see langword="LocalizedDisplayFormatAttribute.NullDisplayText"/> (just set to any value) will be bounded to the
        /// <see langword="NullDisplay{PropertyName}"/> as key in the resource file.
        /// <para>
        /// You can use the <see cref="EnumerationType"/> class to build custom enumeration and use the tag helper <see langword="asp-enum"/> that expects the enumeration value for localization in label and the enumeration value will be bound to the <see langword="Enum{EnumerationValue}"/> as key in the resource file.
        /// </para>
        /// You can use the <see langword="ILocalizationResourceProviderExtended"/> for custom model binder and validator attributes for localization.
        /// You can use <see cref="IsSingleFileUsed"/> for single culture resource for all pages.
        /// </summary>
        IEnumerable<Type> ViewModelResourceTypes { get; }

        /// <summary>
        /// Contains the resource type for all data annotation validation attributes localization using the attribute name as a key.
        /// <para>For example :</para>
        /// The <see langword="RequiredAttribute.ErrorMessageResourceName"/> will be bounded to the <see langword="RequiredAttribute"/>
        /// as key in the resource file.
        /// </summary>
        Type? ValidationType { get; }

        /// <summary>
        /// Determines whether to use a single resource page for culture. If so, the <see cref="ViewModelResourceTypes"/> may contain 
        /// a unique type that will be used for all the page models. 
        /// Otherwise, you must provide a file for each page. The default behavior returns <see langword="false"/>.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if so, otherwise <see langword="false"/>.</returns>
        public bool IsSingleFileUsed => false;

        /// <summary>
        /// Contains a collection of resource types to add localization for application view models.
        /// Each view model is associated with a resource type name that matches the <see langword="{ViewModelName}Localization"/> or can contains
        /// only one localization file in case of singe file use.
        /// </summary>
        public ICorrelationCollection<string, Type> ViewModelResourceTypeCollection
        {
            get
            {
                ICorrelationCollection<string, Type> correlationCollection = new CorrelationCollection<string, Type>();
                if (!ViewModelResourceTypes.Any()) Trace.WriteLine($"{nameof(ViewModelResourceTypes)} is empty.");

                foreach (var type in ViewModelResourceTypes)
                    correlationCollection.AddOrUpdateValue(type.Name, type);

                return correlationCollection;
            }
        }

        /// <summary>
        /// Returns a collection of available cultures found in the current application based on the <see cref="ViewModelResourceTypes"/> definition.
        /// </summary>
        /// <returns>A collection of <see cref="CultureInfo"/> of an empty collection if no result.</returns>
        public sealed IEnumerable<CultureInfo> AvailableViewModelCultures()
        {
            if ((ViewModelResourceTypes?.Count() ?? 0) <= 0) return Enumerable.Empty<CultureInfo>();

            var resourceManager = new ResourceManager(ViewModelResourceTypes!.First());
            var cultureInfos = new List<CultureInfo>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                        continue;

                    if (resourceManager.GetResourceSet(culture, true, false) is { })
                    {
                        cultureInfos.Add(culture);
                        continue;
                    }
                }
                catch (MissingManifestResourceException exception)
                {
                    Trace.WriteLine($"Getting resource manager for '{culture.Name}' thrown exception : {exception}");
                }
            }

            return cultureInfos;
        }
    }
}
