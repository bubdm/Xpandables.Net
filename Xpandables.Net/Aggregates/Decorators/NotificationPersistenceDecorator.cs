
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

using Xpandables.Net.Aggregates.Notifications;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.Aggregates.Decorators
{
    /// <summary>
    /// This class allows the application author to add persistence support to integration event control flow with aggregate.
    /// The target event should implement the <see cref="IPersistenceDecorator"/> 
    /// interface in order to activate the behavior.
    /// The class decorates the target integration event handler with an implementation of <see cref="IUnitOfWork"/> and executes the
    /// the <see cref="IUnitOfWork.PersistAsync(CancellationToken)"/> if available after the main one in the same control flow only
    /// </summary>
    /// <typeparam name="TNotificationEvent">Type of integration event.</typeparam>
    public sealed class NotificationPersistenceDecorator<TNotificationEvent> : INotificationEventHandler<TNotificationEvent>
        where TNotificationEvent : class, INotificationEvent, IPersistenceDecorator
    {
        private readonly INotificationEventHandler<TNotificationEvent> _decoratee;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPersistenceDecorator{TNotification}"/> class with
        /// the decorated handler and the unit of work to act on.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to act on.</param>
        /// <param name="decoratee">The decorated integration event handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="unitOfWork"/> is null.</exception>
        public NotificationPersistenceDecorator(INotificationEventHandler<TNotificationEvent> decoratee, IUnitOfWork unitOfWork)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        ///<inheritdoc/>
        public async Task HandleAsync(TNotificationEvent @event, CancellationToken cancellationToken = default)
        {
            await _decoratee.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.PersistAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
