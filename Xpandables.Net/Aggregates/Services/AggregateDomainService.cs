
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

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        protected IInstanceCreator InstanceCreator { get; }

        /// <summary>
        /// Gets the domain event publisher instance.
        /// </summary>
        protected IDomainEventPublisher DomainEventPublisher { get; }

        /// <summary>
        /// Gets the instance of aggregate unit of work.
        /// </summary>
        protected IAggregateUnitOfWork AggregateUnitOfWork { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateDomainService"/> with the creator instance.
        /// </summary>
        /// <param name="aggregateUnitOfWork">The aggregate unit of work.</param>
        /// <param name="domainEventPublisher">The domain event publisher</param>
        /// <param name="instanceCreator">The creator instance ot be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        public AggregateDomainService(IAggregateUnitOfWork aggregateUnitOfWork, IDomainEventPublisher domainEventPublisher, IInstanceCreator instanceCreator)
        {
            AggregateUnitOfWork = aggregateUnitOfWork ?? throw new ArgumentNullException(nameof(aggregateUnitOfWork));
            InstanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            DomainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IDomainEventSourcing aggregateEventSourcing)
                throw new ArgumentException($"{aggregate.GetType().Name} must implement {nameof(IDomainEventSourcing)}");

            var aggregateId = aggregate.AggregateId.AsString();
            var aggregateTypeName = aggregate.GetType().GetNameWithoutGenericArity();

            foreach (var @event in aggregateEventSourcing.GetUncommittedEvents())
            {
                var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
                var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

                var eventData = @event.GetJsonDocument(SerializerOptions, DocumentOptions);
                var entity = new DomainStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

                await AggregateUnitOfWork.Events.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
                await DomainEventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
                    var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

                    var eventData = @event.GetJsonDocument(SerializerOptions, DocumentOptions);
                    var entity = new NotificationStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, default);

                    await AggregateUnitOfWork.Notifications.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                aggregateOutbox.MarkNotificationsAsCommitted();
            }

            aggregateEventSourcing.MarkEventsAsCommitted();

            await AggregateUnitOfWork.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate
        {
            var criteria = new StoreEntityCriteria<DomainStoreEntity>
            {
                AggregateId = aggregateId.AsString()
            };

            return await ReadAsync<TAggregate>(criteria, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync<TAggregate>(StoreEntityCriteria<DomainStoreEntity> criteria, CancellationToken cancellationToken = default)
         where TAggregate : class, IAggregate
        {
            var aggregate = CreateInstance<TAggregate>();
            if (aggregate is not IDomainEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(IDomainEventSourcing)}");

            await foreach (var @event in AggregateUnitOfWork.Events.FetchAllAsync(criteria, cancellationToken).ConfigureAwait(false))
            {
                if (@event.ToObject(SerializerOptions) is IDomainEvent domainEvent)
                    aggregateEventSourcing.LoadFromHistory(domainEvent);
            }

            return aggregate.AggregateId.IsEmpty() ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadFromSnapShot<TAggregate>(IAggregateId aggregateId, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate
        {
            var aggregate = CreateInstance<TAggregate>();

            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must implement {typeof(IOriginator).Name}");

            var criteria = new StoreEntityCriteria<SnapShotStoreEntity>()
            {
                AggregateId = aggregateId.AsString(),
                DataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() != -1
            };

            var result = await AggregateUnitOfWork.SnapShots
                .TryFindAsync(criteria, o => o.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64(), cancellationToken)
                .ConfigureAwait(false);

            if (result is not null)
                if (result.ToObject(SerializerOptions) is ISnapShot snapShot)
                    originator.SetMemento(snapShot.Memento);

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsSnapShotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate
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

            await AggregateUnitOfWork.SnapShots
                .DeleteAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

            var snapShot = new SnapShot(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);

            var aggregateId = aggregate.AggregateId.AsString();
            var aggregateTypeName = aggregate.GetType().GetNameWithoutGenericArity();
            var eventTypeFullName = snapShot.GetType().AssemblyQualifiedName!;
            var eventTypeName = snapShot.GetType().GetNameWithoutGenericArity();

            var eventData = snapShot.GetJsonDocument(SerializerOptions, DocumentOptions);
            var entity = new SnapShotStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

            await AggregateUnitOfWork.SnapShots.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
            await AggregateUnitOfWork.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TStoreEntity> ReadEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            return (criteria.Size, criteria.Index) switch
            {
                (null, null) => AggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, cancellationToken),
                (null, { }) => AggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, criteria.Index.Value, 50, cancellationToken),
                ({ }, null) => AggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, 1, criteria.Size.Value, cancellationToken),
                (_, _) => AggregateUnitOfWork.GetRepository<TStoreEntity>().FetchAllAsync(criteria, o => o.CreatedOn, criteria.Index.Value, criteria.Size.Value, cancellationToken)
            };
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountEventsAsync<TStoreEntity>(StoreEntityCriteria<TStoreEntity> criteria, CancellationToken cancellationToken = default)
            where TStoreEntity : StoreEntity
            => await AggregateUnitOfWork.GetRepository<TStoreEntity>().CountAsync(criteria, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates a new instance of <typeparamref name="TAggregate"/>.
        /// </summary>
        /// <typeparam name="TAggregate">the type of the aggregate.</typeparam>
        /// <returns>An instance of <typeparamref name="TAggregate"/> type or throws exception.</returns>
        protected virtual TAggregate CreateInstance<TAggregate>()
            where TAggregate : class, IAggregate
        {
            var instanceCreatorException = default(ExceptionDispatchInfo?);
            InstanceCreator.OnException = ex => instanceCreatorException = ex;

            if (InstanceCreator.Create(typeof(TAggregate)) is not TAggregate aggregate)
            {
                if (instanceCreatorException is not null)
                    instanceCreatorException.Throw();

                throw new InvalidOperationException($"{typeof(TAggregate).Name} Unable to create a new instance.");
            }

            return aggregate;
        }
    }
}
