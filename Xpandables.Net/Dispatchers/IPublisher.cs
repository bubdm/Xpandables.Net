
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
using Xpandables.Net.Aggregates.Events;

namespace Xpandables.Net.Dispatchers;

/// <summary>
/// Defines a set of methods to automatically publish <see cref="IEvent"/> when targeting <see cref="IDomainEventHandler{TDomainEvent}"/> 
/// or <see cref="INotificationHandler{TNotificationEvent}"/>.
/// The implementation must be thread-safe when working in a multi-threaded environment.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Asynchronously publishes the specified event to all registered suscribers.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
    /// <returns>A task that represents an asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Publishing the event failed. See inner exception.</exception>
    Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
}
