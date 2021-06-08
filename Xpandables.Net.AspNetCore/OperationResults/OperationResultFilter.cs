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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Xpandables.Net
{
    /// <summary>
    /// When used as a filter, it'll automatically convert bad operation result to MVC Core equivalent.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class OperationResultFilter : IAsyncAlwaysRunResultFilter
    {
        /// <summary>
        /// Returns the title value for the <see cref="ValidationProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The status code to act with.</param>
        /// <returns>A string value.</returns>
        protected virtual string GetValidationProblemTitle(HttpStatusCode statusCode)
             => statusCode switch
             {
                 HttpStatusCode.NotFound => "Request Not Found",
                 HttpStatusCode.Locked => "Request Locked",
                 HttpStatusCode.Conflict => "Request Conflict",
                 HttpStatusCode.Unauthorized => "Unauthorized Access",
                 _ => "Request Validation Errors"
             };

        /// <summary>
        /// Returns the detail value for the <see cref="ValidationProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The status code to act with.</param>
        /// <returns>A string value.</returns>
        protected virtual string GetValidationProblemDetail(HttpStatusCode statusCode)
             => statusCode switch
             {
                 HttpStatusCode.InternalServerError => "Please refer to the errors for additional details",
                 _ => "Please refer to the errors property for additional details"
             };

        ///<inheritdoc/>
        public virtual async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is IOperationResult operationResult && operationResult.Failed)
            {
                var controller = (ControllerBase)context.Controller;
                if (controller is null)
                {
                    controller = context.HttpContext.RequestServices.GetRequiredService<ExceptionController>();
                    controller.ControllerContext = new ControllerContext(
                        new ActionContext(
                            context.HttpContext,
                            context.HttpContext.GetRouteData(),
                            new ControllerActionDescriptor()));
                }

                var statusCode = operationResult.StatusCode;
                context.Result = controller.ValidationProblem(
                    GetValidationProblemDetail(statusCode),
                    context.HttpContext.Request.Path,
                    (int)statusCode,
                    GetValidationProblemTitle(statusCode),
                    modelStateDictionary: operationResult.Errors.GetModelStateDictionary());

                if (operationResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (context.HttpContext.RequestServices.GetService<IAuthenticationSchemeProvider>() is { } authenticationSchemeProvider)
                    {
                        var schemes = await authenticationSchemeProvider.GetRequestHandlerSchemesAsync().ConfigureAwait(false);

                        if (schemes.FirstOrDefault() is { } scheme)
                            context.HttpContext.Response.Headers.Append(HeaderNames.WWWAuthenticate, scheme.Name);
                    }
                }
            }

            await next().ConfigureAwait(false);
        }
    }
}
