
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

using Xpandables.Net.Database;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as an integration domain event.
    /// This kind of events are published after <see cref="IDataContext.PersistAsync(System.Threading.CancellationToken)"/>
    /// completed and the entity change successfully saved to the database.
    /// </summary>
    public interface IIntegrationEvent : IEvent
    {
        /// <summary>
        /// When the event occurred.
        /// </summary>
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as an integration domain event targeting an <see cref="IDomainEvent"/>.
    /// This kind of events are published after <see cref="IDataContext.PersistAsync(System.Threading.CancellationToken)"/>
    /// completed and the entity change successfully saved to the database.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    public interface IIntegrationEvent<out TDomainEvent> : IIntegrationEvent
        where TDomainEvent : notnull, IDomainEvent
    {
        /// <summary>
        /// Gets the target domain event.
        /// </summary>
        TDomainEvent DomainEvent { get; }
    }
}
