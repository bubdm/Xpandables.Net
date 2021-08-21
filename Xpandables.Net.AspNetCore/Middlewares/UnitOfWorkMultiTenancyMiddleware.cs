
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
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.Middlewares;

/// <summary>
/// Defines the unit of work scope using the <see cref="UnitOfWorkMultiTenancyAttribute"/> found in the current endpoint.
/// You can derive from this class to customize its behaviors.
/// </summary>
public class UnitOfWorkMultiTenancyMiddleware : IMiddleware
{
    /// <summary>
    /// Request handling method.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext" /> for the current request.</param>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <returns>A <see cref="Task" /> that represents the execution of this middleware.</returns>
    public virtual async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.GetEndpoint() is { } endpoint && endpoint.Metadata.GetMetadata<UnitOfWorkMultiTenancyAttribute>() is { } unitOfWorkMultiTenancyAttribute)
        {
            var unitOfWorkMultitenancyAccessor = context.RequestServices.GetRequiredService<IUnitOfWorkMultiTenancyAccessor>();
            unitOfWorkMultitenancyAccessor.SetTenantName(unitOfWorkMultiTenancyAttribute.TenantName);
        }

        await next(context).ConfigureAwait(false);
    }
}
