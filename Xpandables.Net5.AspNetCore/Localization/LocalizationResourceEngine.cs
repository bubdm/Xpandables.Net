
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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Xpandables.Net5.Helpers;
using Xpandables.Net5.Localization;

namespace Xpandables.Net5.AspNetCore.Localization
{
    /// <summary>
    /// Implementation of <see cref="ILocalizationResourceEngine"/> that provides with data annotations binder for localization.
    /// </summary>
    public sealed class LocalizationResourceEngine : ILocalizationResourceEngine
    {
        private readonly ILocalizationResourceProvider _localizationResourceProvider;

        /// <summary>
        /// Initializes the engine with the localization resource.
        /// </summary>
        /// <param name="localizationResourceProvider">The localization resource provider to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localizationResourceProvider"/> is null.</exception>
        public LocalizationResourceEngine(ILocalizationResourceProvider localizationResourceProvider)
            => _localizationResourceProvider = localizationResourceProvider
            ?? throw new ArgumentNullException(nameof(localizationResourceProvider));

        /// <summary>
        /// Creates validation from the context.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            BindDisplayAttribute((DefaultModelMetadata)context.ModelMetadata);
            BindDisplayFormatAttribute((DefaultModelMetadata)context.ModelMetadata);

            foreach (var validator in context.ValidatorMetadata.Cast<ValidationAttribute>())
                BindValidationAttribute(validator);

            if (_localizationResourceProvider is ILocalizationResourceProviderExtended resourceProviderExtended)
            {
                resourceProviderExtended.ModelBinderAttributeHandler
                    ?.Invoke((DefaultModelMetadata)context.ModelMetadata, context.ModelMetadata.ContainerMetadata);
                resourceProviderExtended.ModelValidatorAttributeHandler
                    ?.Invoke((DefaultModelMetadata)context.ModelMetadata, context.ValidatorMetadata.Cast<ValidationAttribute>());
            }
        }

        /// <summary>
        /// Updates the binder using the context.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var defaultModelMetadata = (DefaultModelMetadata)context.Metadata;
            if (defaultModelMetadata?.ContainerMetadata?.ModelType is null)
                return null;

            var pageName = GetModelPageName(defaultModelMetadata.ContainerMetadata.ModelType);
            var pageProperties = GetModelPageProperties(defaultModelMetadata);

            foreach (var property in pageProperties)
            {
                BindDisplayAttribute((DefaultModelMetadata)property, pageName);
                BindDisplayFormatAttribute((DefaultModelMetadata)property, pageName);

                foreach (var validator in property.ValidatorMetadata.Cast<ValidationAttribute>())
                    BindValidationAttribute(validator);

                if (_localizationResourceProvider is ILocalizationResourceProviderExtended resourceProviderExtended)
                {
                    resourceProviderExtended.ModelBinderAttributeHandler
                        ?.Invoke((DefaultModelMetadata)property, defaultModelMetadata.ContainerMetadata);
                    resourceProviderExtended.ModelValidatorAttributeHandler
                        ?.Invoke((DefaultModelMetadata)property, property.ValidatorMetadata.Cast<ValidationAttribute>());
                }
            }

            return null;
        }

        private string? GetModelPageName(Type modelType)
            => _localizationResourceProvider.IsSingleFileUsed && _localizationResourceProvider.ViewModelResourceTypes.Any()
                ? _localizationResourceProvider.ViewModelResourceTypeCollection.First().Key
                : modelType.Name.EndsWith("Model")
                    ? $"{modelType.Name.Remove(modelType.Name.IndexOf("Model"))}Localization"
                    : default;

        private static ModelPropertyCollection GetModelPageProperties(DefaultModelMetadata defaultModelMetadata)
            => defaultModelMetadata.ModelType == typeof(string) || defaultModelMetadata.ModelType.IsPrimitive
                ? new ModelPropertyCollection(defaultModelMetadata.SingleToEnumerable())
                : defaultModelMetadata.Properties;

        private void BindDisplayAttribute(DefaultModelMetadata modelMetadata, string? pageName = default)
        {
            var displayAttribute = modelMetadata
                .Attributes.PropertyAttributes?
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            if (displayAttribute is { }
                && _localizationResourceProvider
                .ViewModelResourceTypeCollection
                .TryGetValue(pageName ?? $"{modelMetadata.ContainerType.Name}Localization", out var resourceType))
            {
                if (displayAttribute.Name is { }) displayAttribute.Name = $"Display{modelMetadata.Name}";
                if (displayAttribute.Prompt is { }) displayAttribute.Prompt = $"Prompt{modelMetadata.Name}";
                if (displayAttribute.Description is { }) displayAttribute.Description = $"Description{modelMetadata.Name}";
                displayAttribute.ResourceType = resourceType;
            }
        }

        private void BindDisplayFormatAttribute(DefaultModelMetadata modelMetadata, string? pageName = default)
        {
            var displayFormat = modelMetadata
                .Attributes.PropertyAttributes?
                .OfType<LocalizedDisplayFormatAttribute>()
                .FirstOrDefault();

            if (displayFormat is { }
                && _localizationResourceProvider
                .ViewModelResourceTypeCollection
                .TryGetValue(pageName ?? $"{modelMetadata.ContainerType.Name}Localization", out var resourceType))
            {
                if (displayFormat.DataFormatString is { })
                {
                    displayFormat.DataFormatString = $"Format{modelMetadata.Name}";
                    displayFormat.DataFormatStringResourceType = resourceType;
                }

                if (displayFormat.NullDisplayText is { })
                {
                    displayFormat.NullDisplayText = $"NullDisplay{modelMetadata.Name}";
                    displayFormat.NullDisplayTextResourceType = resourceType;
                }
            }
        }

        private void BindValidationAttribute(ValidationAttribute validator)
        {
            if (_localizationResourceProvider.ValidationType is { })
            {
                validator.ErrorMessageResourceType = _localizationResourceProvider.ValidationType;
                validator.ErrorMessageResourceName = ((Type)validator.TypeId).Name;
            }
        }
    }
}
