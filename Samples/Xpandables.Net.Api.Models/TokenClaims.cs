
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Xpandables.Net.Enumerations;

namespace Xpandables.Net.Api.Models
{
    public sealed class TokenClaimType
    {
        public const string Id = "id";
        public const string Phone = "phone";
        public const string EmailAddress = "emailaddress";
        public const string Role = ClaimTypes.Role;
    }

    public sealed class TokenClaims : IEnumerable<Claim>
    {
        public TokenClaims(string id, PhoneNumber phone, Role role, EmailAddress emailAddress)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
            Token = default!;
        }

        public TokenClaims(string id, PhoneNumber phone, Role role, EmailAddress emailAddress, string token)
            : this(id, phone, role, emailAddress) => Token = token;

        public TokenClaims(IEnumerable<Claim> claims)
        {
            var dictClaims = claims.ToDictionary(d => d.Type, d => d.Value);
            Id = dictClaims[TokenClaimType.Id];
            Phone = new PhoneNumber(dictClaims[TokenClaimType.Phone]);
            Role = EnumerationType.FromDisplayName<Role>(dictClaims[TokenClaimType.Role])!;
            EmailAddress = new EmailAddress(dictClaims[TokenClaimType.EmailAddress]);
            Token = default!;
        }

        public TokenClaims(IEnumerable<Claim> claims, string token)
            : this(claims) => Token = token;

        public string Id { get; }
        public PhoneNumber Phone { get; }
        public Role Role { get; }
        public EmailAddress EmailAddress { get; }
        public string Token { get; }

        public IEnumerator<Claim> GetEnumerator()
        {
            yield return new Claim(TokenClaimType.Id, Id);
            yield return new Claim(TokenClaimType.Phone, Phone.Value);
            yield return new Claim(TokenClaimType.Role, Role);
            yield return new Claim(TokenClaimType.EmailAddress, EmailAddress);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
