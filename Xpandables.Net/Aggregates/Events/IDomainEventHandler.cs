
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Aggregates.Events
{
    /// <summary>
    /// Allows an application author to define a handler for a domain event.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>    
    public interface IDomainEventHandler : IEventHandler
    {
        /// <summary>
        ///  Asynchronously handles the domain event.
        /// </summary>
        /// <param name="event">The domain event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

        Task IEventHandler.HandleAsync(object @event, CancellationToken cancellationToken)
            => HandleAsync((IDomainEvent)@event, cancellationToken);
    }

    /// <summary>
    /// Allows an application author to define a handler for a domain event.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TDomainEvent">The domain event type.</typeparam>
    public interface IDomainEventHandler<TDomainEvent> : IDomainEventHandler, IEventHandler<TDomainEvent>
        where TDomainEvent : class, IDomainEvent
    {
        /// <summary>
        ///  Asynchronously handles the domain event of specific type.
        /// </summary>
        /// <param name="event">The domain event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        new Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken = default);

        Task IEventHandler<TDomainEvent>.HandleAsync(TDomainEvent @event, CancellationToken cancellationToken)
            => HandleAsync(@event, cancellationToken);

        Task IDomainEventHandler.HandleAsync(IDomainEvent @event, CancellationToken cancellationToken)
            => HandleAsync((TDomainEvent)@event, cancellationToken);
    }
}
