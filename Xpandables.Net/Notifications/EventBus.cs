
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
using System.Text;
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
        private readonly IEventStoreDataContext _context;
        private readonly IStoreEntityConverter _converter;

        /// <summary>
        /// Constructs a new instance of <see cref="EventBus"/>.
        /// </summary>
        /// <param name="notificationPublisher">The notification publisher.</param>
        /// <param name="context">The data context.</param>
        /// <param name="converter">The converter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notificationPublisher"/> or <paramref name="context"/> is null.</exception>
        public EventBus(INotificationPublisher notificationPublisher, IEventStoreDataContext context, IStoreEntityConverter converter)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        ///<inheritdoc/>
        public async Task PushAsync()
        {
            var updatedNotifications = new List<NotificationEntity>();
            var notifications = await FetchPendingNotifications().ConfigureAwait(false);

            foreach (var entity in notifications)
            {
                if (!await TryPushAsync(entity).ConfigureAwait(false))
                    break;
                else
                    updatedNotifications.Add(entity);
            }

            if (updatedNotifications.Count > 0)
                await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        async Task<List<NotificationEntity>> FetchPendingNotifications()
        {
            var results = new List<NotificationEntity>();
            await foreach (var entity in _context.FetchAllAsync<NotificationEntity, NotificationEntity>(
                           entity => entity
                               .Where(w => !w.IsDeleted && w.IsActive)
                               .OrderBy(o => o.CreatedOn)
                               .Take(50)))
                results.Add(entity);

            return results;
        }

        async Task<bool> TryPushAsync(NotificationEntity entity)
        {
            try
            {
                if (_converter.Deserialize(Encoding.UTF8.GetString(entity.Data), Type.GetType(entity.Type)!) is not INotification @event)
                    return false;

                await _notificationPublisher.PublishAsync(@event).ConfigureAwait(false);

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
