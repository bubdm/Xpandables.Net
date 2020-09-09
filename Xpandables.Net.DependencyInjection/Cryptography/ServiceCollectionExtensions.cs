
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

using Xpandables.Net.Cryptography;
using Xpandables.Net.Strings;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register correlation collection.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IStringCryptography"/> and <see cref="IStringGenerator"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringGeneratorCryptography(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IStringCryptography, StringCryptography>();
            services.AddTransient<IStringGenerator, StringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringGenerator"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringGenerator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IStringGenerator, StringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TStringGenerator"/> type to the services with transient life time.
        /// </summary>
        /// <typeparam name="TStringGenerator">The string generator type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringGenerator<TStringGenerator>(this IServiceCollection services)
            where TStringGenerator : class, IStringGenerator
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IStringGenerator, TStringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringCryptography"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringCryptography(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IStringCryptography, StringCryptography>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TStringCryptography"/> type to the services with transient life time.
        /// </summary>
        /// <typeparam name="TStringCryptography">The string cryptography type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringCryptography<TStringCryptography>(this IServiceCollection services)
            where TStringCryptography : class, IStringCryptography
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IStringCryptography, TStringCryptography>();
            return services;
        }
    }
}
