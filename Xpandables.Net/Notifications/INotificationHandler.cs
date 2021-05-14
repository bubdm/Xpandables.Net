
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

using Xpandables.Net.Commands;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Allows an application author to define a handler for notification.
    /// The event must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface INotificationHandler : ICanHandle
    {
        /// <summary>
        ///  Asynchronously handle the notification.
        ///  Returns an optional command to be processed.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="notification"/> does not implement <see cref="INotification"/>.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TCommand}"/>.</returns>
        Task<IOperationResult<ICommand?>> HandleAsync(object notification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type notification.
    /// The notification must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to be handled.</typeparam>
    public interface INotificationHandler<in TNotification> : INotificationHandler, ICanHandle<TNotification>
        where TNotification : class, INotification
    {
        /// <summary>
        /// Asynchronously handles the notification of specific type.
        ///  Returns an optional command to be processed.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult{TCommand}"/>.</returns>
        Task<IOperationResult<ICommand?>> HandleAsync(TNotification notification, CancellationToken cancellationToken = default);

        Task<IOperationResult<ICommand?>> INotificationHandler.HandleAsync(object notification, CancellationToken cancellationToken)
        {
            if (notification is TNotification instance)
                return HandleAsync(instance, cancellationToken);

            throw new ArgumentException($"The parameter does not implement {nameof(INotification)} interface.", nameof(notification));
        }
    }
}
