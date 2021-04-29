﻿/************************************************************************************************************
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

using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets the aggregate unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///   /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Marks all the events as committed.
        /// </summary>
        void MarkEventsAsCommitted();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        IEnumerable<IDomainEvent> GetUncommittedEvents();
    }
}