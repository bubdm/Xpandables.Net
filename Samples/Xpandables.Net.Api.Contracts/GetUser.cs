
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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Expressions;
using Xpandables.Net.HttpRestClient;
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

    [HttpRestClient(Path = "api/user/{id}", IsSecured = true, IsNullable = true, Method = "Get", In = ParameterLocation.Path)]
    public sealed class GetUser : QueryExpression<User>, IQuery<UserItem>, IValidationDecorator, IPathStringLocationRequest
    {
        [return: NotNull]
        public override Expression<Func<User, bool>> GetExpression() => user => user.Id == Id && user.IsActive && !user.IsDeleted;
        public GetUser(string id) => Id = id;

        public GetUser() => Id = null!;

        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };

        public string Id { get; set; }
    }
}
