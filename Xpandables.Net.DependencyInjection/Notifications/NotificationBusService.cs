
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

using Microsoft.Extensions.Hosting;

using Xpandables.Net.DependencyInjection;

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// The default implementation of <see cref="INotificationBusService"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class NotificationBusService : BackgroundService, INotificationBusService
    {
        private readonly IServiceScopeFactory<IEventBus> _serviceScopeFactory;

        /// <summary>
        /// Constructs a new instance of <see cref="NotificationBusService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">the scope factory.</param>
        public NotificationBusService(IServiceScopeFactory<IEventBus> serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        ///<inheritdoc/>
        public bool IsRunning { get; private set; }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult> StartServiceAsync(CancellationToken cancellationToken = default)
        {
            if (IsRunning)
                return new FailureOperationResult("status", $"{nameof(NotificationBusService)} is already up.");

            IsRunning = true;
            await StartAsync(cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult();
        }

        ///<inheritdoc/>
        public virtual async Task<IOperationResult> StopServiceAsync(CancellationToken cancellationToken = default)
        {
            if (!IsRunning)
                return new FailureOperationResult("status", $"{nameof(NotificationBusService)} is already down.");

            await StopAsync(cancellationToken).ConfigureAwait(false);
            IsRunning = false;
            return new SuccessOperationResult();
        }

        ///<inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return DoPushPendingMessages(stoppingToken);
        }

        /// <summary>
        /// Returns the <see cref="TimeSpan"/> delay between each execution.
        /// The default value is 60s.
        /// </summary>
        /// <returns>A 60s timespan.</returns>
        protected virtual TimeSpan GetTimeSpanDelay()
            => TimeSpan.FromSeconds(60);

        private async Task DoPushPendingMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
                using var scope = _serviceScopeFactory.CreateScope();
                var eventBus = scope.GetRequiredService();
                await eventBus.PushAsync().ConfigureAwait(false);

                await Task.Delay(GetTimeSpanDelay(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
