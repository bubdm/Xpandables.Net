
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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Xpandables.Net5.Notifications
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="INotificationHandler{TNotification}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification.</typeparam>
    public sealed class NotificationHandlerBuilder<TNotification> : INotificationHandler<TNotification>
        where TNotification : class, INotification
    {
        private readonly Func<TNotification, Task> _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="NotificationHandlerBuilder{TNotification}"/> class with the delegate to be used as
        /// <see cref="INotificationHandler{TNotification}"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler method will be invoked.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public NotificationHandlerBuilder(Func<TNotification, Task> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        public async Task HandleAsync([NotNull] TNotification notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));
            await _handler(notification).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        public async Task HandleAsync([NotNull] object notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));
            if (notification is TNotification instance)
                await _handler(instance).ConfigureAwait(false);
        }
    }
}
