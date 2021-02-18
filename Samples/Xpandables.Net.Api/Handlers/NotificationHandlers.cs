
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactModelCreatedDomainEventHandler : IDomainEventHandler<ContactModelCreatedDomainEvent>
    {
        public Task HandleAsync(ContactModelCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Trace.WriteLine($"A newly created contact with id : {domainEvent.Id} : {DateTime.Now.Ticks}");
            return Task.CompletedTask;
        }
    }

    public sealed class ContactModelCreatedIntegrationEventHandler : IIntegrationEventHandler<ContactModelCreatedIntegrationEvent>
    {
        public Task HandleAsync(ContactModelCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            Trace.WriteLine($"A newly contact has been created with id : {integrationEvent.Id} {DateTime.Now.Ticks}");
            return Task.CompletedTask;
        }
    }

    public sealed class ContactModelUpdatedDomainEventHandler : IDomainEventHandler<ContactModelUpdatedDomainEvent>
    {
        public Task HandleAsync(ContactModelUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Trace.WriteLine($"The contact with id '{domainEvent.Id}' has changed name to : {domainEvent.Name} {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
