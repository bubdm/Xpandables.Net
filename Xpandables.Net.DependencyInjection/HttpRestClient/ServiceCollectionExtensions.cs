
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

using Xpandables.Net.HttpRestClient;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register HTTP rest client.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IHttpRestClientEngine"/> implementation to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientEngine(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<IHttpRestClientEngine, HttpRestClientEngine>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IHttpRestClientEngine"/> implementation to the services.
        /// </summary>
        /// <typeparam name="THttpRestClientEngine">The type of the HTTP rest client engine.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientEngine<THttpRestClientEngine>(this IServiceCollection services)
            where THttpRestClientEngine : class, IHttpRestClientEngine
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<IHttpRestClientEngine, THttpRestClientEngine>();

            return services;
        }        
    }
}
