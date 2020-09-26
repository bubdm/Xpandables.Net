
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Dispatchers;

namespace Xpandables.Net.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public UserController(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsync([FromBody] EditUser editUser, CancellationToken cancellationToken = default)
        {
            await _dispatcher.InvokeAsync(editUser, cancellationToken).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] GetUser getUser, CancellationToken cancellationToken = default)
        {
            var user = await _dispatcher.InvokeAsync(getUser, cancellationToken).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return user switch { { } => Ok(user), _ => NotFound() };
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IAsyncEnumerable<Log>))]
        public async IAsyncEnumerable<Log> GetAsync([FromQuery] EventLogList eventLogList, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var eventLog in _dispatcher.InvokeAsync(eventLogList, cancellationToken).ConfigureAwait(false))
                yield return eventLog;
        }
    }
}
