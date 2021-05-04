
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
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xpandables.Net.Commands;
using Xpandables.Net.Correlations;
using Xpandables.Net.Database;
using Xpandables.Net.Decorators;
using Xpandables.Net.Decorators.Correlations;
using Xpandables.Net.Decorators.Logging;
using Xpandables.Net.Decorators.Persistences;
using Xpandables.Net.Decorators.Transactions;
using Xpandables.Net.Decorators.Validators;
using Xpandables.Net.Decorators.Visitors;
using Xpandables.Net.Dispatchers;
using Xpandables.Net.Events;
using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;
using Xpandables.Net.Handlers;
using Xpandables.Net.Queries;
using Xpandables.Net.Transactions;
using Xpandables.Net.Validators;
using Xpandables.Net.Visitors;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure operations options.
    /// </summary>
    public sealed class HandlerOptions
    {
        /// <summary>
        /// Enables validator behavior to operations that are decorated with the <see cref="IValidatorDecorator"/>.
        /// </summary>
        public HandlerOptions UseValidatorDecorator() => this.With(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to operations that implement the <see cref="IVisitable{TVisitable}"/> interface.
        /// </summary>
        public HandlerOptions UseVisitDecorator() => this.With(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands that are decorated with the <see cref="IPersistenceDecorator"/> .
        /// </summary>
        public HandlerOptions UsePersistenceDecorator() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables logging behavior to commands/queries that are decorated with the <see cref="ILoggingDecorator"/> .
        /// You must provide with an implementation of <see cref="IOperationResultLogger"/>.
        /// </summary>
        public HandlerOptions UseOperationResultLoggerDecorator() => this.With(cq => cq.IsLoggingEnabled = true);

        /// <summary>
        /// Enables correlation behavior to operations that are decorated with the <see cref="ICorrelationDecorator"/>.
        /// </summary>
        public HandlerOptions UseCorrelationDecorator() => this.With(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>.
        /// You must provide with an implementation of <see cref="ITransactionScopeProvider"/>.
        /// </summary>
        public HandlerOptions UseTransactionDecorator() => this.With(cq => cq.IsTransactionEnabled = true);

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
        internal bool IsLoggingEnabled { get; private set; }
    }

    /// <summary>
    /// Provides method to register commands queries.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IEventBusService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TIntegrationEventService">The integration event service type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIntegrationEventService<TIntegrationEventService>(this IServiceCollection services)
            where TIntegrationEventService : class, IHostedService, IEventBusService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IEventBusService, TIntegrationEventService>();
            services.AddHostedService(provider => provider.GetRequiredService<IEventBusService>() as TIntegrationEventService);
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEventBusService"/> type implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIntegrationEventService(this IServiceCollection services)
            => services.AddXIntegrationEventService<EventBusService>();

        /// <summary>
        /// Adds the <see cref="IIntegrationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <typeparam name="TIntegrationEventPublisher">The integration event publisher type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIntegrationEventPublisher<TIntegrationEventPublisher>(this IServiceCollection services)
            where TIntegrationEventPublisher : class, IIntegrationEventPublisher
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IIntegrationEventPublisher, TIntegrationEventPublisher>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IIntegrationEventPublisher"/> type implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIntegrationEventPublisher(this IServiceCollection services)
            => services.AddXIntegrationEventPublisher<IntegrationEventPublisher>();

        /// <summary>
        /// Adds the <see cref="IServiceScopeFactory{TService}"/> needed to resolve the <see cref="IServiceScope{TService}"/> to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXServiceScopeFactory(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddSingleton(typeof(IServiceScopeFactory<>), typeof(ServiceScopeFactory<>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="CorrelationCollection{TKey, TValue}"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationCollection(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped(typeof(CorrelationCollection<,>));
            return services;
        }

        /// <summary>
        /// Adds the implementation type of <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TCorrelationContext">the correlation context type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext<TCorrelationContext>(this IServiceCollection services)
            where TCorrelationContext : class, ICorrelationContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<ICorrelationContext, TCorrelationContext>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICorrelationEvent"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationEvent(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationEvent>();
            services.AddScoped<ICorrelationEvent>(provider => provider.GetRequiredService<CorrelationEvent>());
            return services;
        }

        /// <summary>
        /// Adds correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationEventDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXCorrelationEvent();

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandCorrelationDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryCorrelationDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds the operation result logging type to the services.
        /// </summary>
        /// <typeparam name="TOperationResultLogger">The operation result logger type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXOperationResultLogger<TOperationResultLogger>(this IServiceCollection services)
            where TOperationResultLogger : class, IOperationResultLogger
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddScoped<IOperationResultLogger, TOperationResultLogger>();
        }

        /// <summary>
        /// Adds operation result logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services.
        /// You need to register your implementation of <see cref="IOperationResultLogger"/> separately.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXOperationResultLoggerDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryLoggingDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryLoggingDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds operation result logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services.
        /// </summary>
        /// <typeparam name="TOperationResultLogger">The operation result logger type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXOperationResultLoggerDecorator<TOperationResultLogger>(this IServiceCollection services)
            where TOperationResultLogger : class, IOperationResultLogger
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXOperationResultLogger<TOperationResultLogger>();

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryLoggingDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryLoggingDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IDispatcher"/> implementations to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDispatcher(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IHandlerAccessor"/> implementations to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHandlerAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHandlerAccessor, HandlerAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type class reference implementation as <see cref="IDataContext"/> to the services with scoped life time.
        /// Caution : Do not use with multi-tenancy.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context that implements <see cref="IDataContext"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContext>(this IServiceCollection services)
            where TDataContext : class, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContext, TDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEventStoreDataContext"/> type class reference implementation as <see cref="IEventStoreDataContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TEventStoreDataContext">The type of the data context that implements <see cref="IEventStoreDataContext"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXEventStoreDataContext<TEventStoreDataContext>(this IServiceCollection services)
            where TEventStoreDataContext : class, IEventStoreDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IEventStoreDataContext, TEventStoreDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type to the collection of tenants in multi-tenancy context.
        /// The tenant will be named as the type of the data context.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IServiceCollection)"/>.</para>
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextTenant<TDataContext>(this IServiceCollection services)
            where TDataContext : class, IDataContext
        {
            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContextTenant),
                provider => new DataContextTenant<TDataContext>(() => provider.GetRequiredService<TDataContext>()),
                ServiceLifetime.Scoped);

            services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type to the collection of tenants in multi-tenancy context.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IServiceCollection)"/>.</para>
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="name">The unique identifier of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextTenant<TDataContext>(this IServiceCollection services, string name)
            where TDataContext : class, IDataContext
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContextTenant),
                provider => new DataContextTenant<TDataContext>(name, () => provider.GetRequiredService<TDataContext>()),
                ServiceLifetime.Scoped);

            services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextTenantAccessor"/> implementation type that get called to resolve <see cref="IDataContext"/> in multi-tenancy context.
        /// You have to register your data context(s) using the <see cref="AddXDataContextTenant{TDataContext}(IServiceCollection)"/>.
        /// The type is registered with scoped life time.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IServiceCollection)"/>.</para>
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextTenantAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContextTenantAccessor, DataContextTenantAccessor>();

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContext),
                provider => provider.GetRequiredService<IDataContextTenantAccessor>().GetDataContext(),
                ServiceLifetime.Scoped);

            services.Add(serviceDescriptor);

            return services;
        }

        /// <summary>
        /// Adds persistence behavior to commands and queries that are decorated with the <see cref="IPersistenceDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXPersistenceDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandPersistenceDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandPersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the transaction type provider to the services.
        /// </summary>
        /// <typeparam name="TTransactionScopeProvider">The type transaction scope provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionScopeProvider<TTransactionScopeProvider>(this IServiceCollection services)
            where TTransactionScopeProvider : class, ITransactionScopeProvider
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddScoped<ITransactionScopeProvider, TTransactionScopeProvider>();
        }

        /// <summary>
        /// Adds default transaction type provider to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionScopeProvider(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddScoped<ITransactionScopeProvider, TransactionScopeProvider>();
        }

        /// <summary>
        /// Adds transaction scope behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>
        /// to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionDecorator(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandTransactionDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandTransactionDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand, TResult}"/> and to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            services.AddXCommandHandlerWrapper();

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.AddScoped(interf, handler.Type);
                }
            }

            var genericResultHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)) })
                .ToList();

            foreach (var handler in genericResultHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds the command handler wrapper necessary to resolve handlers using type inference.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlerWrapper(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(CommandHandlerWrapper<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDomainEventHandler{TEvent}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXDomainEventHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)) })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IIntegrationEventHandler{TEvent}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXIntegrationEventHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)) })
                .ToList();

            foreach (var handler in genericHandlers)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="ICommandHandler{TCommand}"/>,
        /// <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="HandlerOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHandlers(
            this IServiceCollection services, Assembly[] assemblies, Action<HandlerOptions> configureOptions)
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
                services.AddXValidations(assemblies);
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

            if (definedOptions.IsLoggingEnabled)
            {
                services.AddXOperationResultLoggerDecorator();
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
        public static IServiceCollection AddXQueryHandlers(this IServiceCollection services, Assembly[] assemblies)
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
                    services.AddScoped(interf, handler.Type);
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
                    services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds the query handler wrapper necessary to resolve handlers using type inference.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQueryHandlerWrapper(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(AsyncQueryHandlerWrapper<,>));
            services.AddTransient(typeof(QueryHandlerWrapper<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="IMetadataDescriptionProvider"/> to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXMetadataDescriptionProvider(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<IMetadataDescriptionProvider, MetadataDescriptionProvider>();
            return services;
        }

        /// <summary>
        /// Adds validation behavior to commands and queries that are decorated with the <see cref="IValidatorDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXValidationDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(ICompositeValidator<>), typeof(CompositeValidator<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandValidatorDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandValidatorDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryValidatorDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryValidatorDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IValidator{TArgument}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXValidations(this IServiceCollection services, Assembly[] assemblies)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericValidators = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IValidator<>)) })
                .ToList();

            foreach (var validator in genericValidators)
            {
                foreach (var interf in validator.Interfaces)
                {
                    services.AddScoped(interf, validator.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Adds visitor behavior to commands and queries that are decorated with the <see cref="IVisitable{TVisitable}"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXVisitorDecorator(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient(typeof(ICompositeVisitor<>), typeof(CompositeVisitor<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandVisitorDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandVisitorDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryVisitorDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryVisitorDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IVisitor{TElement}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXVisitors(this IServiceCollection services, Assembly[] assemblies)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var genericVisitors = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IVisitor<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IVisitor<>)) })
                .ToList();

            foreach (var handler in genericVisitors)
            {
                foreach (var interf in handler.Interfaces)
                {
                    services.AddScoped(interf, handler.Type);
                }
            }

            return services;
        }
    }
}
