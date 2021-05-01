using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Events;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Api.Services
{
    public class EventBus : IEventBus
    {
        public Task PublishAsync(IIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }

        void IEventBus.Subscribe<TEvent>(IIntegrationEventHandler<TEvent> handler)
        {
            throw new NotImplementedException();
        }

        void IEventBus.Unsubscribe<TEvent>(IIntegrationEventHandler<TEvent> handler)
        {
            throw new NotImplementedException();
        }
    }
}
