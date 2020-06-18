﻿
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
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register command and queries.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
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

            if (definedOptions.IsTransactionEnabled is { })
                services.AddXTransactionBehavior(definedOptions.IsTransactionEnabled);

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
                services.AddXIdentityBehavior(definedOptions.IsIdentityDataEnabled);

            return services;
        }
    }
}
