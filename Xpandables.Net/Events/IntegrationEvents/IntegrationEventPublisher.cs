
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
using Xpandables.Net.Dispatchers;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The domain event publisher.
    /// </summary>
    public sealed class IntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly IDataContext _dataContext;
        private readonly IDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of a <see cref="IntegrationEventPublisher"/> class with the data context and the dispatcher.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public IntegrationEventPublisher(IDataContext dataContext, IDispatcher dispatcher)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        /// <summary>
        /// Publishes domain events from the data context.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        public async Task PublishAsync(CancellationToken cancellationToken = default)
        {
            var domainEventTasks = _dataContext.Notifications
                .OfType<IIntegrationEvent>()
                .Select(integrationEvent => _dispatcher.PublishAsync(integrationEvent, cancellationToken));

            await Task.WhenAll(domainEventTasks).ConfigureAwait(false);
            _dataContext.ClearNotifications<IIntegrationEvent>();
        }
    }
}
