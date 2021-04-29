﻿
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAggregateRoot"></typeparam>
    public class AggregateRootAccessor<TAggregateRoot> : OperationResultBase, IAggregateRootAccessor<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        private readonly IEventStore _eventStore;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IInstanceCreator _instanceCreator;

        private Exception? instanceCreatorException;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventStore"></param>
        /// <param name="eventPublisher"></param>
        /// <param name="instanceCreator"></param>
        public AggregateRootAccessor(IEventStore eventStore, IDomainEventPublisher eventPublisher, IInstanceCreator instanceCreator)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _instanceCreator.OnException = dispatchException => instanceCreatorException = dispatchException.SourceException;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IOperationResult> AppendAsync(TAggregateRoot aggregate, CancellationToken cancellationToken = default)
        {
            _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            foreach (var @event in aggregate.GetUncommittedEvents())
            {
                var appendresult = await _eventStore.AppendEventAsync(@event, cancellationToken).ConfigureAwait(false);
                if (appendresult.Failed)
                    return appendresult;

                await _eventPublisher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            aggregate.MarkEventsAsCommitted();

            return OkOperation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IOperationResult<TAggregateRoot>> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            var events = new List<IDomainEvent>();
            await foreach (var @event in _eventStore.ReadEventsAsync(aggregateId, cancellationToken))
                events.Add(@event);

            if (events.Count <= 0)
                return NotFoundOperation<TAggregateRoot>();

            if (_instanceCreator.Create<IEnumerable<IDomainEvent>>(typeof(TAggregateRoot), events) is not TAggregateRoot aggregateRoot)
            {
                if (instanceCreatorException is not null)
                    return BadOperation<TAggregateRoot>(new OperationError(typeof(TAggregateRoot).Name, instanceCreatorException));
                return
                    BadOperation<TAggregateRoot>(typeof(TAggregateRoot).Name, "Unable to create a new instance.");
            }

            return OkOperation(aggregateRoot);
        }
    }
}
