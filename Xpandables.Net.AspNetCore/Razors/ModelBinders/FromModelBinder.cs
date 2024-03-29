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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xpandables.Net.Razors.ModelBinders
{
    /// <summary>
    /// Abstract class that holds the <see cref="FromModelBinder"/> dictionary.
    /// </summary>
    public abstract class FromModelBinder
    {
        /// <summary>
        /// Creates an instance of <see cref="FromModelBinder"/>.
        /// </summary>
        protected FromModelBinder() { }

        /// <summary>
        /// Contains a dictionary with key as attribute and the request path values matching the specified name.
        /// </summary>
        protected static IDictionary<Attribute, Func<HttpContext, string, string?>> RequestAttributeModelReader => new Dictionary<Attribute, Func<HttpContext, string, string?>>
        {
            { new FromHeaderAttribute(), (context, name) => context.Request.Headers[name].FirstOrDefault() },
            { new FromRouteAttribute(), (context, name) => context.Request.RouteValues[name]?.ToString() },
            { new FromQueryAttribute(), (context, name) => context.Request.Query[CultureInfo.CurrentCulture.TextInfo.ToTitleCase( name)].FirstOrDefault() }
        };
    }

    /// <summary>
    /// Model binder used to bind models from the specified attributes : <see cref="FromHeaderAttribute"/>, <see cref="FromRouteAttribute"/> and <see cref="FromQueryAttribute"/>.
    /// </summary>
    /// <typeparam name="TAttribute">the type of the attribute.</typeparam>
    public sealed class FromModelBinder<TAttribute> : FromModelBinder, IModelBinder
         where TAttribute : Attribute, new()
    {
        ///<inheritdoc/>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            _ = bindingContext ?? throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelMetadata.BinderModelName;
            var modelType = bindingContext.ModelMetadata.ModelType;

            if (modelName is not null)
            {
                var attributeValue = RequestAttributeModelReader[new TAttribute()](bindingContext.HttpContext, modelName);
                if (attributeValue is not null)
                {
                    var model = JsonSerializer.Deserialize(attributeValue, modelType, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
            }
            else
            {
                var dictionary = new Dictionary<string, string?>();
                foreach (var property in modelType.GetProperties().Where(p => p.GetSetMethod()?.IsPublic == true))
                {
                    var attributeValue = RequestAttributeModelReader[new TAttribute()](bindingContext.HttpContext, property.Name);
                    dictionary.Add(property.Name, attributeValue);
                }

                var dictString = JsonSerializer.Serialize(dictionary);
                var model = JsonSerializer.Deserialize(dictString, modelType, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                bindingContext.Result = ModelBindingResult.Success(model);
            }

            return Task.CompletedTask;
        }
    }
}
