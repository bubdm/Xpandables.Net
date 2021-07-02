
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
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Database;
using Xpandables.Net.Services;

namespace Xpandables.Net.NotificationEvents
{
    /// <summary>
    /// The default implementation of <see cref="INotificationEventService"/>.
    /// You can derive from this class to customize its behaviors or implement your own.
    /// </summary>
    public class NotificationEventService : BackgroundServiceBase<NotificationEventService>, INotificationEventService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Constructs a new instance of <see cref="NotificationEventService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">the scope factory.</param>
        public NotificationEventService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        ///<inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return PublishPendingNotificationEventsAsync(stoppingToken);
        }

        /// <summary>
        /// Returns the <see cref="TimeSpan"/> delay between each execution.
        /// The default value is 60s.
        /// </summary>
        /// <returns>A 60s timespan.</returns>
        protected virtual TimeSpan GetTimeSpanDelay() => TimeSpan.FromSeconds(60);


        /// <summary>
        /// Returns the criteria to search for pending notification events.
        /// </summary>
        /// <returns>An instance of <see cref="EventStoreEntityCriteria{TEventStoreEntity}"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        protected EventStoreEntityCriteria<NotificationEventStoreEntity> GetNotificationEventStoreEntityCriteria()
            => new()
            {
                IsDeleted = false,
                IsActive = true,
                Count = 50
            };

        /// <summary>
        /// Publishes pending notification events.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        protected virtual async Task PublishPendingNotificationEventsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
                using var scope = _serviceScopeFactory.CreateScope();

                var aggregateDataContext =(AggregateDataContext) scope.ServiceProvider.GetRequiredService<IAggregateDataContext>();
                var notificationEventPublisher = scope.ServiceProvider.GetRequiredService<INotificationEventPublisher>();

                var count = 0;
                var notificationEvents = await FetchPendingNotificationsAsync(aggregateDataContext).ConfigureAwait(false);

                foreach (var notificationEvent in notificationEvents)
                {
                    if (await TryPublishAsync(
                        notificationEvent,
                        notificationEventPublisher,
                        aggregateDataContext)
                        .ConfigureAwait(false))
                    {
                        count++;
                    }
                }

                if (count > 0)
                    await aggregateDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await Task.Delay(GetTimeSpanDelay(), cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Fetches pending notification events matching the <see cref="GetNotificationEventStoreEntityCriteria"/>.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <returns>A collection of <see cref="EventStoreEntity"/> matching the criteria.</returns>
        protected virtual async Task<List<NotificationEventStoreEntity>> FetchPendingNotificationsAsync(AggregateDataContext context)
        {
            var criteria = GetNotificationEventStoreEntityCriteria();

            return await context.NotificationEvents
                    .AsNoTracking()
                    .Where(criteria)
                    .OrderBy(o => o.CreatedOn)
                    .Take(criteria.Count ?? 0)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }


        /// <summary>
        /// Tries to publish the notification found in the entity.
        /// </summary>
        /// <param name="entity">The target entity to act on.</param>
        /// <param name="notificationEventPublisher">The notification event publisher to act with.</param>
        /// <param name="context">The data context to act on.</param>
        /// <returns>A task that represents an asynchronous boolean operation.</returns>
        protected virtual async Task<bool> TryPublishAsync(
            NotificationEventStoreEntity entity,
            INotificationEventPublisher notificationEventPublisher,
            AggregateDataContext context)
        {
            try
            {
                var type = Type.GetType(entity.EventTypeFullName);
                if (type is null)
                    return false;

                if (entity.ToObject(type) is not IEvent @event)
                    return false;

                // you can use an event bus or other
                await notificationEventPublisher.PublishAsync(@event).ConfigureAwait(false);

                entity.Deactivated();
                entity.Deleted();

                context.NotificationEvents.Update(entity);

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
