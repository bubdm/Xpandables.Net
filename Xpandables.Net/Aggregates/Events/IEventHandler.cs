
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
    /// Allows an application author to define a handler for an event.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>    
    public interface IEventHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handles the event.
        /// </summary>
        /// <param name="event">The event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(object @event, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for a generic event.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        ///  Asynchronously handles the event.
        /// </summary>
        /// <param name="event">The event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);

        Task IEventHandler.HandleAsync(object @event, CancellationToken cancellationToken)
            => HandleAsync((TEvent)@event, cancellationToken);
    }
}
