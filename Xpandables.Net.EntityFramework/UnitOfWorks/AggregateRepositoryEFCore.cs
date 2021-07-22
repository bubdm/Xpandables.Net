
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
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Entities;
using Xpandables.Net.Events;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IAggregateRepository{TAggregate}"/>.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    public class AggregateRepositoryEFCore<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : class, IAggregate
    {
        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        protected static readonly IInstanceCreator InstanceCreator = new InstanceCreator();

        /// <summary>
        /// Gets the current context instance.
        /// </summary>
        protected virtual ContextEFCore Context { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateRepositoryEFCore{Taggregate}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public AggregateRepositoryEFCore(ContextEFCore context) => Context = context ?? throw new ArgumentNullException(nameof(context));

        ///<inheritdoc/>
        public virtual async Task AppendAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
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

                var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
                var entity = new DomainEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

                await Context.Set<DomainEventStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            if (aggregate is INotificationSourcing aggregateOutbox)
            {
                foreach (var @event in aggregateOutbox.GetNotifications())
                {
                    var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
                    var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

                    var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
                    var entity = new NotificationEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, default);

                    await Context.Set<NotificationEventStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                aggregateOutbox.MarkNotificationsAsCommitted();
            }

            aggregateEventSourcing.MarkEventsAsCommitted();
        }

        ///<inheritdoc/>
        public virtual async Task AppendNotificationAsync(INotificationEvent @event, CancellationToken cancellationToken = default)
        {
            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = typeof(TAggregate).GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
            var entity = new NotificationEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

            await Context.Set<NotificationEventStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountEventsAsync<TEventStoreEntity>(
            EventStoreEntityCriteria<TEventStoreEntity> criteria,
            CancellationToken cancellationToken = default)
            where TEventStoreEntity : EventStoreEntity
            => await Context.Set<TEventStoreEntity>()
                .CountAsync(criteria, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadAsync(IAggregateId aggregateId, CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();
            if (aggregate is not IDomainEventSourcing aggregateEventSourcing)
                throw new InvalidOperationException($"{typeof(TAggregate).Name} must implement {nameof(IDomainEventSourcing)}");

            var criteria = new EventStoreEntityCriteria<DomainEventStoreEntity>
            {
                AggregateId = aggregateId.AsString()
            };

            await foreach (var @event in Context.Set<DomainEventStoreEntity>().Where(criteria).AsNoTracking().AsAsyncEnumerable().ConfigureAwait(false))
            {
                if (@event.ToObject(SerializerOptions) is IDomainEvent domainEvent)
                    aggregateEventSourcing.LoadFromHistory(domainEvent);
            }

            return aggregate.AggregateId.IsEmpty() ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TEventStoreEntity> ReadEventsAsync<TEventStoreEntity>(
            EventStoreEntityCriteria<TEventStoreEntity> criteria,
            CancellationToken cancellationToken = default) where TEventStoreEntity : EventStoreEntity
        {
            _ = criteria ?? throw new ArgumentNullException(nameof(criteria));

            IQueryable<TEventStoreEntity> selector = (criteria.Size, criteria.Index) switch
            {
                (null, null) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn),
                (null, { }) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value),
                ({ }, null) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Take(criteria.Size.Value),
                (_, _) => Context.Set<TEventStoreEntity>().Where(criteria).OrderBy(o => o.CreatedOn).Skip(criteria.Index.Value * criteria.Size.Value).Take(criteria.Size.Value)
            };

            return selector.AsNoTracking().AsAsyncEnumerable();
        }

        ///<inheritdoc/>
        public virtual async Task<TAggregate?> ReadFromSnapShot(IAggregateId aggregateId, CancellationToken cancellationToken = default)
        {
            var aggregate = CreateInstance();

            if (aggregate is not IOriginator originator)
                throw new ArgumentException($"{typeof(TAggregate).Name} must implement {typeof(IOriginator).Name}");

            var criteria = new EventStoreEntityCriteria<SnapShotStoreEntity>()
            {
                AggregateId = aggregateId.AsString(),
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() != -1
            };

            var result = await Context.Set<SnapShotStoreEntity>()
                .AsNoTracking()
                .Where(criteria)
                .OrderBy(o => o.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64())
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (result is not null)
                if (result.ToObject(SerializerOptions) is ISnapShot snapShot)
                    originator.SetMemento(snapShot.Memento);

            return aggregate.IsEmpty ? default : aggregate;
        }

        ///<inheritdoc/>
        public virtual async Task AppendAsSnapShotAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            if (aggregate is not IOriginator originator)
                throw new InvalidOperationException($"{aggregate.GetType().Name} must implement '{nameof(IOriginator)}' interface");

            long version = aggregate.Version;
            var criteria = new EventStoreEntityCriteria<SnapShotStoreEntity>
            {
                AggregateId = aggregate.AggregateId.AsString(),
                EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() == version
            };

            var oldSnapShot = await Context.Set<SnapShotStoreEntity>()
                .Where(criteria)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (oldSnapShot is not null)
            {
                oldSnapShot.Deleted();
                Context.Set<SnapShotStoreEntity>().Remove(oldSnapShot);
            }

            var snapShot = new SnapShot(originator.CreateMemento(), aggregate.AggregateId, aggregate.Version);

            var aggregateId = aggregate.AggregateId.AsString();
            var aggregateTypeName = aggregate.GetType().GetNameWithoutGenericArity();
            var eventTypeFullName = snapShot.GetType().AssemblyQualifiedName!;
            var eventTypeName = snapShot.GetType().GetNameWithoutGenericArity();

            var eventData = GetJsonDocument(snapShot, SerializerOptions, DocumentOptions);
            var entity = new SnapShotStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);

            await Context.Set<SnapShotStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TAggregate"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TAggregate"/> type or throws exception.</returns>
        protected virtual TAggregate CreateInstance()
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

        /// <summary>
        /// Returns the <see cref="JsonDocument"/> from the specified document.
        /// </summary>
        /// <typeparam name="TDocument">The type of document.</typeparam>
        /// <param name="document">An instance of document to parse.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <returns>An instance of <see cref="JsonDocument"/>.</returns>
        protected virtual JsonDocument GetJsonDocument<TDocument>(
            TDocument document,
            JsonSerializerOptions? serializerOptions,
            JsonDocumentOptions documentOptions)
            where TDocument : notnull
        {
            var eventString = JsonSerializer.Serialize(document, document.GetType(), serializerOptions);
            return JsonDocument.Parse(eventString, documentOptions);
        }
    }
}
