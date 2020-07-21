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

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;
using Xpandables.Net.Transactions;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace

{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
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
        /// Adds transaction scope behavior to commands and queries that are decorated with the <see cref="IBehaviorTransaction"/>
        /// to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionBehavior(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandTransactionBehavior<>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryTransactionBehavior<,>));
            return services;
        }
    }
}