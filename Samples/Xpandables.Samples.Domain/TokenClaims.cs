using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Xpandables.Net;

namespace Xpandables.Samples.Domain
{
    public static class ClaimsFactory
    {
        public static TokenClaims Create(IEnumerable<Claim> claims)
        {
            var dictClaims = claims.ToDictionary(d => d.Type, d => d.Value);
            return new TokenClaims(
                dictClaims[TokenClaimType.ID],
                dictClaims[TokenClaimType.Email],
                dictClaims[TokenClaimType.Expiration]);
        }

        public static TokenClaims Create(string id, string email, string expiration)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            _ = email ?? throw new ArgumentNullException(nameof(email));
            _ = expiration ?? throw new ArgumentNullException(nameof(expiration));

            return new TokenClaims(id, email, expiration);
        }
    }

    public sealed class TokenClaimType
    {
        public const string ID = "id";
        public const string Email = "email";
        public const string Expiration = ClaimTypes.Expiration;
    }

    public sealed class TokenClaims : ValueObject, IEnumerable<Claim>
    {
        internal TokenClaims(string id, string email, string expiration)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Expiration = expiration ?? throw new ArgumentNullException(nameof(expiration));
        }

        public string Id { get; }
        public string Email { get; }
        public string Expiration { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Email;
            yield return Expiration;
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            yield return new Claim(TokenClaimType.ID, Id);
            yield return new Claim(TokenClaimType.Email, Email);
            yield return new Claim(TokenClaimType.Expiration, Expiration);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
