
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

using Xpandables.Net.Api.Domains.Integrations;
using Xpandables.Net.Commands;
using Xpandables.Net.Decorators;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Api.Handlers
{
    public class ContactNameChangedFailedCommand : ICommand, IAggregatePersistenceDecorator
    {
        public ContactNameChangedFailedCommand(Guid aggregateId, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public string Name { get; }
    }

    public class ContactNameChangedIntegrationEventHandler : IntegrationEventHandler<ContactNameChangeIntegrationEvent>
    {
        public override async Task<IOperationResult<ICommand>> HandleAsync(ContactNameChangeIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return BadOperation(new ContactNameChangedFailedCommand(integrationEvent.AggregateId, integrationEvent.OldName));
        }
    }
}
