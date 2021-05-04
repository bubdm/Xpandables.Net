
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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Database;
using Xpandables.Net.Entities;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEventBus"/> interface.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IIntegrationEventPublisher _integrationEventPublisher;
        private readonly IEventStoreDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="EventBus"/>.
        /// </summary>
        /// <param name="integrationEventPublisher">The integration event publisher.</param>
        /// <param name="context">The data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="integrationEventPublisher"/> or <paramref name="context"/> is null.</exception>
        public EventBus(IIntegrationEventPublisher integrationEventPublisher, IEventStoreDataContext context)
        {
            _integrationEventPublisher = integrationEventPublisher ?? throw new ArgumentNullException(nameof(integrationEventPublisher));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<inheritdoc/>
        public async Task PushAsync()
        {
            var updatedEvents = new List<IntegrationEventEntity>();
            await foreach (var entity in FetchPendingIntegrationEvents())
            {
                if (!await TryPushAsync(entity).ConfigureAwait(false))
                    break;
                else
                    updatedEvents.Add(entity);
            }

            if (updatedEvents.Count > 0)
                await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        IAsyncEnumerable<IntegrationEventEntity> FetchPendingIntegrationEvents()
             => _context.FetchAllAsync<IntegrationEventEntity, IntegrationEventEntity>(
                 entity => entity
                     .Where(w => !w.IsDeleted && w.IsActive)
                     .OrderBy(o => o.CreatedOn)
                     .Take(50));

        async Task<bool> TryPushAsync(IntegrationEventEntity entity)
        {
            try
            {
                if (entity.Deserialize() is not { } @event)
                    return false;

                await _integrationEventPublisher.PublishAsync(@event).ConfigureAwait(false);

                entity.Deactivated();
                entity.Deleted();
                await _context.UpdateAsync(entity).ConfigureAwait(false);

                return true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                return false;
            }
        }
    }
}
