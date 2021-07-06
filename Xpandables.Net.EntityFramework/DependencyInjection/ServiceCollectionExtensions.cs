
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

using Xpandables.Net.Database;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register EFCore objects.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IDomainEventDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainEventDataContext(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<DomainEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IDomainEventDataContext, DomainEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="INotificationEventDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationEventDataContext(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<NotificationEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<INotificationEventDataContext, NotificationEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEmailEventDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEmailEventDataContext(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<EmailEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IEmailEventDataContext, EmailEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="ISnapShotDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXSnapShotDataContext(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<SnapShotDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<ISnapShotDataContext, SnapShotDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDomainEventDataContext"/> type as <see cref="IDomainEventDataContext"/> 
        /// implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDomainEventDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainEventDataContext<TDomainEventDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TDomainEventDataContext : DomainEventDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TDomainEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IDomainEventDataContext, TDomainEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEmailEventDataContext"/> type as <see cref="IEmailEventDataContext"/> 
        /// implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TEmailEventDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEmailEventDataContext<TEmailEventDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TEmailEventDataContext : EmailEventDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TEmailEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IEmailEventDataContext, TEmailEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TNotificationEventDataContext"/> type as <see cref="INotificationEventDataContext"/> 
        /// implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TNotificationEventDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationEventDataContext<TNotificationEventDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TNotificationEventDataContext : NotificationEventDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TNotificationEventDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<INotificationEventDataContext, TNotificationEventDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TSnapShotDataContext"/> type as <see cref="ISnapShotDataContext"/> 
        /// implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TSnapShotDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXSnapShotDataContext<TSnapShotDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TSnapShotDataContext : SnapShotDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TSnapShotDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<ISnapShotDataContext, TSnapShotDataContext>();
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
        public static IXpandableServiceBuilder AddXDataContext<TDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TDataContext : DbContext, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IDataContext, TDataContext>();
            return services;
        }
    }
}
