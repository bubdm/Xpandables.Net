
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Identities;
using Xpandables.Net.Queries;
using Xpandables.Net.Validations;

namespace Xpandables.Net.Api.Contracts
{
    public sealed class UserItem
    {
        public UserItem(string id, string phone, string email)
        {
            Id = id;
            Phone = phone;
            Email = email;
        }

        public string Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    [HttpRestClient(Path = "api/user/getuser", IsSecured = true, IsNullable = true, Method = "Get", In = ParameterLocation.Query)]
    public sealed class GetUser : IdentityDataExpression<TokenClaims, User>, IAsyncQuery<UserItem>, IValidationDecorator, IIdentityDecorator, IQueryStringRequest
    {
        public GetUser(string? id) => Id = id;

        public GetUser() { }

        protected override Expression<Func<User, bool>> BuildExpression()
        {
            Id ??= Identity.Id;
            return user => user.Id == Id && user.IsActive && !user.IsDeleted;
        }

        public IDictionary<string, string?> GetQueryString() => new Dictionary<string, string?> { { nameof(Id), Id } };

        public string? Id { get; set; }
    }
}
