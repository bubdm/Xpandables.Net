
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
namespace System.Design.Localization
{
    /// <summary>
    /// Using with <see langword="ServiceLocalizationProvider"/>, gives access to properties resources types
    /// to be used for localization. The resource type is identified by its string type name and behave as the data annotations
    /// attributes localization.
    /// </summary>
    public interface ILocalizationResourceAccessor
    {
        /// <summary>
        /// Contains the resource type for all data annotation validation attributes localization using the attribute name as a key.
        /// <para>For example :</para>
        /// The <see langword="RequiredAttribute.ErrorMessageResourceName"/> will be bounded to the <see langword="RequiredAttribute"/>
        /// as key in the resource file.
        /// </summary>
        Type? ValidationType { get; }

        /// <summary>
        /// Contains a collection of resource types to add localization for application view models.
        /// Each view model is associated with a resource type name that matches the <see langword="{ViewModelName}Localization"/>.
        /// </summary>
        ICorrelationCollection<string, Type> ViewModelResourceTypes { get; }
    }
}
