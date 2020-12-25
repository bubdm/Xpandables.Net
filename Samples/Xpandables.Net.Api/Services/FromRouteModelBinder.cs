
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
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using Newtonsoft.Json;

namespace Xpandables.Net.Api.Services
{
    public sealed class FromRouteModelBinderProvider : IModelBinderProvider
    {

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (context.Metadata.IsComplexType)
            {
                var metaData = (DefaultModelMetadata)context.Metadata;
                if (metaData.Attributes.Attributes.OfType<FromRouteAttribute>().FirstOrDefault() is not null)
                    return new BinderTypeModelBinder(typeof(FromRouteModelBinder));
            }

            return default;
        }
    }

    public sealed class FromRouteModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            _ = bindingContext ?? throw new ArgumentNullException(nameof(bindingContext));

            var headerKey = bindingContext.ModelMetadata.BinderModelName;
            var modelType = bindingContext.ModelMetadata.ModelType;

            if (headerKey is not null)
            {
                var headerValue = bindingContext.HttpContext.Request.RouteValues[headerKey] as string;
                if (headerValue is not null)
                {
                    bindingContext.Model = JsonConvert.DeserializeObject(headerValue, modelType)!;
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                }
            }
            else
            {
                var dictionary = new Dictionary<string, string?>();
                foreach (var property in modelType.GetProperties().Where(p => p.GetSetMethod()?.IsPublic == true))
                {
                    var headerValue = bindingContext.HttpContext.Request.RouteValues[property.Name] as string;
                    dictionary.Add(property.Name, headerValue);
                }

                var dictString = JsonConvert.SerializeObject(dictionary);
                bindingContext.Model = JsonConvert.DeserializeObject(dictString, modelType)!;
                bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            }

            return Task.CompletedTask;
        }
    }
}
