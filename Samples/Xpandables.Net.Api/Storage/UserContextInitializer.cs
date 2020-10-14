
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

using Xpandables.Net.Api.Storage.Services;
using Xpandables.Net.Strings;

namespace Xpandables.Net.Api.Storage
{
    public sealed class UserContextInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public UserContextInitializer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var service = _serviceScopeFactory.CreateScope();
            var stringCryptography = service.ServiceProvider.GetRequiredService<IStringCryptography>();
            var dataContext = service.ServiceProvider.GetRequiredService<UserContext>();

            if (dataContext.Users.Any()) return;

            var user = await dataContext.CreateNewUser("+33123456789", "motdepasse", "email@email.com", stringCryptography);

            await dataContext.AddEntityAsync(user, cancellationToken).ConfigureAwait(false);
            await dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
