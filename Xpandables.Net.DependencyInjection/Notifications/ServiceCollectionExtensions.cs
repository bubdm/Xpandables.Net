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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

using Xpandables.Net.Aggregates.Notifications;
using Xpandables.Net.Services;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="INotificationService"/> as <see cref="INotificationService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TNotificationService">The notification event service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationService<TNotificationService>(this IXpandableServiceBuilder services)
            where TNotificationService : class, IHostedService, INotificationService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<INotificationService, TNotificationService>();
            services.Services.AddHostedService(provider => provider.GetRequiredService<INotificationService>() as TNotificationService);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TNotificationEventPublisher"/> as <see cref="INotificationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TNotificationEventPublisher">The notification publisher type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationEventPublisher<TNotificationEventPublisher>(this IXpandableServiceBuilder services)
            where TNotificationEventPublisher : class, INotificationEventPublisher
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<INotificationEventPublisher, TNotificationEventPublisher>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="INotificationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationEventPublisher(this IXpandableServiceBuilder services)
            => services.AddXNotificationEventPublisher<NotificationEventPublisher>();
    }
}
