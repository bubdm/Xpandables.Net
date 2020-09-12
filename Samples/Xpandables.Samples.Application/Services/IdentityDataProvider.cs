using System;

using Xpandables.Net.Http;
using Xpandables.Net.Identities;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Business.Services
{
    public class IdentityDataProvider : IIdentityDataProvider
    {
        private readonly HttpTokenContainer _securedTokenContainer;

        public IdentityDataProvider(HttpTokenContainer securedTokenContainer) => _securedTokenContainer = securedTokenContainer ?? throw new ArgumentNullException(nameof(securedTokenContainer));

        public object GetIdentity()
        {
            var token = _securedTokenContainer.GetToken("Authorization") ?? throw new InvalidOperationException("Expected token not found");
            var claims = _securedTokenContainer.ReadToken(token);

            return UserFactory.ToClaims(claims);
        }
    }
}
