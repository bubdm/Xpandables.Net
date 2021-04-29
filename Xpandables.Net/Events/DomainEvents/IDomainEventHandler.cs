
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

namespace Xpandables.Net.Events.DomainEvents
{
    /// <summary>
    /// Allows an application author to define a handler for a domain event.
    /// The event must implement <see cref="IDomainEvent"/> interface.
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
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(object domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// The event must implement <see cref="IDomainEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TEvent">The event type to be handled.</typeparam>
    public interface IDomainEventHandler<in TEvent> : IDomainEventHandler, ICanHandle<TEvent>
        where TEvent : class, IDomainEvent
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

            throw new ArgumentNullException(nameof(domainEvent));
        }
    }
}
