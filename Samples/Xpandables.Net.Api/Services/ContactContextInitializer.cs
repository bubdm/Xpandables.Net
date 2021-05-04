
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xpandables.Net.Api.Domains;
using Xpandables.Net.Database;
using Xpandables.Net.Entities;
using Xpandables.Net.EntityFramework;

namespace Xpandables.Net.Api.Services
{
    public sealed class ContactContextInitializer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ContactContextInitializer(IServiceScopeFactory serviceScopeFactory)
            => _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return DoInitializeContextAsync(stoppingToken);
        }

        private async Task DoInitializeContextAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            using var service = _serviceScopeFactory.CreateScope();
            using var context = service.ServiceProvider.GetRequiredService<IEventStoreDataContext>();
            var aggregateAccessor = service.ServiceProvider.GetRequiredService<IAggregateAccessor<ContactAggregate>>();

            if (context.Set<DomainEventEntity>().Any())
                return;

            var contact = ContactAggregate.CreateNewContact("myName", "Paris", "Alexandre LeGrand 01", "France");
            ContactAggregate.FirstGuidCreated = contact.Guid.ToString();

            await aggregateAccessor.AppendAsync(contact, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
