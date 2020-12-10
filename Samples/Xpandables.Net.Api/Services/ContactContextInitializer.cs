
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

using Xpandables.Net.Api.Database;
using Xpandables.Net.Api.Models;

namespace Xpandables.Net.Api.Services
{
    public sealed class ContactContextInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ContactContextInitializer(IServiceScopeFactory serviceScopeFactory) 
            => _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var service = _serviceScopeFactory.CreateScope();
            var dataContext = service.ServiceProvider.GetRequiredService<ContactContext>();

            if (dataContext.Contacts.Any()) return;

            var contact = new ContactModel("myName", "Paris", "Alexandre LeGrand 01", "France");

            await dataContext.AddEntityAsync(contact, cancellationToken).ConfigureAwait(false);
            await dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask.ConfigureAwait(false);
    }
}
