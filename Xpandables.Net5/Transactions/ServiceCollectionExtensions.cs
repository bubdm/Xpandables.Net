
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
using System.Design.Behaviors;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds security behavior to commands and queries that are decorated with the <see cref="IBehaviorTransaction"/> to the services
        /// </summary>
        /// <typeparam name="TTransactionScopeProvider">The type transaction scope provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionBehavior<TTransactionScopeProvider>(this IServiceCollection services)
            where TTransactionScopeProvider : class, ITransactionScopeProvider
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            return services.AddXTransactionBehavior(typeof(TTransactionScopeProvider));
        }

        /// <summary>
        /// Adds transaction scope behavior to commands and queries that are decorated with the <see cref="IBehaviorTransaction"/>
        /// to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="transactionScopeProviderType">The type transaction scope provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTransactionBehavior(this IServiceCollection services, Type transactionScopeProviderType)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (transactionScopeProviderType is null) throw new ArgumentNullException(nameof(transactionScopeProviderType));

            services.AddScoped(typeof(ITransactionScopeProvider), transactionScopeProviderType);
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandTransactionBehavior<>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryTransactionBehavior<,>));
            return services;
        }
    }
}
