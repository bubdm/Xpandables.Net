
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

using Xpandables.Net.Commands;
using Xpandables.Net.Events;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand, TResult}"/> and to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCommandHandlers(this IXpandableServiceBuilder services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            services.AddXCommandHandlerWrapper();

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                               && !type.IsInterface
                               && !type.IsGenericType
                               && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType
                                                                     && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            var genericResultHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                               && !type.IsInterface
                               && !type.IsGenericType
                               && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType
                                                                     && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                })
                .ToList();

            foreach (var handler in genericResultHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

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
        /// Adds the <see cref="INotificationEventHandler{TAggregateId, TNotification}"/> implementations to the services with scope life time.
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
                            && inter.GetGenericTypeDefinition() == typeof(INotificationEventHandler<,>)))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces()
                    .Where(inter => inter.IsGenericType
                    && inter.GetGenericTypeDefinition() == typeof(INotificationEventHandler<,>))
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
        /// Adds the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXQueryHandlers(this IXpandableServiceBuilder services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            services.AddXQueryHandlerWrapper();

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            var genericResultHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>)) })
                .ToList();

            foreach (var handler in genericResultHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.Services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds the command handler wrapper necessary to resolve handlers using type inference.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCommandHandlerWrapper(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient(typeof(CommandHandlerWrapper<,>));
            return services;
        }

        /// <summary>
        /// Adds the query handler wrapper necessary to resolve handlers using type inference.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXQueryHandlerWrapper(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient(typeof(AsyncQueryHandlerWrapper<,>));
            services.Services.AddTransient(typeof(QueryHandlerWrapper<,>));
            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="ICommandHandler{TCommand}"/>, <see cref="INotificationEventHandler{TAggregateId, TNotificationEvent}"/>,
        /// <see cref="IQueryHandler{TQuery, TResult}"/>, <see cref="IDomainEventHandler{TAggregateId, TEvent}"/>
        /// and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="HandlerOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHandlers(
            this IXpandableServiceBuilder services, Assembly[] assemblies, Action<HandlerOptions> configureOptions)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddXCommandHandlers(assemblies);
            services.AddXQueryHandlers(assemblies);
            services.AddXDomainEventHandlers(assemblies);
            services.AddXNotificationHandlers(assemblies);

            var definedOptions = new HandlerOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsPersistenceEnabled)
            {
                services.AddXPersistenceDecorator();
            }

            if (definedOptions.IsTransactionEnabled)
            {
                services.AddXTransactionDecorator();
            }

            if (definedOptions.IsValidatorEnabled)
            {
                services.AddXValidators(assemblies);
                services.AddXValidationDecorator();
            }

            if (definedOptions.IsVisitorEnabled)
            {
                services.AddXVisitors(assemblies);
                services.AddXVisitorDecorator();
            }

            if (definedOptions.IsCorrelationEnabled)
            {
                services.AddXCorrelationEventDecorator();
            }

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
