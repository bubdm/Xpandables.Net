
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
        private readonly INotificationEventStoreContext _notificationContext;

        /// <summary>
        /// Constructs a new instance of <see cref="EventBus"/>.
        /// </summary>
        /// <param name="notificationPublisher">The notification publisher.</param>
        /// <param name="notificationContext">The data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notificationPublisher"/>
        /// or <paramref name="notificationContext"/> is null.</exception>
        public EventBus(INotificationPublisher notificationPublisher,
                        INotificationEventStoreContext notificationContext)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _notificationContext = notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));
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
                await _notificationContext.SaveChangesAsync().ConfigureAwait(false);
        }

        async Task<List<EventStoreEntity>> FetchPendingNotifications()
        {
            var results = new List<EventStoreEntity>();
            var criteria = new EventStoreEntityCriteria() { IsDeleted = false, IsActive = true, Count = 50 };

            await foreach (var entity in _notificationContext.FetchAllAsync<EventStoreEntity, EventStoreEntity>(
                           entity => entity
                               .Where(criteria)
                               .OrderBy(o => o.CreatedOn)
                               .Take(criteria.Count.Value)))
                results.Add(entity);

            return results;
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

                await _notificationContext.UpdateAsync(entity).ConfigureAwait(false);

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
