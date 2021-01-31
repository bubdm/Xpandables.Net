
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

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// Allows an application author to define a handler for an integration domain event.
    /// The event must implement <see cref="IIntegrationEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IIntegrationEventHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handle the integration domain event.
        /// </summary>
        /// <param name="integrationEvent">The event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="integrationEvent"/> is null.</exception>
        Task HandleAsync(object integrationEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// The event must implement <see cref="IIntegrationEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TEvent">The event type to be handled.</typeparam>
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler, ICanHandle<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Asynchronously handles the event.
        /// </summary>
        /// <param name="integrationEvent">The event instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="integrationEvent"/> is null.</exception>
        Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken = default);

        Task IIntegrationEventHandler.HandleAsync(object integrationEvent, CancellationToken cancellationToken)
        {
            if (integrationEvent is TEvent integrationInstance)
                return HandleAsync(integrationInstance, cancellationToken);

            throw new ArgumentNullException(nameof(integrationEvent));
        }
    }
}
