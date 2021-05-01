
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xpandables.Net.Database;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The default implementation of <see cref="IIntegrationEventService"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class IntegrationEventService : BackgroundService, IIntegrationEventService
    {
        private readonly Timer _timer;
        private static readonly object _locker = new();

        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Constructs a new instance of <see cref="IntegrationEventService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">the scope factory.</param>
        public IntegrationEventService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _timer = new Timer(
                PushMessages,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(30));
        }

        ///<inheritdoc/>
        public bool IsRunning { get; private set; }

        ///<inheritdoc/>
        public async Task<IOperationResult> StartServiceAsync(CancellationToken cancellationToken = default)
        {
            if (IsRunning)
                return new FailureOperationResult("status", $"{nameof(IntegrationEventService)} is already up.");

            IsRunning = true;
            _timer.Change(30, 0);
            await StartAsync(cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult();
        }

        ///<inheritdoc/>
        public async Task<IOperationResult> StopServiceAsync(CancellationToken cancellationToken = default)
        {
            if (!IsRunning)
                return new FailureOperationResult("status", $"{nameof(IntegrationEventService)} is already down.");

            await StopAsync(cancellationToken).ConfigureAwait(false);
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));
            IsRunning = false;
            return new SuccessOperationResult();
        }

        ///<inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

        private async void PushMessages(object? state)
        {
            //var hasLock = false;

            //try
            //{
            //    Monitor.TryEnter(_locker, ref hasLock);

            //    if (!hasLock)
            //    {
            //        return;
            //    }

                using var scope = _serviceScopeFactory.CreateScope();
                var tenant = scope.ServiceProvider.GetRequiredService<IDataContextTenantAccessor>();
                tenant.SetTenantName("ContactContext");
                var integrationEventProcessor = scope.ServiceProvider.GetRequiredService<IIntegrationEventProcessor>();
                await integrationEventProcessor.PushPendingMessages().ConfigureAwait(false);

            //}
            //finally
            //{
            //    if (hasLock)
            //    {
            //        Monitor.Exit(_locker);
            //    }
            //}
        }
    }
}
