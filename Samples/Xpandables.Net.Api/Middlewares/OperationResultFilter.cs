
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Xpandables.Net.CQRS;

namespace Xpandables.Net.Api.Middlewares
{
    public sealed class OperationResultFilter : IAsyncAlwaysRunResultFilter
    {
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is not ObjectResult objectResult ||
                objectResult.Value is not IOperationResult operationResult) 
                return next();

            if (operationResult.IsFailure)
            {
                context.Result = operationResult.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => new NotFoundObjectResult(new ValidationProblemDetails(operationResult.Errors.ToDictionary())
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status404NotFound,
                        Title = "Request Not Found",
                        Detail = "Please refer to the errors for additional details"
                    }),
                    System.Net.HttpStatusCode.InternalServerError => new BadRequestObjectResult(new ValidationProblemDetails(operationResult.Errors.ToDictionary())
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Internal Server Error",
                        Detail = "Please refer to the errors for additional details"
                    }),
                    _ => new BadRequestObjectResult(new ValidationProblemDetails(operationResult.Errors.ToDictionary())
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Request Validation Errors",
                        Detail = "Please refer to the errors property for additional details"
                    })
                };
            }

            return next();
        }
    }
}
