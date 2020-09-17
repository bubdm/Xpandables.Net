using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Xpandables.Net;
using Xpandables.Net.Cryptography;
using Xpandables.Net.Http;
using Xpandables.Net.Strings;
using Xpandables.Net.Types;

namespace Xpandables.Samples.Domain.Models
{
    public static class UserFactory
    {
        public static User Create(string email, string password, Picture picture)
        {
            IStringCryptography stringCryptography = new StringCryptography(new StringGenerator());
            var user = new User(email, stringCryptography.Encrypt(password), picture);
            user.Activate();
            return user;
        }

        public static TokenClaims ToClaims(IEnumerable<Claim> claims) => ClaimsFactory.Create(claims);
    }

    public sealed class User : Entity
    {
        public User() { }

        internal User(string email, ValueEncrypted password, Picture picture)
            : this(email)
        {
            Password = password;
            Picture = picture;
        }

        internal User(string email)
        {
            Email = email;

        }

        [Required, DataType(DataType.EmailAddress), StringLength(byte.MaxValue, MinimumLength = 3)]
        public string Email { get; private set; }

        [Required]
        public ValueEncrypted Password { get; private set; }

        [Required]
        public Picture Picture { get; private set; }

        public IEnumerable<Claim> GetClaims() => ClaimsFactory.Create(Id, Email, TimeSpan.FromMinutes(30).ToString());

        public Token GetToken(IHttpTokenEngine httpTokenEngine)
        {
            _ = httpTokenEngine ?? throw new ArgumentNullException(nameof(httpTokenEngine));
            return httpTokenEngine.WriteToken(GetClaims());
        }
    }
}
