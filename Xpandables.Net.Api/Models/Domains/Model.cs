
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
using System.Text;

namespace Xpandables.Net.Api.Models.Domains
{
    public abstract class Model : Entity
    {
        protected override string KeyGenerator()
        {
            var stringBuilder = new StringBuilder();

            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(_ => Guid.NewGuid())
                .Take(32)
                .ToList()
                .ForEach(e => stringBuilder.Append(e));

            return stringBuilder.ToString().ToUpperInvariant();
        }

        protected void AddEventLog(string eventName, string description)
        {
            _ = eventName ?? throw new ArgumentNullException(nameof(eventName));
            _ = description ?? throw new ArgumentNullException(nameof(description));

            _eventLogs.Add(new EventLog(eventName, DateTime.UtcNow, description ?? throw new ArgumentNullException(nameof(description))));
        }

        protected readonly HashSet<EventLog> _eventLogs = new HashSet<EventLog>();
        public IEnumerable<EventLog> EventLogs => _eventLogs.AsEnumerable();
    }
}
