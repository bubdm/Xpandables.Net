
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
using System.Collections.Generic;

using Xpandables.Net.Events;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// using with EFCore, provides with members to track entities events.
    /// </summary>
    public interface IDataContextEventTracker
    {
        /// <summary>
        /// When used with EFCore, contains all events (domain events and integration events) from entities being tracked.
        /// </summary>
        IReadOnlyCollection<IEvent> Events { get; }

        /// <summary>
        /// Clears all events found in tracked entities that match the event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to clear.</typeparam>
        void ClearNotifications<TEvent>() where TEvent : IEvent;
    }
}
