
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Allows an application author to define a handler for a domain event notification.
    /// The event must implement <see cref="IDomainEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDomainEventNotificationHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handle the domain event notification.
        /// </summary>
        /// <param name="domainEventNotification">The domain event notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventNotification"/> is null.</exception>
        Task HandleAsync(object domainEventNotification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type domain event notification.
    /// The event must implement <see cref="IDomainEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TDomainEventNotification">The domain event notification type to be handled.</typeparam>
    public interface IDomainEventNotificationHandler<in TDomainEventNotification> : IDomainEventNotificationHandler, ICanHandle<TDomainEventNotification>
        where TDomainEventNotification : class, IDomainEvent
    {
        /// <summary>
        /// Asynchronously handles the domain event notification.
        /// </summary>
        /// <param name="domainEventNotification">The domain event notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="domainEventNotification"/> is null.</exception>
        Task HandleAsync(TDomainEventNotification domainEventNotification, CancellationToken cancellationToken = default);

        Task IDomainEventNotificationHandler.HandleAsync(object domainEventNotification, CancellationToken cancellationToken)
        {
            if (domainEventNotification is TDomainEventNotification notif)
                return HandleAsync(notif, cancellationToken);

            throw new ArgumentNullException(nameof(domainEventNotification));
        }
    }
}
