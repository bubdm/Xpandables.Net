using System;

using Xpandables.Net5.Http;
using Xpandables.Net5.Identities;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Application.Services
{
    public class IdentityDataProvider : IIdentityProvider
    {
        private readonly HttpTokenContainer _securedTokenContainer;

        public IdentityDataProvider(HttpTokenContainer securedTokenContainer)
        {
            _securedTokenContainer = securedTokenContainer ?? throw new ArgumentNullException(nameof(securedTokenContainer));
        }

        public object GetIdentity()
        {
            var token = _securedTokenContainer.GetToken("Authorization") ?? throw new InvalidOperationException("Expected token not found");
            var claims = _securedTokenContainer.GetClaims(token);

            return UserFactory.ToClaims(claims);
        }
    }
}
