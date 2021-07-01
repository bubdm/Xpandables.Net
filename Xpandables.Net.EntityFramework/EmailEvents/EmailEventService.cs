
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

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// The default implementation of <see cref="IEmailEventService"/>.
    /// You can derive from this class to customize its behaviors or implement your own.
    /// </summary>
    public class EmailEventService : BackgroundServiceBase<EmailEventService>, IEmailEventService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Constructs a new instance of <see cref="EmailEventService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">the scope factory.</param>
        public EmailEventService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        ///<inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return SendPendingEmailEventsAsync(stoppingToken);
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
        protected virtual EventStoreEntityCriteria<EmailEventStoreEntity> GetEmailEventStoreEntityCriteria()
            => new()
            {
                IsDeleted = false,
                IsActive = true,
                Count = 50
            };

        /// <summary>
        /// Sends pending email events.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        protected virtual async Task SendPendingEmailEventsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
                using var scope = _serviceScopeFactory.CreateScope();

                var aggregateDataContext = (AggregateDataContext)scope.ServiceProvider.GetRequiredService<IAggregateDataContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var count = 0;
                var emailEvents = await FetchPendingEmailEventsAsync(aggregateDataContext).ConfigureAwait(false);

                foreach (var emailEvent in emailEvents)
                {
                    if (await TrySendEmailAsync(
                        emailEvent,
                        emailSender,
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
        /// Fetches pending email events matching the <see cref="GetEmailEventStoreEntityCriteria"/>.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <returns>A collection of <see cref="EventStoreEntity"/> matching the criteria.</returns>
        protected virtual async Task<List<EmailEventStoreEntity>> FetchPendingEmailEventsAsync(AggregateDataContext context)
        {
            var criteria = GetEmailEventStoreEntityCriteria();

            return await context.EmailEvents
                    .AsNoTracking()
                    .Where(criteria)
                    .OrderBy(o => o.CreatedOn)
                    .Take(criteria.Count ?? 0)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }


        /// <summary>
        /// Tries to send the email found in the entity.
        /// </summary>
        /// <param name="entity">The target entity to act on.</param>
        /// <param name="emailSender">The email sender to act with.</param>
        /// <param name="context">The data context to act on.</param>
        /// <returns>A task that represents an asynchronous boolean operation.</returns>
        protected virtual async Task<bool> TrySendEmailAsync(
            EmailEventStoreEntity entity,
            IEmailSender emailSender,
            AggregateDataContext context)
        {
            try
            {
                if (entity.To() is not IEmailEvent @event)
                    return false;

                await emailSender.SendEmailAsync(@event).ConfigureAwait(false);

                entity.Deactivated();
                entity.Deleted();

                context.EmailEvents.Update(entity);

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
