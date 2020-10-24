﻿
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class GetUserHandler : IQueryHandler<GetUser, UserItem>
    {
        private readonly IDataContext _dataContext;

        public GetUserHandler(IDataContext dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public async Task<UserItem> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
        {
            //var user = await _dataContext.FindAsync<User>(u => u.Where(query), cancellationToken).ConfigureAwait(false);
            var user = await _dataContext.SetOf<User>().FirstAsync(query, cancellationToken).ConfigureAwait(false);
            return new UserItem(user!.Id, user.Phone, user.Email);
        }
    }
}
