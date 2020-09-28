
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
using System.Runtime.CompilerServices;
using System.Threading;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Storage.Services;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class GetUserHandler : IAsyncQueryHandler<GetUser, UserItem>
    {
        private readonly IDataContext _dataContext;

        public GetUserHandler(IDataContext dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public async IAsyncEnumerable<UserItem> HandleAsync(GetUser query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.GetUserAsync(query, false, cancellationToken).ConfigureAwait(false);
            if (user is not null)
                yield return new UserItem(user.Id, user.Phone, user.Email);
            else
                yield break;
        }
    }
}
