
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

using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Http;
using Xpandables.Net.Http.Network;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpHeaderAccessor">The type of HTTP request header.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpHeaderAccessor<THttpHeaderAccessor>(
            this IServiceCollection services)
            where THttpHeaderAccessor : class, IHttpHeaderAccessor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpHeaderAccessor, THttpHeaderAccessor>();
            return services;
        }     

        /// <summary>
        /// Adds the specified HTTP request token claim provider that implements the <see cref="ITokenEntityProvider"/>.
        /// </summary>
        /// <typeparam name="THttpTokenEntityProvider">The type of HTTP token claim provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenClaimProvider<THttpTokenEntityProvider>(
            this IServiceCollection services)
            where THttpTokenEntityProvider : class, ITokenEntityProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<ITokenEntityProvider, THttpTokenEntityProvider>();
            return services;
        }

        /// <summary>
        /// Adds the token claims provider type to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="tokenClaimProviderType">The token claims provider type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenClaimProviderType"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenClaimProvider(this IServiceCollection services, Type tokenClaimProviderType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tokenClaimProviderType ?? throw new ArgumentNullException(nameof(tokenClaimProviderType));

            if (!typeof(ITokenEntityProvider).IsAssignableFrom(tokenClaimProviderType))
                throw new ArgumentException($"{nameof(tokenClaimProviderType)} must implement {nameof(ITokenEntityProvider)}.");

            return services.AddScoped(typeof(ITokenEntityProvider), tokenClaimProviderType);
        }

        /// <summary>
        /// Adds the specified token engine to the services collection.
        /// </summary>
        /// <typeparam name="THttpTokenEngine">The token engine type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenEngine<THttpTokenEngine>(this IServiceCollection services)
            where THttpTokenEngine : class, ITokenEngine
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddXHttpTokenEngine(typeof(THttpTokenEngine));
        }

        /// <summary>
        /// Adds the specified token engine to the services collection with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="tokenEngineType">The type that implements <see cref="ITokenEngine"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenEngineType"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenEngine(this IServiceCollection services, Type tokenEngineType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tokenEngineType ?? throw new ArgumentNullException(nameof(tokenEngineType));

            services.AddScoped(typeof(ITokenEngine), tokenEngineType);
            return services;
        }

        /// <summary>
        /// Adds a delegate that will be used to provide the authorization token before request execution
        /// using an implementation of <see cref="IHttpHeaderAccessor"/>. You need to register the implementation using
        /// the <see cref="AddXHttpHeaderAccessor{THttpHeaderAccessor}(IServiceCollection)"/> or the default extension.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IHttpClientBuilder.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        public static IHttpClientBuilder ConfigureXPrimaryAuthorizationTokenHandler(this IHttpClientBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var httpHeaderAccessor = provider.GetRequiredService<IHttpHeaderAccessor>();
                return new HttpRestClientAuthorizationHandler(httpHeaderAccessor);
            });

            return builder;
        }

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressAccessor"/> to retrieve the IPAddress.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IHttpIPAddressAccessor, HttpIPAddressAccessor>();
            return services;
        }

        /// <summary>
        /// Adds an <see cref="IHttpLocationAccessor"/> to retrieve the user location.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpLocationAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IHttpLocationAccessor, HttpLocationAccessor>();
            return services;
        }
    }
}
