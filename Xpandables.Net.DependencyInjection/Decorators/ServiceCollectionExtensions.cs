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

using System;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Commands;
using Xpandables.Net.Decorators;
using Xpandables.Net.Decorators.Persistences;
using Xpandables.Net.Decorators.Transactions;
using Xpandables.Net.Decorators.Validators;
using Xpandables.Net.Decorators.Visitors;
using Xpandables.Net.Notifications;
using Xpandables.Net.Queries;
using Xpandables.Net.Transactions;
using Xpandables.Net.Validators;
using Xpandables.Net.Visitors;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds persistence behavior to commands and queries that are decorated with the <see cref="IPersistenceDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXPersistenceDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandPersistenceDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandPersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds aggregate persistence behavior to commands and queries that are decorated with the <see cref="IAggregate"/> and
        /// <see cref="IPersistenceDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXAggregatePersistenceDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(AggregatePersistenceDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(AggregatePersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds aggregate persistence behavior to integration events that are decorated with the <see cref="IPersistenceDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXNotificationPersistenceDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(INotificationHandler<,>), typeof(NotificationPersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the transaction type provider to the services.
        /// </summary>
        /// <typeparam name="TTransactionScopeProvider">The type transaction scope provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXTransactionScopeProvider<TTransactionScopeProvider>(this IXpandableServiceBuilder services)
            where TTransactionScopeProvider : class, ITransactionScopeProvider
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Services.AddScoped<ITransactionScopeProvider, TTransactionScopeProvider>();
            return services;
        }

        /// <summary>
        /// Adds default transaction type provider to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXTransactionScopeProvider(this IXpandableServiceBuilder services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Services.AddScoped<ITransactionScopeProvider, TransactionScopeProvider>();
            return services;
        }

        /// <summary>
        /// Adds transaction scope behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>
        /// to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXTransactionDecorator(this IXpandableServiceBuilder services)
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
        /// Adds validation behavior to commands and queries that are decorated with the <see cref="IValidatorDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXValidationDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient(typeof(ICompositeValidator<>), typeof(CompositeValidator<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandValidatorDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandValidatorDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryValidatorDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryValidatorDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds visitor behavior to commands and queries that are decorated with the <see cref="IVisitable{TVisitable}"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXVisitorDecorator(this IXpandableServiceBuilder services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Services.AddTransient(typeof(ICompositeVisitor<>), typeof(CompositeVisitor<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandVisitorDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandVisitorDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryVisitorDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryVisitorDecorator<,>));
            return services;
        }

    }
}
