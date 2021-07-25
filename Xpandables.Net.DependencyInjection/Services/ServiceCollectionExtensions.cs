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

using Xpandables.Net.Services;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <typeparamref name="TBackgroundService"/> type implementation to the services with singleton life time.
        /// </summary>
        /// <typeparam name="TBackgroundService">The background service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXBackgroundService<TBackgroundService>(this IXpandableServiceBuilder services)
            where TBackgroundService : class, IHostedService, IBackgroundService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddHostedService<TBackgroundService>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TBackgroundServiceImplementation"/> as <typeparamref name="TBackgroundServiceInterface"/> type implementation to the services with singleton life time.
        /// </summary>
        /// <typeparam name="TBackgroundServiceInterface">The background service type interface.</typeparam>
        /// <typeparam name="TBackgroundServiceImplementation">The background service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXBackgroundService<TBackgroundServiceInterface, TBackgroundServiceImplementation>(this IXpandableServiceBuilder services)
            where TBackgroundServiceInterface : class, IBackgroundService
            where TBackgroundServiceImplementation : BackgroundServiceBase<TBackgroundServiceImplementation>, TBackgroundServiceInterface
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<TBackgroundServiceInterface, TBackgroundServiceImplementation>();
            services.Services.AddHostedService(provider => provider.GetRequiredService<TBackgroundServiceInterface>() as TBackgroundServiceImplementation);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TApplicationService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TApplicationService">The application service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXApplicationService<TApplicationService>(this IXpandableServiceBuilder services)
            where TApplicationService : ApplicationService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<TApplicationService>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TApplicationServiceImplementation"/> as <typeparamref name="TApplicationServiceInterface"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TApplicationServiceInterface">The application service type interface.</typeparam>
        /// <typeparam name="TApplicationServiceImplementation">The application service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXApplicationService<TApplicationServiceInterface, TApplicationServiceImplementation>(this IXpandableServiceBuilder services)
            where TApplicationServiceInterface : class, IApplicationService
            where TApplicationServiceImplementation : ApplicationService, TApplicationServiceInterface
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<TApplicationServiceInterface, TApplicationServiceImplementation>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDomainService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TDomainService">The domain service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainService<TDomainService>(this IXpandableServiceBuilder services)
            where TDomainService : DomainService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<TDomainService>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDomainServiceImplementation"/> as <typeparamref name="TDomainServiceInterface"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TDomainServiceInterface">The domain service type interface.</typeparam>
        /// <typeparam name="TDomainServiceImplementation">The domain service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainService<TDomainServiceInterface, TDomainServiceImplementation>(this IXpandableServiceBuilder services)
            where TDomainServiceInterface : class, IDomainService
            where TDomainServiceImplementation : DomainService, TDomainServiceInterface
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<TDomainServiceInterface, TDomainServiceImplementation>();
            return services;
        }
    }
}
