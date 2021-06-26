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

using Xpandables.Net.NotificationEvents;
using Xpandables.Net.Notifications;
using Xpandables.Net.Services;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="INotificationBusService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TNotificationBusService">The notification bus service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationBusService<TNotificationBusService>(this IXpandableServiceBuilder services)
            where TNotificationBusService : class, IHostedService, INotificationBusService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<INotificationBusService, TNotificationBusService>();
            services.Services.AddHostedService(provider => provider.GetRequiredService<INotificationBusService>() as TNotificationBusService);
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="INotificationBusService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationBusService(this IXpandableServiceBuilder services)
            => services.AddXNotificationBusService<NotificationBusService>();

        /// <summary>
        /// Adds the <see cref="INotificationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TNotificationPublisher">The notification publisher type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationPublisher<TNotificationPublisher>(this IXpandableServiceBuilder services)
            where TNotificationPublisher : class, INotificationEventPublisher
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<INotificationEventPublisher, TNotificationPublisher>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="INotificationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationPublisher(this IXpandableServiceBuilder services)
            => services.AddXNotificationPublisher<NotificationEventPublisher>();
    }
}
