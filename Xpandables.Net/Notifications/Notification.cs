
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

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="INotification{TAggregateId}"/>.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    public abstract class Notification<TAggregateId> : Event<TAggregateId>, INotification<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification{TAggregateId}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        protected Notification(TAggregateId aggregateId) : base(aggregateId) { }

        /// <summary>
        /// Constructs a new instance of <see cref="Notification{TAggregateId}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name.</param>
        protected Notification(TAggregateId aggregateId, DateTimeOffset occurredOn, string createdBy)
            : base(aggregateId, occurredOn, createdBy) { }
    }
}
