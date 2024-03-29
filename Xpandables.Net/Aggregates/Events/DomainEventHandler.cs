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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Aggregates.Events
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IDomainEventHandler{TDomainEvent}"/> interface.
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event to act on.</typeparam>
    public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : class, IDomainEvent
    {
        ///<inheritdoc/>
        public abstract Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken = default);

        Task IEventHandler.HandleAsync(object @event, CancellationToken cancellationToken)
            => HandleAsync((TDomainEvent)@event, cancellationToken);
    }
}