
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

using System.Reflection;

using Xpandables.Net.Aggregates.Events;
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Provides method to register services.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="ICommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand, TResult}"/> and to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
    public static IXpandableServiceBuilder AddXCommandHandlers(this IXpandableServiceBuilder services, params Assembly[] assemblies)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
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
    /// Adds the <see cref="IDomainEventHandler{TEvent}"/> implementations to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
    public static IXpandableServiceBuilder AddXDomainEventHandlers(this IXpandableServiceBuilder services, params Assembly[] assemblies)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
            .Where(type => !type.IsAbstract
                && !type.IsInterface
                && !type.IsGenericType
                && type.GetInterfaces()
                    .Any(inter => inter.IsGenericType
                        && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .Select(type => new
            {
                Type = type,
                Interfaces = type.GetInterfaces()
                .Where(inter => inter.IsGenericType
                    && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
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
    /// Adds the <see cref="INotificationHandler{TNotificationEvent}"/> implementations to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
    public static IXpandableServiceBuilder AddXNotificationHandlers(this IXpandableServiceBuilder services, params Assembly[] assemblies)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
            .Where(type => !type.IsAbstract
                && !type.IsInterface
                && !type.IsGenericType
                && type.GetInterfaces()
                    .Any(inter => inter.IsGenericType
                        && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
            .Select(type => new
            {
                Type = type,
                Interfaces = type.GetInterfaces()
                .Where(inter => inter.IsGenericType
                && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
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
    /// <param name="assemblies">The assemblies to scan for implemented types. if not set, the calling assembly will be used.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
    public static IXpandableServiceBuilder AddXQueryHandlers(this IXpandableServiceBuilder services, params Assembly[] assemblies)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
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
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
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
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXQueryHandlerWrapper(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddTransient(typeof(AsyncQueryHandlerWrapper<,>));
        services.Services.AddTransient(typeof(QueryHandlerWrapper<,>));
        return services;
    }

    /// <summary>
    /// Adds and configures the <see cref="ICommandHandler{TCommand}"/>, <see cref="INotificationHandler{TNotificationEvent}"/>,
    /// <see cref="IQueryHandler{TQuery, TResult}"/>, <see cref="IDomainEventHandler{TEvent}"/>
    /// and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> behaviors.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="HandlerOptions"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHandlers(
        this IXpandableServiceBuilder services, Action<HandlerOptions> configureOptions, params Assembly[] assemblies)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
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
}
