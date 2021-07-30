
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
using System.Text.Json.Serialization;

namespace Xpandables.Net.Aggregates.Events
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="INotification"/> interface.
    /// </summary>
    public abstract class Notification : Event, INotification
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        /// <param name="domainEvent">The target domain event.</param>
        protected Notification(AggregateId aggregateId, IDomainEvent? domainEvent = default)
            : base(aggregateId)
        {
            DomainEvent = domainEvent;
        }

        ///<inheritdoc/>
        [JsonIgnore]
        public IDomainEvent? DomainEvent { get; }
    }
}
