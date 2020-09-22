
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
using System.Data;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;

using Xpandables.NetCore.HttpRestClient;

namespace Xpandables.Net.Api.Middlewares
{
    public sealed class UserExceptionHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception exception) when (!context.Response.HasStarted)
            {
                IActionResult actionResult = exception switch
                {
                    ValidationException validationException => new BadRequestObjectResult(
                        validationException.GetHttpRestClientValidation()),
                    ArgumentException argumentException => new BadRequestObjectResult(argumentException.Message),
                    OperationCanceledException operationCanceledException => new BadRequestObjectResult(operationCanceledException.Message),
                    DuplicateNameException duplicateNameException => new BadRequestObjectResult(duplicateNameException.Message),
                    SecurityTokenException securityTokenException => new BadRequestObjectResult(securityTokenException.Message),
                    UnauthorizedAccessException unauthorizedAccessException => new UnauthorizedObjectResult(unauthorizedAccessException.Message),
                    KeyNotFoundException keyNotFoundException => new NotFoundObjectResult(keyNotFoundException.Message),
                    InvalidOperationException invalidOperationException =>
                        invalidOperationException.InnerException is { }
                            ? new BadRequestObjectResult(invalidOperationException.InnerException.ToString())
                            : new BadRequestObjectResult(invalidOperationException.Message),
                    _ => new BadRequestObjectResult("Internal Server Error.Please try later.")
                    { StatusCode = StatusCodes.Status500InternalServerError }
                };

                await actionResult.ExecuteResultAsync(
                   new ActionContext(context, context.GetRouteData(), new ActionDescriptor()))
                   .ConfigureAwait(false);
            }
        }
    }
}
