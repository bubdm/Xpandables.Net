using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Xpandables.Net5.Cryptography;
using Xpandables.Net5.Entities;
using Xpandables.Net5.Http;

namespace Xpandables.Samples.Domain.Models
{
    public static class UserFactory
    {
        public static User Create(string email, string firstName, string lastName, string password, Gender gender, Picture picture)
        {
            var user = new User(email, Name.Create(firstName, lastName), gender, new StringCryptography(new StringGenerator()).Encrypt(password), picture);
            user.Activate();
            return user;
        }

        public static TokenClaims ToClaims(IEnumerable<Claim> claims) => ClaimsFactory.Create(claims);
    }

    public sealed class User : Entity
    {
        internal User(string email, Name name, Gender gender, ValueEncrypted password, Picture picture)
            : this(email, gender)
        {
            Name = name;
            Password = password;
            Picture = picture;
        }

        internal User(string email, Gender gender)
        {
            Email = email;
            Gender = gender;

        }

        [Required, DataType(DataType.EmailAddress), StringLength(byte.MaxValue, MinimumLength = 3)]
        public string Email { get; private set; }

        [Required]
        public ValueEncrypted Password { get; private set; }

        [Required]
        public Name Name { get; private set; }

        [Required]
        public Gender Gender { get; private set; }

        [Required]
        public Picture Picture { get; private set; }

        public IEnumerable<Claim> GetClaims() => ClaimsFactory.Create(Id, Email, Name.FirstName, Name.LastName, TimeSpan.FromMinutes(30).ToString(), Gender);

        public string GetToken(IHttpTokenEngine httpTokenEngine)
        {
            _ = httpTokenEngine ?? throw new ArgumentNullException(nameof(httpTokenEngine));
            return httpTokenEngine.BuildToken(GetClaims());
        }
    }
}
