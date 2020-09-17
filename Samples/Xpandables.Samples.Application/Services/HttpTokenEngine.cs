using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Xpandables.Net.Http;
using Xpandables.Net.Types;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Business.Services
{
    public sealed class HttpTokenEngine : IHttpTokenEngine
    {
        public Token WriteToken(IEnumerable<Claim> claims)
        {
            if (claims is null) throw new ArgumentNullException(nameof(claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your secret key for authentication"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "connected",
                Audience = "connected",
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return Token.Create(token, "Bearer", tokenDescriptor.Expires.Value);
        }

        public IEnumerable<Claim> ReadToken(string source)
        {
            if (source.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase))
                source = source.Replace("Bearer", string.Empty).TrimStart().TrimEnd();

            var handler = new JwtSecurityTokenHandler();
            var securedToken = handler.ReadJwtToken(source);
            var claims = securedToken.Claims.ToList();
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, claims.First(f => f.Type == "unique_name").Value));
            return claims;
        }

        public object ReadIdentity(IEnumerable<Claim> claims) => UserFactory.ToClaims(claims);

    }
}
