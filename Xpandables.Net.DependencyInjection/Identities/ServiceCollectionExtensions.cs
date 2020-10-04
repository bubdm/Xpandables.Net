
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
using Xpandables.Net.Identities;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default token claims provider type to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTokenClaimProvider(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services)); 
            return services.AddScoped<ITokenClaimProvider, TokenClaimProvider>();
        }

        /// <summary>
        /// Adds the token claims provider data type to the services.
        /// </summary>
        /// <typeparam name="TTokenClaimProvider">The token claims provider type provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTokenClaimProvider<TTokenClaimProvider>(this IServiceCollection services)
            where TTokenClaimProvider : class, ITokenClaimProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddScoped<ITokenClaimProvider, TTokenClaimProvider>();
        }

        /// <summary>
        /// Adds the token claims provider data type to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="tokenClaimProviderType">The token claims provider type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenClaimProviderType"/> is null.</exception>
        public static IServiceCollection AddXTokenClaimProvider(this IServiceCollection services, Type tokenClaimProviderType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tokenClaimProviderType ?? throw new ArgumentNullException(nameof(tokenClaimProviderType));

            if (!typeof(ITokenClaimProvider).IsAssignableFrom(tokenClaimProviderType))
                throw new ArgumentException($"{nameof(tokenClaimProviderType)} must implement {nameof(ITokenClaimProvider)}.");
            return services.AddScoped(typeof(ITokenClaimProvider), tokenClaimProviderType);
        }

        /// <summary>
        /// Adds token claim data behavior to commands and queries that are decorated with the <see cref="ITokenClaimDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTokenClaimDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandTokenClaimDecorator<>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryTokenClaimDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryTokenClaimDecorator<,>));

            return services;
        }
    }
}
