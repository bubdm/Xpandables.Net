using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net5.EntityFramework;
using Xpandables.Net5.Http;
using Xpandables.Net5.Queries;
using Xpandables.Samples.Application.Contracts;
using Xpandables.Samples.Application.Services;

namespace Xpandables.Samples.Application.Handlers
{
    public sealed class SignInRequestHandler : IQueryHandler<SignInRequest, SignInResponse>
    {
        private readonly IDataContext _dataContext;
        private readonly IHttpTokenEngine _tokenEngine;
        private readonly HttpIPService _httpIPService;

        public SignInRequestHandler(IDataContext dataContext, IHttpTokenEngine tokenEngine, HttpIPService httpIPService)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tokenEngine = tokenEngine ?? throw new ArgumentNullException(nameof(tokenEngine));
            _httpIPService = httpIPService ?? throw new ArgumentNullException(nameof(httpIPService));
        }

        public async Task<SignInResponse> HandleAsync(SignInRequest query, CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.SetOf(query).FirstAsync(query, cancellationToken).ConfigureAwait(false);
            var token = user.GetToken(_tokenEngine);
            var location = await _httpIPService.GetIPGeoLocationAsync().ConfigureAwait(false);

            return new SignInResponse(
                token, user.Email, user.Name.LastName, user.Name.FirstName, user.Gender, location);
        }
    }
}
