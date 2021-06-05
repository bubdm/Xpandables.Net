
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

using Xpandables.Net.Aggregates;

namespace Xpandables.Net.DomainEvents
{
    /// <summary>
    /// Allows an application author to define a handler for a domain event.
    /// The event must implement <see cref="IDomainEvent{TAggregateId}"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDomainEventHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handle the domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEvent"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="domainEvent"/> does not implement <see cref="IDomainEvent{TAggregateId}"/>.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(object domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// The event must implement <see cref="IDomainEvent{TAggregateId}"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TEvent">The event type to be handled.</typeparam>
    public interface IDomainEventHandler<TAggregateId, in TEvent> : IDomainEventHandler, ICanHandle<TEvent>
        where TEvent : class, IDomainEvent<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        /// <summary>
        /// Asynchronously handles the domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEvent"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);

        Task IDomainEventHandler.HandleAsync(object domainEvent, CancellationToken cancellationToken)
        {
            if (domainEvent is TEvent eventInstance)
                return HandleAsync(eventInstance, cancellationToken);

            throw new ArgumentException($"The parameter does not implement {nameof(IDomainEvent<TAggregateId>)} interface.", nameof(domainEvent));
        }
    }
}
