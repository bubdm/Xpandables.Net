
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// The data context event publisher.
    /// </summary>
    /// <typeparam name="TNotification">The type of event.</typeparam>
    public sealed class DataContextEventPublisher<TNotification> : IDataContextEventPublisher<TNotification>
        where TNotification : class, INotification
    {
        private readonly IDataContext _dataContext;
        private readonly INotificationDispatcher _notificationDispatcher;

        /// <summary>
        /// Initializes a new instance of a <see cref="DataContextEventPublisher{TNotification}"/> class with the data context and the dispatcher.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="notificationDispatcher">The event dispatcher.</param>
        public DataContextEventPublisher(IDataContext dataContext, INotificationDispatcher notificationDispatcher)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _notificationDispatcher = notificationDispatcher ?? throw new ArgumentNullException(nameof(notificationDispatcher));
        }

        /// <summary>
        /// Publishes events (domain/integration) from the data context.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        public async Task Publisher(CancellationToken cancellationToken = default)
        {
            var domainEventTasks = _dataContext.Notifications
                .OfType<TNotification>()
                .Select(async integrationEvent => await _notificationDispatcher.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false))
                .ToList();

            _dataContext.ClearNotifications<TNotification>();
            await Task.WhenAll(domainEventTasks).ConfigureAwait(false);
        }
    }
}
