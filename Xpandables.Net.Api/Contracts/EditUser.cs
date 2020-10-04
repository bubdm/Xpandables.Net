
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
using System.Linq.Expressions;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Commands;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Identities;

namespace Xpandables.Net.Api.Contracts
{
    [HttpRestClient(Path = "api/user", Method = "Post", IsSecured = true, IsNullable = false)]
    public sealed class EditUser : TokenClaimExpression<TokenClaims, User>, IAsyncCommand, ITokenClaimDecorator, IPersistenceDecorator
    {
        public EditUser(string? email, string? password, string? phone)
        {
            Email = email;
            Password = password;
            Phone = phone;
        }

        protected override Expression<Func<User, bool>> BuildExpression() => user => user.Phone.Value == Claims.Phone.Value && user.IsActive && !user.IsDeleted;

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
    }
}
