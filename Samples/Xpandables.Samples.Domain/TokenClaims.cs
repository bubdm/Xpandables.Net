using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Xpandables.Net;
using Xpandables.Net.Entities;
using Xpandables.Net.Enumerations;

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
                dictClaims[TokenClaimType.FirstName],
                dictClaims[TokenClaimType.LastName],
                dictClaims[TokenClaimType.Expiration],
                EnumerationType.FromDisplayName<Gender>(dictClaims[TokenClaimType.Gender])!);
        }

        public static TokenClaims Create(string id, string email, string firstName, string lastName, string expiration, Gender gender)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            _ = email ?? throw new ArgumentNullException(nameof(email));
            _ = firstName ?? throw new ArgumentNullException(nameof(firstName));
            _ = lastName ?? throw new ArgumentNullException(nameof(lastName));
            _ = expiration ?? throw new ArgumentNullException(nameof(expiration));
            _ = gender;

            return new TokenClaims(id, email, firstName, lastName, expiration, gender);
        }
    }

    public sealed class TokenClaimType
    {
        public const string ID = "id";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string Email = "email";
        public const string Name = ClaimTypes.Name;
        public const string Gender = "gender";
        public const string Expiration = ClaimTypes.Expiration;
    }

    public sealed class TokenClaims : ValueObject, IEnumerable<Claim>
    {
        internal TokenClaims(string id, string email, string firstName, string lastName, string expiration, Gender gender)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Expiration = expiration ?? throw new ArgumentNullException(nameof(expiration));
            Gender = gender;
        }

        public string Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name => $"{FirstName} {LastName}";
        public string Expiration { get; }
        public Gender Gender { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Email;
            yield return FirstName;
            yield return LastName;
            yield return Name;
            yield return Expiration;
            yield return Gender;
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            yield return new Claim(TokenClaimType.ID, Id);
            yield return new Claim(TokenClaimType.Email, Email);
            yield return new Claim(TokenClaimType.FirstName, FirstName);
            yield return new Claim(TokenClaimType.LastName, LastName);
            yield return new Claim(TokenClaimType.Name, Name);
            yield return new Claim(TokenClaimType.Expiration, Expiration);
            yield return new Claim(TokenClaimType.Gender, Gender);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
