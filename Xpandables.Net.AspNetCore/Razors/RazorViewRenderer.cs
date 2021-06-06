
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
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net.Razors
{
    // Based upon code from: https://github.com/aspnet/Entropy/blob/93ee2cf54eb700c4bf8ad3251f627c8f1a07fb17/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs

    /// <summary>
    /// Implementation of <see cref="IRazorViewRenderer"/>.
    /// </summary>
    public sealed class RazorViewRenderer : IRazorViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRazorModelViewCollection _modelViews;

        /// <inheritdoc />
        public RazorViewRenderer(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceScopeFactory serviceScopeFactory, IRazorModelViewCollection modelViews)
        {
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            _tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _modelViews = modelViews ?? throw new ArgumentNullException(nameof(modelViews));
        }

        ///<inheritdoc/>
        public async Task<string> RenderFromModelToStringAsync<TModel>(TModel model)
        {
            foreach (var (modelType, identifier) in _modelViews)
            {
                if (modelType == typeof(TModel))
                    return await RenderViewToStringAsync(identifier, model).ConfigureAwait(false);
            }

            throw new InvalidOperationException($"Unable to find view from model {typeof(TModel).Name}");
        }

        ///<inheritdoc/>
        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var actionContext = GetActionContext(scope.ServiceProvider);
            var view = FindView(actionContext, viewName);

            using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext).ConfigureAwait(false);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
                return getViewResult.View;

            var findVirwResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findVirwResult.Success)
                return findVirwResult.View;

            var searchedLocations = getViewResult.SearchedLocations.Concat(findVirwResult.SearchedLocations);
            var errorMessage = string.Join(
                           Environment.NewLine,
                           new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        private static ActionContext GetActionContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
