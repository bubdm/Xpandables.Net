﻿
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Database;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The integration event publisher.
    /// </summary>
    public sealed class IntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly IDataContext _dataContext;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Initializes a new instance of a <see cref="IntegrationEventPublisher"/> class with the data context and the event publisher.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="eventPublisher">The event publisher.</param>
        public IntegrationEventPublisher(IDataContext dataContext, IEventPublisher eventPublisher)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        /// <summary>
        /// Publishes integration events.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        public async Task PublishAsync(CancellationToken cancellationToken = default)
        {
            var domainEventTasks = _dataContext.Notifications
                .OfType<IIntegrationEvent>()
                .Select(integrationEvent => _eventPublisher.PublishAsync(integrationEvent, cancellationToken));

            await Task.WhenAll(domainEventTasks).ConfigureAwait(false);
            _dataContext.ClearNotifications<IIntegrationEvent>();
        }
    }
}
