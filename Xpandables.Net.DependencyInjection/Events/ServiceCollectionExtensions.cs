
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

using System;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Aggregates;
using Xpandables.Net.DomainEvents;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDomainEventHandler{TAggregateId, TEvent}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainEventHandlers(this IXpandableServiceBuilder services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType
                    && type.GetInterfaces()
                        .Any(inter => inter.IsGenericType
                            && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<,>)))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces()
                    .Where(inter => inter.IsGenericType
                        && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<,>))
                })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds the <see cref="INotificationHandler{TAggregateId, TNotification}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationHandlers(this IXpandableServiceBuilder services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType
                    && type.GetInterfaces()
                        .Any(inter => inter.IsGenericType
                            && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<,>)))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces()
                    .Where(inter => inter.IsGenericType
                    && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<,>))
                })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="INotificationHandler{TAggregateId, TNotification}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="EventOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationHandlers(
            this IXpandableServiceBuilder services, Assembly[] assemblies, Action<EventOptions> configureOptions)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
                throw new ArgumentNullException(nameof(assemblies));

            services.AddXNotificationHandlers(assemblies);
            var definedOptions = new EventOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsPersistenceEnabled)
                services.AddXNotificationPersistenceDecorator();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEventStore{TAggregateId}"/> implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TAggregateId">The type of the aggregate identifier.</typeparam>
        /// <typeparam name="TEventStore">The type that implements <see cref="IEventStore{TAggregateId}"/></typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventStore<TAggregateId, TEventStore>(this IXpandableServiceBuilder services)
            where TEventStore : class, IEventStore<TAggregateId>
            where TAggregateId : notnull, IAggregateId
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped<IEventStore<TAggregateId>, TEventStore>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="System.Text.Json"/> event store entity converter implementation to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventStoreConverter(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddSingleton<IStoreEntityConverter, StoreEntityConverter>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TStoreEntityConverter"/> as <see cref="IStoreEntityConverter"/> event store entity converter 
        /// implementation to the services with singleton life time.
        /// </summary>
        /// <typeparam name="TStoreEntityConverter">Type of the converter.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventStoreConverter<TStoreEntityConverter>(this IXpandableServiceBuilder services)
            where TStoreEntityConverter : class, IStoreEntityConverter
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddSingleton<IStoreEntityConverter, TStoreEntityConverter>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEventBus"/> implementation to the services with singleton life time.
        /// </summary>
        /// <typeparam name="TEventBus">The type that implements <see cref="IEventBus"/></typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventBus<TEventBus>(this IXpandableServiceBuilder services)
            where TEventBus : class, IEventBus
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped<IEventBus, TEventBus>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEventBus"/> implementation to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEventBus(this IXpandableServiceBuilder services)
            => services.AddXEventBus<EventBus>();

        /// <summary>
        /// Adds the default <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/> implementation to the services with scope life time.
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
        /// Adds the <see cref="IDomainEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TDomainEventPublisher">The domain event publisher type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainEventPublisher<TDomainEventPublisher>(this IXpandableServiceBuilder services)
            where TDomainEventPublisher : class, IDomainEventPublisher
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IDomainEventPublisher, TDomainEventPublisher>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IDomainEventPublisher"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDomainEventPublisher(this IXpandableServiceBuilder services)
            => services.AddXDomainEventPublisher<DomainEventPublisher>();

    }
}
