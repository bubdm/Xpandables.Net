
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

using Xpandables.Net.Aggregates;
using Xpandables.Net.Commands;
using Xpandables.Net.Database;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.Decorators.Persistences
{
    /// <summary>
    /// This class allows the application author to add persistence support to integration event control flow.
    /// The target event should implement the <see cref="IPersistenceDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target integration event handler with an implementation of <see cref="IDataContext"/> and executes the
    /// the <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)"/> if available after the main one in the same control flow only
    /// </summary>
    /// <typeparam name="TAggregateId">The type the aggregate identity.</typeparam>
    /// <typeparam name="TNotification">Type of integration event.</typeparam>
    public sealed class NotificationPersistenceDecorator<TAggregateId, TNotification> : PersistenceDecoratorBase, INotificationHandler<TAggregateId, TNotification>
        where TNotification : class, INotification<TAggregateId>, IPersistenceDecorator
        where TAggregateId : notnull, IAggregateId
    {
        private readonly INotificationHandler<TAggregateId, TNotification> _decoratee;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPersistenceDecorator{TAggregateId, TNotification}"/> class with
        /// the decorated handler and the db context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <param name="decoratee">The decorated integration event handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public NotificationPersistenceDecorator(IDataContext dataContext, INotificationHandler<TAggregateId, TNotification> decoratee)
            : base(dataContext)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        ///<inheritdoc/>
        public async Task<IOperationResult<ICommand?>> HandleAsync(TNotification integrationEvent, CancellationToken cancellationToken = default)
            => await HandleAsync(_decoratee.HandleAsync(integrationEvent, cancellationToken), cancellationToken).ConfigureAwait(false);
    }
}
