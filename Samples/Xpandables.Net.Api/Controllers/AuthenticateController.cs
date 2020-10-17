
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
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Xpandables.Net.Api.Attributes;
using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Models;
using Xpandables.Net.Dispatchers;

namespace Xpandables.Net.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public AuthenticateController(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        [HttpPost, AllowAnonymous]
        [BasicAuthentication]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        public async Task<IActionResult> PostAsync(CancellationToken cancellationToken = default)
        {
            var requestAuthen = new GetAuthenToken(ControllerContext.HttpContext.Request.Headers["Phone"], ControllerContext.HttpContext.Request.Headers["Password"]);
            var response = await _dispatcher.InvokeAsync(requestAuthen, cancellationToken).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
