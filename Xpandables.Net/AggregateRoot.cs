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

using Xpandables.Net.Events;

namespace Xpandables.Net
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    public abstract class AggregateRoot : Entity
    {
        internal readonly HashSet<IEvent> InternalEvents = new();

        /// <summary>
        /// Gets the collection of events occurred.
        /// </summary>
        public IReadOnlyCollection<IEvent> Events => InternalEvents;

        /// <summary>
        /// Adds the specified event to the entity collection of events.
        /// </summary>
        /// <param name="event">The event to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="event"/> is null. </exception>
        public void AddEvent(IEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));
            InternalEvents.Add(@event);
        }

        /// <summary>
        /// Removes the specified event from the entity collection of events.
        /// </summary>
        /// <param name="event">The event to be removed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public void RemoveEvent(IEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));
            InternalEvents.Remove(@event);
        }

        /// <summary>
        /// Clears all events.
        /// </summary>
        public void ClearEvents() => InternalEvents.Clear();
    }
}
