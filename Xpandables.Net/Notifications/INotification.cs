
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
using System.Text.Json.Serialization;

using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as a notification.
    /// </summary>
    public interface INotification : IEvent { }

    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as notification holding a specific <see cref="IDomainEvent"/>.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    public interface INotification<out TDomainEvent> : INotification
        where TDomainEvent : notnull, IDomainEvent
    {
        /// <summary>
        /// Gets the target domain event.
        /// </summary>
        [JsonIgnore]
        TDomainEvent DomainEvent { get; }
    }
}
