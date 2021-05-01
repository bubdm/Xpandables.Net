
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
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;

using Xpandables.Net.Entities;
using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents an out box message to be written using Newtonsoft.
    /// </summary>
    public class IntegrationEventEntityEFCore : IntegrationEventEntity
    {
        /// <summary>
        /// Constructs anew instance of <see cref="IntegrationEventEntityEFCore"/> from the specified event.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public IntegrationEventEntityEFCore(IIntegrationEvent @event) : base(@event) { }

        ///<inheritdoc/>
        protected override byte[] Serialize(IIntegrationEvent @event)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

        ///<inheritdoc/>
        public override IIntegrationEvent? Deserialize()
        {
            JsonSerializerSettings settings = new() { ContractResolver = new PrivateSetterContractResolver() };
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Data), System.Type.GetType(Type)!, settings) as IIntegrationEvent;
        }
    }
}
