
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
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Mvc;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Expressions;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Queries;
using Xpandables.Net.Validations;

namespace Xpandables.Net.Api.Contracts
{
    public sealed class AuthenToken : Response<AuthenToken>
    {
        internal AuthenToken AddToken(string token) => this.Assign(s => s.Token = token);
        internal AuthenToken AddType(string type) => this.Assign(s => s.Type = type);
        internal AuthenToken AddExpiry(DateTime expiry) => this.Assign(s => s.Expiry = expiry);
        public string Token { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime Expiry { get; set; }
    }

    [HttpRestClient(Path = "api/authenticate", Method = "Post", IsSecured = false, IsNullable = true)]
    public sealed class RequestAuthenToken : QueryExpression<User>, IQuery<AuthenToken>, IValidationDecorator
    {
        public RequestAuthenToken() { }
        public RequestAuthenToken(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }

        public override Expression<Func<User, bool>> GetExpression() => user => user.Phone.Value == Phone && user.IsActive && !user.IsDeleted;

        [Required, FromHeader]
        public string Phone { get; set; } = null!;
        [Required, FromHeader]
        public string Password { get; set; } = null!;
    }
}
