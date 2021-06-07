
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

using Xpandables.Net.Aggregates;
using Xpandables.Net.Commands;
using Xpandables.Net.DomainEvents;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="INotificationHandler{TAggregateId, TNotification}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TNotification">Type of notification to act on.</typeparam>
    public abstract class NotificationHandler<TAggregateId, TNotification> : OperationResults, INotificationHandler<TAggregateId, TNotification>
        where TNotification : class, INotification<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        ///<inheritdoc/>
        public abstract Task<IOperationResult<ICommand?>> HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="INotificationHandler{TAggregateId, TDomainEvent, TNotification}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TDomainEvent">The type of target domain event.</typeparam>
    /// <typeparam name="TNotification">Type of notification to act on.</typeparam>
    public abstract class NotificationHandler<TAggregateId, TDomainEvent, TNotification> :
        OperationResults, INotificationHandler<TAggregateId, TDomainEvent, TNotification>
        where TNotification : class, INotification<TAggregateId, TDomainEvent>
        where TDomainEvent : class, IDomainEvent<TAggregateId>
        where TAggregateId : notnull, AggregateId
    {
        ///<inheritdoc/>
        public abstract Task<IOperationResult<ICommand?>> HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
    }
}