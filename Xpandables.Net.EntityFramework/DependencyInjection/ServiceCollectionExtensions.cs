
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
using System.Linq;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Database;
using Xpandables.Net.NotificationEvents;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register EFCore objects.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IEventBus"/> implementation to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventBus(this IXpandableServiceBuilder services)
            => services.AddXEventBus<EventBus>();

        /// <summary>
        /// Adds the default <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/> implementation 
        /// to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXAggregateAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped(typeof(IAggregateAccessor<,>), typeof(AggregateAccessor<,>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IAggregateDataContext"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXAggregateDataContext(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<AggregateDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IAggregateDataContext, AggregateDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TAggregateDataContext"/> type as <see cref="IAggregateDataContext"/> 
        /// implementation to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TAggregateDataContext">The type of the event store data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
        /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
        /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXAggregateDataContext<TAggregateDataContext>(
            this IXpandableServiceBuilder services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TAggregateDataContext : AggregateDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddDbContext<TAggregateDataContext>(optionsAction, contextLifetime, optionsLifetime);
            services.Services.AddScoped<IAggregateDataContext, TAggregateDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEntityAccessor{TEntity}"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEntityAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped(typeof(IEntityAccessor<>), typeof(EntityAccessor<>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEntityAccessor{TEntity}"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="entityAccessorType">The generic entity accessor that implements <see cref="IEntityAccessor{TEntity}"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityAccessorType"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEntityAccessor(this IXpandableServiceBuilder services, Type entityAccessorType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = entityAccessorType ?? throw new ArgumentNullException(nameof(entityAccessorType));

            if (!entityAccessorType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityAccessor<>)))
                throw new ArgumentException(
                    $"the type '{nameof(entityAccessorType)}' must implement the '{typeof(IEntityAccessor<>).GetNameWithoutGenericArity()}' interface.");

            services.Services.AddScoped(typeof(IEntityAccessor<>), entityAccessorType);
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
