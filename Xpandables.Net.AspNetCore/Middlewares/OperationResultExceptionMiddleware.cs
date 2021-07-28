
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

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Xpandables.Net.Middlewares
{
    /// <summary>
    /// Defines the operation result exception handler middleware.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class OperationResultExceptionMiddleware : IMiddleware
    {
        private readonly bool _bypassResponseHasStarted = true;

        ///<inheritdoc/>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (OperationResultException exception) when (!context.Response.HasStarted && _bypassResponseHasStarted)
            {
                await OnOperationResultExceptionAsync(context, next, exception).ConfigureAwait(false);
            }
            catch (Exception exception) when (!context.Response.HasStarted && _bypassResponseHasStarted)
            {
                await OnExceptionAsync(context, exception).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static OperationResultExceptionController GetExceptionController(HttpContext context)
        {
            var controller = context.RequestServices.GetRequiredService<OperationResultExceptionController>();
            controller.ControllerContext = new ControllerContext(
                new ActionContext(
                    context,
                    context.GetRouteData(),
                    new ControllerActionDescriptor()));

            return controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected async Task OnOperationResultExceptionAsync(HttpContext context, RequestDelegate next, OperationResultException exception)
        {
            var operationResult = exception.Result;

            if (operationResult.IsFailed)
            {
                var controller = GetExceptionController(context);
                var validationProblemDtails = GetValidationProblemDetails(controller, context, operationResult);

                if (operationResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (await GetAuthenticateSchemeAsync(context).ConfigureAwait(false) is { } scheme)
                    {
                        context.Response.Headers.Append(HeaderNames.WWWAuthenticate, scheme);
                    }
                }

                await validationProblemDtails.ExecuteResultAsync(controller.ControllerContext).ConfigureAwait(false);
            }
            else
            {
                await next(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected async Task OnExceptionAsync(HttpContext context, Exception exception)
        {
            var controller = GetExceptionController(context);
            var problemDtails = GetProblemDetails(controller, context, exception);

            if (exception is UnauthorizedAccessException)
            {
                if (await GetAuthenticateSchemeAsync(context).ConfigureAwait(false) is { } scheme)
                {
                    context.Response.Headers.Append(HeaderNames.WWWAuthenticate, scheme);
                }
            }

            await problemDtails.ExecuteResultAsync(controller.ControllerContext).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="context"></param>
        /// <param name="operationResult"></param>
        /// <returns></returns>
        protected ActionResult GetValidationProblemDetails(OperationResultExceptionController controller, HttpContext context, IOperationResult operationResult)
        {
            var statusCode = operationResult.StatusCode;

            var validationProblemDetails = controller.ValidationProblem(
                GetValidationProblemDetailMessage(statusCode),
                context.Request.Path,
                (int)statusCode,
                GetProblemTitle(statusCode),
                modelStateDictionary: operationResult.Errors.GetModelStateDictionary());

            return validationProblemDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected ActionResult GetProblemDetails(OperationResultExceptionController controller, HttpContext context, Exception exception)
        {
            var statusCode = exception is UnauthorizedAccessException ? HttpStatusCode.Unauthorized : HttpStatusCode.InternalServerError;

            var problemDetails = controller.Problem(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? $"{exception}" : GetValidationProblemDetailMessage(statusCode),
                context.Request.Path,
                (int)statusCode,
                GetProblemTitle(statusCode));

            return problemDetails;
        }

        /// <summary>
        /// Returns the title value for the <see cref="ValidationProblemDetails"/>/<see cref="ProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The status code to act with.</param>
        /// <returns>A string value.</returns>
        protected virtual string GetProblemTitle(HttpStatusCode statusCode)
             => statusCode switch
             {
                 HttpStatusCode.NotFound => "Request Not Found",
                 HttpStatusCode.Locked => "Request Locked",
                 HttpStatusCode.Conflict => "Request Conflict",
                 HttpStatusCode.Unauthorized => "Unauthorized Access",
                 HttpStatusCode.InternalServerError => "Internal Server Error",
                 _ => "Request Validation Errors"
             };

        /// <summary>
        /// Returns the message detail value for the <see cref="ValidationProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The status code to act with.</param>
        /// <returns>A string value.</returns>
        protected virtual string GetValidationProblemDetailMessage(HttpStatusCode statusCode)
             => statusCode switch
             {
                 HttpStatusCode.InternalServerError or HttpStatusCode.Unauthorized => "Please refer to the errors for additional details",
                 _ => "Please refer to the errors property for additional details"
             };

        /// <summary>
        /// Returns the authentication scheme name.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>if found, the authentication scheme name or null.</returns>
        protected virtual async Task<string?> GetAuthenticateSchemeAsync(HttpContext context)
        {
            if (context.RequestServices.GetService<IAuthenticationSchemeProvider>() is { } authenticationSchemeProvider)
            {
                var requestSchemes = await authenticationSchemeProvider.GetRequestHandlerSchemesAsync().ConfigureAwait(false);
                var defaultSchemes = await authenticationSchemeProvider.GetDefaultAuthenticateSchemeAsync().ConfigureAwait(false);
                var allSchemes = await authenticationSchemeProvider.GetAllSchemesAsync().ConfigureAwait(false);

                var scheme = requestSchemes.FirstOrDefault() ?? defaultSchemes ?? allSchemes.FirstOrDefault();
                if (scheme is not null)
                    return scheme.Name;
            }

            return default;
        }
    }
}
