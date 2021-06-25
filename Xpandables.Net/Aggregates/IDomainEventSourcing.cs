
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
using System.Linq;
using System.Runtime.CompilerServices;

using Xpandables.Net.DomainEvents;

[assembly: InternalsVisibleTo("Xpandables.Net.EntityFramework, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]
namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Event-sourcing pattern interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    internal interface IDomainEventSourcing<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the unique identifier of the aggregate.
        /// </summary>
        TAggregateId AggregateId { get; }

        /// <summary>
        /// Marks all the domain events as committed.
        /// </summary>
        void MarkEventsAsCommitted();

        /// <summary>
        /// Returns a collection of uncommitted events.
        /// </summary>
        /// <returns>A list of uncommitted events.</returns>
        IOrderedEnumerable<IDomainEvent<TAggregateId>> GetUncommittedEvents();

        /// <summary>
        /// Initializes the underlying aggregate with the specified history collection of events.
        /// </summary>
        /// <param name="events">The collection of events to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="events"/> is null.</exception>
        void LoadFromHistory(IOrderedEnumerable<IDomainEvent<TAggregateId>> events);

        /// <summary>
        /// Applies the history specified domain event to the underlying aggregate.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        void LoadFromHistory(IDomainEvent<TAggregateId> @event);

        /// <summary>
        /// Applies the specified event to the current instance.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentException">The <paramref name="event"/> is null.</exception>
        void Apply(IDomainEvent<TAggregateId> @event);

        /// <summary>
        /// Applies the mutation calling the handler that matches the specified event.
        /// </summary>
        /// <param name="event">The event to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The expected handler is not registered.</exception>
        void Mutate(IDomainEvent<TAggregateId> @event);
    }
}
