
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
using System.Threading.Tasks;

using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Defines an interface to subscribe and publish events
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes to the specified integration event type with the specified action.
        /// </summary>
        /// <typeparam name="TEvent">The type of the integration event.</typeparam>
        /// <param name="handler">The Action to invoke when an event of this type is published</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        void Subscribe<TEvent>(IIntegrationEventHandler<TEvent> handler) where TEvent : class, IIntegrationEvent;

        /// <summary>
        ///  Unsubscribes from the integration Event type related to the specified <paramref name="handler"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the integration event.</typeparam>
        /// <param name="handler">The action to unsubscribe from.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        void Unsubscribe<TEvent>(IIntegrationEventHandler<TEvent> handler) where TEvent : class, IIntegrationEvent;

        /// <summary>
        /// Asynchronously publishes the specified event to any subscribers.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <returns>a task that represents an asynchronous operation.</returns>
        Task PublishAsync(IIntegrationEvent @event);
    }
}
