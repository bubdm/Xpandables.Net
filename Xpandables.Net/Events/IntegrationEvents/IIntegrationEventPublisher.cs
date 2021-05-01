
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// Defines a method to automatically dispatch <see cref="IIntegrationEvent"/> type.
    /// </summary>
    public interface IIntegrationEventPublisher
    {
        /// <summary>
        /// Publishes integration events.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}