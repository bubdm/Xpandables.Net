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

using Xpandables.Net.Aggregates;

namespace Xpandables.Net
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as an event : Domain event or Integration event.
    /// The events can be raised using the differed approach described by "Jimmy Bogard"
    /// </summary>
    public interface IEvent : ICommandQueryEvent
    {
        /// <summary>
        /// Gets the identifier of the target aggregate.
        /// </summary>
        AggregateId AggregateId { get; }

        /// <summary>
        /// Gets the event identifier.
        /// </summary>
        Guid Guid { get; }
    }
}
