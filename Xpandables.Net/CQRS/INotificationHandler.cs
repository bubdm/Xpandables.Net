
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
    /// Allows an application author to define a handler for an notification.
    /// The event must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface INotificationHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handle the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        Task HandleAsync(object notification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type notification.
    /// The event must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to be handled.</typeparam>
    public interface INotificationHandler<in TNotification> : INotificationHandler, ICanHandle<TNotification>
        where TNotification : class, INotification
    {
        /// <summary>
        /// Asynchronously handles the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);

        Task INotificationHandler.HandleAsync(object notification, CancellationToken cancellationToken)
        {
            if (notification is TNotification notif)
                return HandleAsync(notif, cancellationToken);

            throw new ArgumentNullException(nameof(notification));
        }
    }
}
