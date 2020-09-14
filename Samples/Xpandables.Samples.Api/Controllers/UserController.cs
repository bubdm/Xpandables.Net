using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Xpandables.Net.Dispatchers;
using Xpandables.Net.Enumerables;
using Xpandables.Samples.Business.Contracts;

namespace Xpandables.Samples.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public UserController(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        [HttpPost, AllowAnonymous]
        public async IAsyncEnumerable<SignInResponse> SignIn(SignInRequest signInRequest, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var result in _dispatcher.SendQueryAsync(signInRequest, cancellationToken).ConfigureAwait(false))
                yield return result;

            //var results = await _dispatcher.SendQueryAsync(signInRequest, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
            //return Ok(results.FirstOrDefault());
        }
    }
}
