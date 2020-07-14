
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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Xpandables.Net5.Localization;

namespace Xpandables.Net5.AspNetCore.Localization
{
    /// <summary>
    ///  Defines a method signature to be used to handle custom model binder attributes for localization binding.
    /// </summary>
    /// <param name="modelMetadata">The current model meta-data being used.</param>
    /// <param name="containerMetadata">The current container model being used.</param>
#pragma warning disable ET001 // Type name does not match file name
    public delegate void ModelBinderAttributeHandler(DefaultModelMetadata modelMetadata, ModelMetadata containerMetadata);
#pragma warning restore ET001 // Type name does not match file name

    /// <summary>
    ///  Defines a method signature to be used to handle custom model validator attributes for localization binding.
    /// </summary>
    /// <param name="modelMetadata">The current model meta-data being used.</param>
    /// <param name="validationAttributes">The collection of validation attributes.</param>
#pragma warning disable ET001 // Type name does not match file name
    public delegate void ModelValidatorAttributeHandler(DefaultModelMetadata modelMetadata, IEnumerable<ValidationAttribute> validationAttributes);
#pragma warning restore ET001 // Type name does not match file name

    /// <summary>
    /// Gives access to properties resources types to be used for localization and provides with custom model binder and validator attributes.
    /// The resource type is identified by its string type name and behave as the data annotations attributes localization.
    /// </summary>
    public interface ILocalizationResourceProviderExtended : ILocalizationResourceProvider
    {
        /// <summary>
        /// Contains a custom model binder attributes.
        /// This handler get called when model is bound to attributes other than <see cref="ValidationAttribute"/>.
        /// </summary>
        ModelBinderAttributeHandler? ModelBinderAttributeHandler { get; }

        /// <summary>
        /// Contains a custom model validator attributes.
        /// This handler get called when model is bound to validation attributes.
        /// </summary>
        ModelValidatorAttributeHandler? ModelValidatorAttributeHandler { get; }
    }
}
