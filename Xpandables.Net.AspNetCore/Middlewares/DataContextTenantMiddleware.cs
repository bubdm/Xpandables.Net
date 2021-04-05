
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

using System.Threading.Tasks;

using Xpandables.Net.Database;

namespace Xpandables.Net.Middlewares
{
    /// <summary>
    /// Defines the data context scope using the <see cref="DataContextTenantAttribute"/> if available.
    /// </summary>
    public sealed class DataContextTenantMiddleware : IMiddleware
    {
        /// <summary>
        /// Request handling method.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /> for the current request.</param>
        /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
        /// <returns>A <see cref="Task" /> that represents the execution of this middleware.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.GetEndpoint() is { } endpoint && endpoint.Metadata.GetMetadata<DataContextTenantAttribute>() is { } dataContextFactoryAttribute)
            {
                var dataContextTenantAccessor = context.RequestServices.GetRequiredService<IDataContextTenantAccessor>();
                dataContextTenantAccessor.SetTenantName(dataContextFactoryAttribute.TenantName);
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
