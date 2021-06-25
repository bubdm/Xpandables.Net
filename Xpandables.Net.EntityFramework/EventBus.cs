
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
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Database;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEventBus"/> interface.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly AggregateDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="EventBus"/>.
        /// </summary>
        /// <param name="notificationPublisher">The notification publisher.</param>
        /// <param name="context">The data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notificationPublisher"/>
        /// or <paramref name="context"/> is null.</exception>
        public EventBus(INotificationPublisher notificationPublisher, IAggregateDataContext context)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _context = (AggregateDataContext)context;
        }

        ///<inheritdoc/>
        public async Task PushAsync()
        {
            var updatedNotifications = new List<EventStoreEntity>();
            var notifications = await FetchPendingNotifications().ConfigureAwait(false);

            foreach (var entity in notifications)
            {
                if (await TryPushAsync(entity).ConfigureAwait(false))
                    updatedNotifications.Add(entity);
            }

            if (updatedNotifications.Count > 0)
                await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        async Task<List<EventStoreEntity>> FetchPendingNotifications()
        {
            var criteria = new EventStoreEntityCriteria()
            {
                IsDeleted = false,
                IsActive = true,
                Count = 50
            };

            return await _context.Notifications
                    .AsNoTracking()
                    .Where(criteria)
                    .OrderBy(o => o.CreatedOn)
                    .Take(criteria.Count.Value)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        async Task<bool> TryPushAsync(EventStoreEntity entity)
        {
            try
            {
                var type = Type.GetType(entity.EventTypeFullName);
                if (type is null)
                    return false;

                if (entity.To(type) is not IEvent @event)
                    return false;

                await _notificationPublisher.PublishAsync(@event).ConfigureAwait(false);

                entity.Deactivated();
                entity.Deleted();

                _context.Notifications.Update(entity);

                return true;
            }
            catch (Exception exception)
            {
#if DEBUG
                Trace.WriteLine(exception);
#endif
                return false;
            }
        }
    }
}
