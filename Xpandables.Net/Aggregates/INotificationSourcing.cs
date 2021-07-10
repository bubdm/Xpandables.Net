
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
using System.Linq;

using Xpandables.Net.NotificationEvents;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Out-box pattern interface (notification).
    /// </summary>
    internal interface INotificationSourcing
    {
        /// <summary>
        /// Marks all notifications as committed.
        /// </summary>
        void MarkNotificationsAsCommitted();

        /// <summary>
        /// Returns a collection of notifications.
        /// </summary>
        /// <returns>A list of notifications.</returns>
        IOrderedEnumerable<INotificationEvent> GetNotifications();
    }

    /// <summary>
    /// Out-box pattern interface (notification).
    /// </summary>
    /// /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    internal interface INotificationSourcing<TAggregateId> : INotificationSourcing
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Returns a collection of notifications.
        /// </summary>
        /// <returns>A list of notifications.</returns>
        new IOrderedEnumerable<INotificationEvent<TAggregateId>> GetNotifications();

        IOrderedEnumerable<INotificationEvent> INotificationSourcing.GetNotifications()
            => GetNotifications();
    }
}
