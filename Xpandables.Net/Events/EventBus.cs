
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEventBus"/> interface.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IDictionary<Type, List<IIntegrationEventHandler>> _subscriptions;
        private static readonly object _subscriptionsLock = new();

        /// <summary>
        /// Constructs a new instance of <see cref="EventBus"/>.
        /// </summary>
        public EventBus() => _subscriptions = new Dictionary<Type, List<IIntegrationEventHandler>>();

        ///<inheritdoc/>
        public async Task PublishAsync(IIntegrationEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var handlers = new List<IIntegrationEventHandler>();

            lock (_subscriptionsLock)
            {
                if (_subscriptions.ContainsKey(@event.GetType()))
                    handlers = _subscriptions[@event.GetType()];
            }

            foreach (var handler in handlers)
                await handler.HandleAsync(@event).ConfigureAwait(false);
        }

        void IEventBus.Subscribe<TEvent>(IIntegrationEventHandler<TEvent> handler)
        {
            _ = handler ?? throw new ArgumentNullException(nameof(handler));

            lock (_subscriptionsLock)
            {
                if (!_subscriptions.ContainsKey(typeof(TEvent)))
                    _subscriptions.Add(typeof(TEvent), new());

                _subscriptions[typeof(TEvent)].Add(handler);
            }
        }

        void IEventBus.Unsubscribe<TEvent>(IIntegrationEventHandler<TEvent> handler)
        {
            _ = handler ?? throw new ArgumentNullException(nameof(handler));

            lock (_subscriptionsLock)
            {
                if (_subscriptions.ContainsKey(typeof(TEvent)))
                {
                    var allSubscriptions = _subscriptions[typeof(TEvent)];
                    var subscriptionToRemove = allSubscriptions.FirstOrDefault(h => handler.GetType().Equals(h.GetType()));
                    if (subscriptionToRemove is not null)
                        _subscriptions[typeof(TEvent)].Remove(subscriptionToRemove);
                }
            }
        }
    }
}
