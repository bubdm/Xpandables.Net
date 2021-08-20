
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

using Xpandables.Net.Security;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides with lazy instance resolution.
    /// </summary>
    /// <typeparam name="T">The type to be resolved.</typeparam>
    public sealed class LazyResolved<T> : Lazy<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyResolved{T}" /> class that uses a preinitialized specified value from the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for preinitialized value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public LazyResolved(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>()) { }
    }

    /// <summary>
    /// Provides with methods to register Xpandable services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Allows registration of Xpandable Services.
        /// </summary>
        /// <param name="services">The service collection to act on.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXpandableServices(this IServiceCollection services)
            => new XpandableServiceBuilder(services);

        /// <summary>
        /// Adds the <see cref="IServiceScopeFactory{TService}"/> needed to resolve the <see cref="IServiceScope{TService}"/> to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXServiceScopeFactory(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton(typeof(IServiceScopeFactory<>), typeof(ServiceScopeFactory<>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IInstanceCreator"/> implementation to the services with singleton life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXInstanceCreator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<IInstanceCreator, InstanceCreator>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TInstanceCreator"/> type to the services with singleton life time.
        /// </summary>
        /// <typeparam name="TInstanceCreator">The type of instance creator.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXInstanceCreator<TInstanceCreator>(this IXpandableServiceBuilder services)
            where TInstanceCreator : class, IInstanceCreator
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<IInstanceCreator, TInstanceCreator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringCryptography"/> and <see cref="IStringGenerator"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStringGeneratorCryptography(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient<IStringCryptography, StringCryptography>();
            services.Services.AddTransient<IStringGenerator, StringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringGenerator"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStringGenerator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient<IStringGenerator, StringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TStringGenerator"/> type to the services with transient life time.
        /// </summary>
        /// <typeparam name="TStringGenerator">The string generator type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStringGenerator<TStringGenerator>(this IXpandableServiceBuilder services)
            where TStringGenerator : class, IStringGenerator
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient<IStringGenerator, TStringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringCryptography"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStringCryptography(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient<IStringCryptography, StringCryptography>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TStringCryptography"/> type to the services with transient life time.
        /// </summary>
        /// <typeparam name="TStringCryptography">The string cryptography type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStringCryptography<TStringCryptography>(this IXpandableServiceBuilder services)
            where TStringCryptography : class, IStringCryptography
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddTransient<IStringCryptography, TStringCryptography>();
            return services;
        }

        /// <summary>
        /// Adds the specified token engine to the services collection with scoped life time..
        /// </summary>
        /// <typeparam name="TTokenEngine">The token engine type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXTokenEngine<TTokenEngine>(this IXpandableServiceBuilder services)
            where TTokenEngine : class, ITokenEngine
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXTokenEngine(typeof(TTokenEngine));
            return services;
        }

        /// <summary>
        /// Adds the specified token engine to the services collection with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="tokenEngineType">The type that implements <see cref="ITokenEngine"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenEngineType"/> is null.</exception>
        public static IXpandableServiceBuilder AddXTokenEngine(this IXpandableServiceBuilder services, Type tokenEngineType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tokenEngineType ?? throw new ArgumentNullException(nameof(tokenEngineType));

            services.Services.AddScoped(typeof(ITokenEngine), tokenEngineType);
            return services;
        }

        /// <summary>
        /// Adds the specified refresh token engine to the services collection with scoped life time..
        /// </summary>
        /// <typeparam name="TRefreshTokenEngine">The refresh token engine type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXRefreshTokenEngine<TRefreshTokenEngine>(this IXpandableServiceBuilder services)
            where TRefreshTokenEngine : class, IRefreshTokenEngine
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IRefreshTokenEngine, TRefreshTokenEngine>();
            return services;
        }
    }
}
