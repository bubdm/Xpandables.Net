
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace System.Design.Localization
{
    /// <summary>
    /// Gives access to properties resources types to be used for localization.
    /// The resource type is identified by its string type name and behave as the data annotations attributes localization.
    /// </summary>
    public interface ILocalizationResourceProvider
    {
        /// <summary>
        /// Contains the collection of resource types used to add localization for application viewmodels.
        /// Each viewmodel is associated with a resource type name that matches the <see langword="{ViewModelName}Localization"/>.
        /// This behavior is available for the following attributes :
        /// <para><see cref="DisplayAttribute"/> :</para>
        /// <see cref="DisplayAttribute.Name"/> will be bounded to the <see langword="Display{PropertyName}"/>
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Prompt"/> will be bounded to the <see langword="Prompt{PropertyName}"/>
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Description"/> will be bound to the <see langword="Description{PropertyName}"/>
        /// as key in the resource file.
        /// <para><see cref="LocalizedDisplayFormatAttribute"/> :</para>
        /// <see cref="LocalizedDisplayFormatAttribute.DataFormatString"/> will be bounded to the <see langword="Format{PropertyName}"/>
        /// as key in the resource.
        /// <see langword="LocalizedDisplayFormatAttribute.NullDisplayText"/> will be bounded to the
        /// <see langword="NullDisplay{PropertyName}"/> as key in the resource file.
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
        /// Returns a collection of available cultures found in the current application based on the <see cref="ViewModelResourceTypes"/> definition.
        /// </summary>
        /// <returns>A collection of <see cref="CultureInfo"/> of an empty collection if no result.</returns>
        IEnumerable<CultureInfo> AvailableViewModelCultures()
        {
            if ((ViewModelResourceTypes?.Count() ?? 0) <= 0) return Enumerable.Empty<CultureInfo>();

            var resourceManager = new ResourceManager(ViewModelResourceTypes!.First());
            var cultureInfos = new List<CultureInfo>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue;
                    if (resourceManager.GetResourceSet(culture, true, false) is { })
                        cultureInfos.Add(culture);
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
