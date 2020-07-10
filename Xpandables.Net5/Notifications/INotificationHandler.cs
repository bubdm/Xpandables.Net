
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
    /// Defines a marker interface to be used to mark an object to act as a event notification.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface INotification { }
#pragma warning restore CA1040 // Avoid empty interfaces

    /// <summary>
    /// Allows an application author to define a handler for an notification.
    /// The event must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface INotificationHandler
    {
        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        Task HandleAsync( object notification);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type notification.
    /// The event must implement <see cref="INotification"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to be handled.</typeparam>
    public interface INotificationHandler<in TNotification> : INotificationHandler
        where TNotification : class, INotification
    {
        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        Task HandleAsync( TNotification notification);
    }
}