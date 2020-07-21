using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Cryptography;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Helpers;
using Xpandables.Net.Http;
using Xpandables.Net.Queries;
using Xpandables.Samples.Business.Contracts;
using Xpandables.Samples.Business.Localization;
using Xpandables.Samples.Business.Services;

namespace Xpandables.Samples.Business.Handlers
{
    public sealed class SignInRequestHandler : IQueryHandler<SignInRequest, SignInResponse>
    {
        private readonly IDataContext _dataContext;
        private readonly IHttpTokenEngine _tokenEngine;
        private readonly HttpIPService _httpIPService;
        private readonly IStringCryptography _stringCryptography;

        public SignInRequestHandler(IDataContext dataContext, IHttpTokenEngine tokenEngine, HttpIPService httpIPService, IStringCryptography stringCryptography)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tokenEngine = tokenEngine ?? throw new ArgumentNullException(nameof(tokenEngine));
            _httpIPService = httpIPService ?? throw new ArgumentNullException(nameof(httpIPService));
            _stringCryptography = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));
        }

        public async Task<SignInResponse> HandleAsync(SignInRequest query, CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.SetOf(query).FirstOrDefaultAsync(query, cancellationToken).ConfigureAwait(false);

            if (user?.Password.IsEqualTo(query.Password, _stringCryptography) != true)
            {
                throw this.GetValidationException(
                    ErrorMessages.UserNotExist.StringFormat(query.Email),
                    query.Email,
                    nameof(query.Email), nameof(query.Password));
            }

            var token = user.GetToken(_tokenEngine);
            var location = await _httpIPService.GetIPGeoLocationAsync().ConfigureAwait(false);

            return new SignInResponse(
                token, user.Email, user.Name.LastName, user.Name.FirstName, user.Gender, location);
        }
    }
}
