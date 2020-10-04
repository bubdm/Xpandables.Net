
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
using System.Threading;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Expressions;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class EventLogListHandler : IAsyncQueryHandler<EventLogList, Log>
    {
        private readonly IDataContext _dataContext;

        public EventLogListHandler(IDataContext dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public IAsyncEnumerable<Log> HandleAsync(EventLogList query, CancellationToken cancellationToken = default)
        {
            var queryExpression = QueryExpressionFactory.Create<EventLog>();
            if (query.Name is not null) queryExpression = queryExpression.And(el => el.EventName.Contains(query.Name));
            if (query.StartOccuredOn is not null) queryExpression = queryExpression.And(el => el.OccuredOn >= query.StartOccuredOn.Value);
            if (query.EndOccuredOn is not null) queryExpression = queryExpression.And(el => el.OccuredOn <= query.EndOccuredOn.Value);

            return _dataContext.FindAllAsync<User, Log>(u => u
                 .AsNoTracking()
                 .Include(i => i.EventLogs)
                 .SelectMany(user => user.EventLogs)
                 .Where(queryExpression)
                 .Select(log => new Log(log.EventName, log.OccuredOn, log.Description)), cancellationToken);
        }
    }
}
