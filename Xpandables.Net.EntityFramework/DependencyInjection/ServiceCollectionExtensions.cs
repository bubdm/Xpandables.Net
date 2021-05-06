
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Database;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.EntityFramework.EventStoreNewtonsoft;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register EFCore objects.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IEventStore"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXEventStore(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddScoped<IEventStore, EventStore>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="Newtonsoft.Json"/> event store entity converter implementation to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXEventStoreConverterNewtonsoft(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IStoreEntityConverter, NewtonsoftStoreEntityConverter>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEventStoreDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXEventStoreDataContext(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddDbContext<EventStoreDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.AddScoped<IEventStoreDataContext, EventStoreDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEventStoreDataContext"/> type as <see cref="IEventStoreDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TEventStoreDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXEventStoreDataContext<TEventStoreDataContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TEventStoreDataContext : DbContext, IEventStoreDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddDbContext<TEventStoreDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.AddScoped<IEventStoreDataContext, TEventStoreDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type class reference implementation as <see cref="IDataContext"/> to the services with scoped life time.
        /// Caution : Do not use with multi-tenancy.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context that implements <see cref="IDataContext"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TDataContext : DbContext, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddDbContext<TDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.AddScoped<IDataContext, TDataContext>();
            return services;
        }
    }
}
