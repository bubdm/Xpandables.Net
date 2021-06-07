
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Defines a method to automatically publish <see cref="INotification{TAggregateId}"/> type.
    /// </summary>
    public interface INotificationPublisher
    {
        /// <summary>
        /// Publishes notifications.
        /// </summary>
        /// <param name="event">The notification to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="event"/> must implement 
        /// <see cref="INotification{TAggregateId}"/> or <see cref="INotification{TAggregateId, TDomainEvent}"/> interface.</exception>
        Task PublishAsync(ICommandQueryEvent @event, CancellationToken cancellationToken = default);
    }
}