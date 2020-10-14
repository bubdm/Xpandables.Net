
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
using System.Net.Http.Headers;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Xpandables.Net.Api.Attributes
{
    public sealed class BasicAuthenticationFilter : IAuthorizationFilter
    {
        private readonly string _realm;
        public BasicAuthenticationFilter(string realm) => _realm = realm ?? throw new ArgumentNullException(nameof(realm));

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);
                var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter!);
                var credentialStrings = Encoding.UTF8.GetString(credentialBytes).Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                context.HttpContext.Request.Headers["Phone"] = credentialStrings[0];
                context.HttpContext.Request.Headers["Password"] = credentialStrings[1];
            }
            catch
            {
                ReturnUnauthorizedResult(context);
            }
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic real=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
    }
}
