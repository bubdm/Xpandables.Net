
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

using Xpandables.Net.Aggregates;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEvent{TAggreagate}"/> interface. 
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    public abstract class Event<TAggregateId> : CommandQueryEvent, IEvent<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        /// <summary>
        /// Constructs a new instance of <see cref="Event{TAggregateId}"/> with its target aggregate id.
        /// </summary>
        /// <param name="aggregateId">The target aggregate Id</param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        protected Event(TAggregateId aggregateId) : base() => AggregateId = aggregateId;

        ///<inheritdoc/>
        public TAggregateId AggregateId { get; protected set; }

        ///<inheritdoc/>
        public Guid Guid { get; } = Guid.NewGuid();

        IAggregateId IEvent.AggregateId => AggregateId;
    }
}
