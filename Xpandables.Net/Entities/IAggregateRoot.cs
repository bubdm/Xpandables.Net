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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Gets the collection of events occurred.
        /// </summary>
        IReadOnlyCollection<IEvent> Events { get; }

        /// <summary>
        /// Adds the specified event to the entity collection of events.
        /// </summary>
        /// <param name="event">The event to be added.</param>
        /// <exception cref=" ArgumentNullException">The <paramref name="event"/> is null. </exception>
        void AddEvent(IEvent @event);

        /// <summary>
        /// Clears all events.
        /// </summary>
        void ClearEvents();

        /// <summary>
        /// Clears all events that match the specified type of events.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to search for.</typeparam>
        void ClearEvents<TEvent>() where TEvent : class, IEvent;

        /// <summary>
        /// Removes the specified event from the entity collection of events.
        /// </summary>
        /// <param name="event">The event to be removed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        void RemoveEvent(IEvent @event);
    }
}