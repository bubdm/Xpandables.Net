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

namespace Xpandables.Net
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEvent"/> interface. 
    /// </summary>
    [Serializable]
    public abstract class Event : CommandQueryEvent, IEvent
    {
        /// <summary>
        /// Constructs a new instance of <see cref="Event"/> with its target aggregate id.
        /// </summary>
        /// <param name="aggregateId">The target aggregate Id</param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        protected Event(Guid aggregateId) : base() => AggregateId = aggregateId;

        /// <summary>
        /// Constructs a new instance of <see cref="Event"/> with the specified information.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        /// <param name="guid">The event identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        protected Event(Guid aggregateId, Guid guid, DateTimeOffset occurredOn, string createdBy)
            : base(guid, occurredOn, createdBy) => AggregateId = aggregateId;

        /// <summary>
        /// Gets the identifier of the target aggregate.
        /// </summary>
        public Guid AggregateId { get; protected set; }
    }
}