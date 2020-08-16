
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
using Xpandables.Net.Correlation;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Extensions;
using Xpandables.Net.Identities;
using Xpandables.Net.Queries;
using Xpandables.Net.Retry;
using Xpandables.Net.Transactions;
using Xpandables.Net.ValidatorRules;
using Xpandables.Net.VisitorRules;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure command/query handlers.
    /// </summary>
    public sealed class CommandQueryOptions
    {
        /// <summary>
        /// Enables validation behavior to commands and queries that are decorated with the <see cref="IBehaviorValidation"/>.
        /// </summary>
        public CommandQueryOptions UseValidatorBehavior() => this.With(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to commands and queries that implement the <see cref="IVisitable"/> interface.
        /// </summary>
        public CommandQueryOptions UseVisitorBehavior() => this.With(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands and queries that are decorated with the <see cref="IBehaviorPersistence"/> .
        /// </summary>
        public CommandQueryOptions UsePersistenceBehavior() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables correlation behavior to commands and queries that are decorated with the <see cref="IBehaviorCorrelation"/>.
        /// </summary>
        public CommandQueryOptions UseCorrelationBehavior() => this.With(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables retry behavior to commands and queries that are decorated with the <see cref="IBehaviorRetry"/>.
        /// </summary>
        public CommandQueryOptions UseRetryBehavior() => this.With(cq => cq.IsRetryEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands and queries that are decorated with the <see cref="IBehaviorTransaction"/>.
        /// </summary>
        public CommandQueryOptions UseTransactionBehavior() => this.With(cq => cq.IsTransactionEnabled = true);

        /// <summary>
        /// Enables identity data behavior to commands and queries that are decorated with the <see cref="IBehaviorIdentity"/>.
        /// </summary>
        public CommandQueryOptions UseIdentityBehavior<TIdentityDataProvider>() => this.With(cq => cq.IsIdentityDataEnabled = typeof(TIdentityDataProvider));

        /// <summary>
        /// Enables identity data behavior to commands and queries that are decorated with the <see cref="IBehaviorIdentity"/>.
        /// </summary>
        /// <param name="identityDataProvider">The identity data provider type.</param>
        public CommandQueryOptions UseIdentityBehavior(Type identityDataProvider)
            => this.With(cq => cq.IsIdentityDataEnabled = identityDataProvider ?? throw new ArgumentNullException(nameof(identityDataProvider)));

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
        internal bool IsRetryEnabled { get; private set; }
        internal Type? IsIdentityDataEnabled { get; private set; }
    }

    /// <summary>
    /// Provides method to register commands queries.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.XRegister(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="ICommandHandler{TCommand}"/> and <see cref="IQueryHandler{TQuery, TResult}"/> behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="CommandQueryOptions"/>.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandQueriesHandlers(
            this IServiceCollection services, Action<CommandQueryOptions> configureOptions, params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.AddXQueryHandlerWrapper();
            services.AddXCommandHandlers(assemblies);
            services.AddXQueryHandlers(assemblies);

            var definedOptions = new CommandQueryOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsCorrelationEnabled)
                services.AddXCorrelationBehavior();

            if (definedOptions.IsRetryEnabled)
                services.AddXRetryBehavior();

            if (definedOptions.IsTransactionEnabled)
                services.AddXTransactionBehavior();

            if (definedOptions.IsPersistenceEnabled)
                services.AddXPersistenceBehavior();

            if (definedOptions.IsVisitorEnabled)
            {
                services.AddXVisitorRules(assemblies);
                services.AddXVisitorBehavior();
            }

            if (definedOptions.IsValidatorEnabled)
            {
                services.AddXValidatorRules(assemblies);
                services.AddXValidatorRuleBehavior();
            }

            if (definedOptions.IsIdentityDataEnabled is { })
            {
                services.AddXIdentityDataProvider(definedOptions.IsIdentityDataEnabled);
                services.AddXIdentityBehavior();
            }

            return services;
        }
    }
}
