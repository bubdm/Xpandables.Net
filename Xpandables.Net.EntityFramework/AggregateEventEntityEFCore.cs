
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
using System.Reflection;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Xpandables.Net.Entities;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents an aggregate event to be written for EFCore using Newtonsoft.
    /// </summary>
    public class AggregateEventEntityEFCore : AggregateEventEntity
    {
        ///<inheritdoc/>
        [JsonConstructor]
        protected AggregateEventEntityEFCore(Guid eventId, Guid aggregateId, string type, long version, bool isJson, byte[] data)
            : base(eventId, aggregateId, type, version, isJson, data) { }

        ///<inheritdoc/>
        public AggregateEventEntityEFCore(IDomainEvent @event) : base(@event)
        {
        }

        ///<inheritdoc/>
        protected override byte[] Serialize(IDomainEvent @event)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

        ///<inheritdoc/>
        public override IDomainEvent? Deserialize()
        {
            JsonSerializerSettings settings = new() { ContractResolver = new PrivateSetterContractResolver() };
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Data), System.Type.GetType(Type)!, settings) as IDomainEvent;
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
