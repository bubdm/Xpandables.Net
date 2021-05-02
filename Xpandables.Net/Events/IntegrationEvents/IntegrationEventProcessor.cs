
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

using Xpandables.Net.Database;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The default implementation of <see cref="IIntegrationEventProcessor"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class IntegrationEventProcessor : IIntegrationEventProcessor
    {
        private readonly IEventBus _eventBus;
        private readonly IDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="IntegrationEventProcessor"/>.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="context">The data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventBus"/> or <paramref name="context"/> is null.</exception>
        public IntegrationEventProcessor(IEventBus eventBus, IDataContext context)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _context = context;
        }

        ///<inheritdoc/>
        public virtual async Task PushPendingMessages()
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

                await _eventBus.PublishAsync(@event).ConfigureAwait(false);

                entity.Deactivated();
                entity.Deleted();
                await _context.UpdateAsync(entity).ConfigureAwait(false);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
