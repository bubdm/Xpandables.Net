
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

using Xpandables.Net.CQRS;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure operations options.
    /// </summary>
    public sealed class HandlerOptions
    {
        /// <summary>
        /// Enables validation behavior to operations that are decorated with the <see cref="IValidationDecorator"/>.
        /// </summary>
        public HandlerOptions UseValidationDecorator() => this.With(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to operations that implement the <see cref="IVisitable{TVisitable}"/> interface.
        /// </summary>
        public HandlerOptions UseVisitDecorator() => this.With(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands that are decorated with the <see cref="IPersistenceDecorator"/> .
        /// </summary>
        public HandlerOptions UsePersistenceDecorator() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables notification behavior to commands that are decorated with the <see cref="INotificationDecorator"/> .
        /// </summary>
        public HandlerOptions UseNotificationDecorator() => this.With(cq => cq.IsNotificationEnabled = true);

        /// <summary>
        /// Enables correlation behavior to operations that are decorated with the <see cref="ICorrelationDecorator"/>.
        /// </summary>
        public HandlerOptions UseCorrelationDecorator() => this.With(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>.
        /// </summary>
        public HandlerOptions UseTransactionDecorator() => this.With(cq => cq.IsTransactionEnabled = true);

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsNotificationEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
    }

    /// <summary>
    /// Provides method to register commands queries.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
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
        /// Adds the <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationContext>();
            services.AddScoped<ICorrelationContext>(provider => provider.GetRequiredService<CorrelationContext>());
            return services;
        }

        /// <summary>
        /// Adds correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXCorrelationContext();

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandCorrelationDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryCorrelationDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IDispatcher"/> and <see cref="IDispatcherHandlerProvider"/> implementations to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDispatcher(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDispatcherHandlerProvider, DispatcherHandlerProvider>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDispatcher"/>, <typeparamref name="TNotificationDispatcher"/> and <typeparamref name="TDispatcherHandlerProvider"/> types to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDispatcher">The dispatcher type implementation.</typeparam>
        /// <typeparam name="TNotificationDispatcher">The notification type implementation.</typeparam>
        /// <typeparam name="TDispatcherHandlerProvider">The dispatcher handler provider type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDispatcher<TDispatcher, TNotificationDispatcher, TDispatcherHandlerProvider>(this IServiceCollection services)
            where TDispatcher : class, IDispatcher
            where TNotificationDispatcher : class, INotificationDispatcher
            where TDispatcherHandlerProvider : class, IDispatcherHandlerProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDispatcherHandlerProvider, TDispatcherHandlerProvider>();
            services.AddScoped<INotificationDispatcher, TNotificationDispatcher>();
            services.AddScoped<IDispatcher, TDispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type class reference implementation as <see cref="IDataContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context that implements <see cref="IDataContext"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContext>(this IServiceCollection services)
            where TDataContext : class, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            var serviceDescriptor = new ServiceDescriptor(typeof(IDataContext), provider => provider.GetRequiredService<TDataContext>(), ServiceLifetime.Scoped);
            services.Add(serviceDescriptor);
            services.AddScoped(typeof(IDataContext<>), typeof(DataContext<>));

            return services;
        }

        /// <summary>
        /// Adds the default implementation of <see cref="IEntityAccessor{TEntity}"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddEntityAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddScoped(typeof(IEntityAccessor<>), typeof(EntityAccessor<>));

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
        /// Adds notification behavior to commands that are decorated with the <see cref="INotificationDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXNotificationDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandNotificationDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandNotificationDecorator<,>));
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
            if (services is null) throw new ArgumentNullException(nameof(services));
            return services.AddScoped<ITransactionScopeProvider, TTransactionScopeProvider>();
        }

        /// <summary>
        /// Adds transaction scope behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>
        /// to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandTransactionDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandTransactionDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/> and <see cref="ICommandHandler{TCommand, TResult}"/> to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.AddXCommandHandlerWrapper();

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) })
                .ToList();

            foreach (var handler in genericHandlers)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

            var genericResultHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)) })
                .ToList();

            foreach (var handler in genericResultHandlers)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

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
        /// Adds the <see cref="INotificationHandler{TNotification}"/> implementations to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXNotificationHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(INotificationHandler<>)) })
                .ToList();

            foreach (var handler in genericHandlers)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="ICommandHandler{TCommand}"/>, <see cref="INotificationHandler{TNotification}"/>, <see cref="IQueryHandler{TQuery, TResult}"/> and <see cref="IAsyncQueryHandler{TQuery, TResult}"/> behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="HandlerOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHandlers(
            this IServiceCollection services, Assembly[] assemblies, Action<HandlerOptions> configureOptions)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.AddXCommandHandlers(assemblies);
            services.AddXQueryHandlers(assemblies);

            var definedOptions = new HandlerOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsCorrelationEnabled)
                services.AddXCorrelationDecorator();

            if (definedOptions.IsTransactionEnabled)
                services.AddXTransactionDecorator();

            if (definedOptions.IsPersistenceEnabled)
                services.AddXPersistenceDecorator();

            if (definedOptions.IsNotificationEnabled)
            {
                services.AddXNotificationHandlers(assemblies);
                services.AddXNotificationDecorator();
            }

            if (definedOptions.IsVisitorEnabled)
            {
                services.AddXVisitors(assemblies);
                services.AddXVisitorDecorator();
            }

            if (definedOptions.IsValidatorEnabled)
            {
                services.AddXValidations(assemblies);
                services.AddXValidationDecorator();
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
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.AddXQueryHandlerWrapper();

            var genericHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) })
                .ToList();

            foreach (var handler in genericHandlers)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

            var genericResultHandlers = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>)) })
                .ToList();

            foreach (var handler in genericResultHandlers)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

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
        /// Adds validation behavior to commands and queries that are decorated with the <see cref="IValidationDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXValidationDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(ICompositeValidation<>), typeof(CompositeValidation<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandValidatorDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandValidatorDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryValidatorDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryValidatorDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IValidation{TArgument}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXValidations(this IServiceCollection services, Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            var genericValidators = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IValidation<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IValidation<>)) })
                .ToList();

            foreach (var validator in genericValidators)
                foreach (var interf in validator.Interfaces)
                    services.AddScoped(interf, validator.Type);

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
            if (services is null) throw new ArgumentNullException(nameof(services));

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
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            var genericVisitors = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                .Where(type => type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IVisitor<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IVisitor<>)) })
                .ToList();

            foreach (var handler in genericVisitors)
                foreach (var interf in handler.Interfaces)
                    services.AddScoped(interf, handler.Type);

            return services;
        }
    }
}
