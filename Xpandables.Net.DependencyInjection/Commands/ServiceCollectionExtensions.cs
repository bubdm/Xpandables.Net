
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

using Xpandables.Net.Commands;
using Xpandables.Net.Correlation;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Identities;
using Xpandables.Net.Queries;
using Xpandables.Net.Serilog;
using Xpandables.Net.Transactions;
using Xpandables.Net.Validations;
using Xpandables.Net.Visitors;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure command/query handlers.
    /// </summary>
    public sealed class CommandQueryOptions
    {
        /// <summary>
        /// Enables validation behavior to commands and queries that are decorated with the <see cref="IValidationDecorator"/>.
        /// </summary>
        public CommandQueryOptions UseValidatorDecorator() => this.Assign(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to commands and queries that implement the <see cref="IVisitable{TVisitable}"/> interface.
        /// </summary>
        public CommandQueryOptions UseVisitorDecorator() => this.Assign(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands and queries that are decorated with the <see cref="IPersistenceDecorator"/> .
        /// </summary>
        public CommandQueryOptions UsePersistenceDecorator() => this.Assign(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationDecorator"/>.
        /// </summary>
        public CommandQueryOptions UseCorrelationDecorator() => this.Assign(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands and queries that are decorated with the <see cref="ITransactionDecorator"/>.
        /// </summary>
        public CommandQueryOptions UseTransactionDecorator() => this.Assign(cq => cq.IsTransactionEnabled = true);

        /// <summary>
        /// Enables identity data behavior to commands and queries that are decorated with the <see cref="IIdentityDecorator"/>.
        /// </summary>
        public CommandQueryOptions UseIdentityDecorator<TIdentityDataProvider>()
            where TIdentityDataProvider : class, IIdentityDataProvider => this.Assign(cq => cq.IsIdentityDataEnabled = typeof(TIdentityDataProvider));

        /// <summary>
        /// Enables identity data behavior to commands and queries that are decorated with the <see cref="IIdentityDecorator"/>.
        /// </summary>
        /// <param name="identityDataProvider">The identity data provider type.</param>
        public CommandQueryOptions UseIdentityDecorator(Type identityDataProvider)
            => this.Assign(cq => cq.IsIdentityDataEnabled = identityDataProvider ?? throw new ArgumentNullException(nameof(identityDataProvider)));

        /// <summary>
        /// Enables logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/>.
        /// </summary>
        public CommandQueryOptions UseLoggingDecorator<TLogger>()
            where TLogger : class, ILoggerEngine => this.Assign(cq => cq.IsLoggingEnabled = typeof(TLogger));

        /// <summary>
        /// Enables logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/>.
        /// </summary>
        /// <param name="loggerType">The identity data provider type.</param>
        public CommandQueryOptions UseLoggingDecorator(Type loggerType)
            => this.Assign(cq => cq.IsLoggingEnabled = loggerType ?? throw new ArgumentNullException(nameof(loggerType)));

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
        internal Type? IsIdentityDataEnabled { get; private set; }
        internal Type? IsLoggingEnabled { get; private set; }
    }

    /// <summary>
    /// Provides method to register commands queries.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IAsyncCommandHandler{TCommand}"/> and <see cref="ICommandHandler{TCommand}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.XRegister(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAsyncCommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="IAsyncCommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand}"/>, 
        /// <see cref="IAsyncQueryHandler{TQuery, TResult}"/> and <see cref="IQueryHandler{TQuery, TResult}"/> behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="CommandQueryOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandQueriesHandlers(
            this IServiceCollection services, Assembly[] assemblies, Action<CommandQueryOptions> configureOptions)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.AddXQueryHandlerWrapper();
            services.AddXCommandHandlers(assemblies);
            services.AddXQueryHandlers(assemblies);

            var definedOptions = new CommandQueryOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsLoggingEnabled is { })
                services.AddXLoggingDecorator(definedOptions.IsLoggingEnabled);

            if (definedOptions.IsCorrelationEnabled)
                services.AddXCorrelationDecorator();

            if (definedOptions.IsTransactionEnabled)
                services.AddXTransactionDecorator();

            if (definedOptions.IsPersistenceEnabled)
                services.AddXPersistenceDecorator();

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

            if (definedOptions.IsIdentityDataEnabled is { })
            {
                services.AddXIdentityDataProvider(definedOptions.IsIdentityDataEnabled);
                services.AddXIdentityDecorator();
            }

            return services;
        }
    }
}
