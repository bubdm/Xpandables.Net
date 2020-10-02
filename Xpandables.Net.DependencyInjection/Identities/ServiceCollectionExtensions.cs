
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
        /// Adds the default identity data type to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIdentityDataProvider(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddScoped<IIdentityDataProvider, IdentityDataProvider>();
        }

        /// <summary>
        /// Adds the identity data type to the services.
        /// </summary>
        /// <typeparam name="TIdentityProvider">The identity data type provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIdentityDataProvider<TIdentityProvider>(this IServiceCollection services)
            where TIdentityProvider : class, IIdentityDataProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddScoped<IIdentityDataProvider, TIdentityProvider>();
        }

        /// <summary>
        /// Adds the identity data type to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="identityProviderType">The identity data provider type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="identityProviderType"/> is null.</exception>
        public static IServiceCollection AddXIdentityDataProvider(this IServiceCollection services, Type identityProviderType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = identityProviderType ?? throw new ArgumentNullException(nameof(identityProviderType));

            if (!typeof(IIdentityDataProvider).IsAssignableFrom(identityProviderType))
                throw new ArgumentException($"{nameof(identityProviderType)} must implement {nameof(IIdentityDataProvider)}.");
            return services.AddScoped(typeof(IIdentityDataProvider), identityProviderType);
        }

        /// <summary>
        /// Adds identity data behavior to commands and queries that are decorated with the <see cref="IIdentityDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIdentityDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandIdentityDecorator<>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryIdentityDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryIdentityDecorator<,>));

            return services;
        }
    }
}
