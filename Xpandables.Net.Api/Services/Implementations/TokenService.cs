
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Settings;
using Xpandables.Net.Http;
using Xpandables.Net.Types;

namespace Xpandables.Net.Api.Services.Implementations
{
    public sealed class TokenService : IHttpTokenEngine
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> options) => _jwtSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));

        public Token WriteToken(IEnumerable<Claim> claims)
        {
            _ = claims ?? throw new ArgumentNullException(nameof(claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JWT_Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.JWT_ExpirationMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                Issuer = _jwtSettings.JWT_Issuer,
                Audience = _jwtSettings.JWT_Issuer,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return Token.Create(token, "Bearer", expiry);
        }

        public IEnumerable<Claim> ReadToken(string token)
        {
            _ = token ?? throw new ArgumentNullException(nameof(token));

            if (token.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase))
                token = token.Replace("Bearer", string.Empty).TrimStart().TrimEnd();

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = new[] { new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.JWT_Secret)) },
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtSettings.JWT_Issuer,
                ValidAudience = _jwtSettings.JWT_Issuer
            };

            var handler = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
            return handler.Claims;
        }

        public object ReadTokenClaim(IEnumerable<Claim> claims) => new TokenClaims(claims);

        public object ReadTokenClaim(string token)
        {
            var claims = ReadToken(token);
            return ReadTokenClaim(claims);
        }
    }
}
