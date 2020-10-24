
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
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Expressions;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Contracts
{
    public sealed class Log
    {
        public Log(string name, DateTime occuredOn, string description)
        {
            Name = name;
            OccuredOn = occuredOn;
            Description = description;
        }

        public string Name { get; set; } = null!;
        public DateTime OccuredOn { get; set; }
        public string Description { get; set; } = null!;

        public override string ToString() => $"{Name} - {OccuredOn} - {Description}";
    }

    [HttpRestClient(Path = "api/user", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Query)]
    public sealed class EventLogList : QueryExpression<EventLog>, IAsyncQuery<Log>, IQueryStringLocationRequest
    {

        public EventLogList() { }

        public EventLogList(string? name, DateTime? startOccuredOn, DateTime? endOccuredOn)
        {
            Name = name;
            StartOccuredOn = startOccuredOn;
            EndOccuredOn = endOccuredOn;
        }

        public IDictionary<string, string?>? GetQueryStringSource() => new Dictionary<string, string?>
        {
            { nameof(Name), Name },
            { nameof(StartOccuredOn), StartOccuredOn?.ToString("yyyy-MM-dd HH:mm:ss") },
            { nameof(EndOccuredOn), EndOccuredOn?.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        public override Expression<Func<EventLog, bool>> GetExpression()
        {
            var queryExpression = QueryExpressionFactory.Create<EventLog>();
            if (Name is not null) queryExpression = queryExpression.And(el => el.EventName.Contains(Name));
            if (StartOccuredOn is not null) queryExpression = queryExpression.And(el => el.OccuredOn >= StartOccuredOn.Value);
            if (EndOccuredOn is not null) queryExpression = queryExpression.And(el => el.OccuredOn <= EndOccuredOn.Value);

            return queryExpression;
        }

        public string? Name { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? StartOccuredOn { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? EndOccuredOn { get; set; }
    }
}
