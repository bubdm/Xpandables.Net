using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

using Xpandables.NetCore.Helpers;

namespace Xpandables.Samples.Api.Middlewares
{
    public sealed class XpandablesExceptionHandlerMiddleware : IMiddleware
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
                            ? new BadRequestObjectResult(invalidOperationException.InnerException.Message)
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
