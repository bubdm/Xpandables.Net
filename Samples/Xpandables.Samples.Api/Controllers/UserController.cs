using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net5.Dispatchers;
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
        public async Task<SignInResponse> SignIn(SignInRequest signInRequest, CancellationToken cancellationToken)
                 => await _dispatcher.HandleQueryAsync(signInRequest, cancellationToken).ConfigureAwait(false);
    }
}
