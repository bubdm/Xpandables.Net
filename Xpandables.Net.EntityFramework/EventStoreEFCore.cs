
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Xpandables.Net.Database;
using Xpandables.Net.Entities;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class EventStoreEFCore : OperationResultBase, IEventStore
    {
        private readonly IDataContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public EventStoreEFCore(IDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IOperationResult> AppendEventAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            var entityEvent = new EntityEvent(
                @event.Guid,
                @event.AggregateId,
                @event.GetType().AssemblyQualifiedName!,
                true,
                Serialize(@event));

            await _context.InsertAsync(entityEvent, cancellationToken).ConfigureAwait(false);
            return OkOperation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggreagateId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<IDomainEvent> ReadEventsAsync(Guid aggreagateId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var entityEvent in _context.FetchAllAsync<EntityEvent, EntityEvent>(
                e => e.Where(x => x.AggregateId == aggreagateId), cancellationToken)
                .ConfigureAwait(false))
            {
                if (Deserialize(entityEvent.Type, entityEvent.Data) is { } @event)
                    yield return @event;
            }
        }

        private static byte[] Serialize(IDomainEvent @event) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
        private static IDomainEvent? Deserialize(string eventType, byte[] data)
        {
            JsonSerializerSettings settings = new() { ContractResolver = new PrivateSetterContractResolver() };
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType(eventType), settings) as IDomainEvent;
        }
    }

    class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
