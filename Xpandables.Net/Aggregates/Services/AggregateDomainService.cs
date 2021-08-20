
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
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates.Events;
using Xpandables.Net.Dispatchers;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Aggregates.Services
{
    /// <summary>
    /// The default implementation of <see cref="IAggregateDomainService"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class AggregateDomainService : IAggregateDomainService
    {
        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        private readonly IInstanceCreator _instanceCreator;

        private readonly IDomainEventDispatcher _dispatcher;

        private readonly IAggregateUnitOfWork _aggregateUnitOfWork;

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateDomainService"/> with the creator instance.
        /// </summary>
        /// <param name="aggregateUnitOfWork">The aggregate unit of work.</param>
        /// <param name="dispatcher">The domain event publisher</param>
        /// <param name="instanceCreator">The creator instance ot be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public AggregateDomainService(IAggregateUnitOfWork aggregateUnitOfWork, IDomainEventDispatcher dispatcher, IInstanceCreator instanceCreator)
        {
            _aggregateUnitOfWork = aggregateUnitOfWork ?? throw new ArgumentNullException(nameof(aggregateUnitOfWork));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IDomainEventSourcing aggregateEventSourcing)
                throw new ArgumentException($"{aggregate.GetType().Name} must implement {nameof(IDomainEventSourcing)}");

            var aggregateId = aggregate.AggregateId.AsString();
            var aggregateTypeName = aggregate.GetType().GetNameWithoutGenericArity();

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                await _dispatcher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);

                var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
                var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

                var eventData = @event.GetJsonDocument(SerializerOptions, DocumentOptions);
                var entity = new DomainStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

                await _aggregateUnitOfWork.Events.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
                    var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

                    var eventData = @event.GetJsonDocument(SerializerOptions, DocumentOptions);
                    var entity = new NotificationStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, default);

                    await _aggregateUnitOfWork.Notifications.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                aggregateOutbox.MarkNotificationsAsCommitted();
            }

            aggregateEventSourcing.MarkEventsAsCommitted();

            await _aggregateUnitOfWork.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot
        {
            var criteria = new StoreEntityCriteria<DomainStoreEntity>
            {
                AggregateId = aggregateId.AsString()
            };

            return await ReadAsync<TAggregate>(criteria, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync<TAggregate>(StoreEntityCriteria<DomainStoreEntity> criteria, CancellationToken cancellationToken = default)
         where TAggregate : class, IAggregateRoot
        {
            var aggregate = CreateInstance<TAggregate>();
            if (aggregate is not IDomainEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(IDomainEventSourcing)}");

            await foreach (var @event in _aggregateUnitOfWork.Events.FetchAllAsync(criteria, cancellationToken).ConfigureAwait(false))
            {
                if (@event.ToObject(SerializerOptions) is IDomainEvent domainEvent)
                    aggregateEventSourcing.LoadFromHistory(domainEvent);
            }

            return aggregate.AggregateId.IsEmpty() ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadFromSnapShot<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot
        {
            var aggregate = CreateInstance<TAggregate>();

            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must implement {typeof(IOriginator).Name}");

            var criteria = new StoreEntityCriteria<SnapShotStoreEntity>()
            {
                AggregateId = aggregateId.AsString(),
                DataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() != -1
            };

            var result = await _aggregateUnitOfWork.SnapShots
                .TryFindAsync(criteria, o => o.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64(), cancellationToken)
                .ConfigureAwait(false);

            if (result is not null)
                if (result.ToObject(SerializerOptions) is ISnapShot snapShot)
                    originator.SetMemento(snapShot.Memento);

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsSnapShotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregateRoot
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IOriginator originator)
                throw new InvalidOperationException($"{aggregate.GetType().Name} must implement '{nameof(IOriginator)}' interface");

            long version = aggregate.Version;
            var criteria = new StoreEntityCriteria<SnapShotStoreEntity>
            {
                AggregateId = aggregate.AggregateId.AsString(),
                DataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() == version
            };

            await _aggregateUnitOfWork.SnapShots
                .DeleteAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

            var snapShot = new SnapShot(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);

            var aggregateId = aggregate.AggregateId.AsString();
            var aggregateTypeName = aggregate.GetType().GetNameWithoutGenericArity();
            var eventTypeFullName = snapShot.GetType().AssemblyQualifiedName!;
            var eventTypeName = snapShot.GetType().GetNameWithoutGenericArity();

            var eventData = snapShot.GetJsonDocument(SerializerOptions, DocumentOptions);
            var entity = new SnapShotStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

            await _aggregateUnitOfWork.SnapShots.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
            await _aggregateUnitOfWork.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TStoreEntity> ReadEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            return (criteria.Size, criteria.Index) switch
            {
                (null, null) => _aggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, cancellationToken),
                (null, { }) => _aggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, criteria.Index.Value, 50, cancellationToken),
                ({ }, null) => _aggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, 1, criteria.Size.Value, cancellationToken),
                (_, _) => _aggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, criteria.Index.Value, criteria.Size.Value, cancellationToken)
            };
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity
            => await _aggregateUnitOfWork.GetRepository<TStoreEntity>().CountAsync(criteria, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates a new instance of <typeparamref name="TAggregate"/>.
        /// </summary>
        /// <typeparam name="TAggregate">the type of the aggregate.</typeparam>
        /// <returns>An instance of <typeparamref name="TAggregate"/> type or throws exception.</returns>
        protected virtual TAggregate CreateInstance<TAggregate>()
            where TAggregate : class, IAggregateRoot
        {
            var instanceCreatorException = default(ExceptionDispatchInfo?);
            _instanceCreator.OnException = ex => instanceCreatorException = ex;

            if (_instanceCreator.Create(typeof(TAggregate)) is not TAggregate aggregate)
            {
                if (instanceCreatorException is not null)
                    instanceCreatorException.Throw();

                throw new InvalidOperationException($"{typeof(TAggregate).Name} Unable to create a new instance.");
            }

            return aggregate;
        }
    }
}
