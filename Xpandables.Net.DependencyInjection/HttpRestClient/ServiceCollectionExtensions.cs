
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

using Xpandables.Net.HttpRestClient;
using Xpandables.Net.HttpRestClient.Network;

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

        /// <summary>
        /// Adds an <see cref="IHttpRestClientIPHandler"/> to retrieve the IPAddress.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientIPHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<HttpRestClientIPMessageHandler>();
            services.AddHttpClient<IHttpRestClientIPHandler, HttpRestClientIPHandler>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://ipinfo.io/ip");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            })
            .ConfigurePrimaryHttpMessageHandler<HttpRestClientIPMessageHandler>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IHttpRestClientLocationHandler"/> to retrieve the user location.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientLocationHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddHttpClient<IHttpRestClientLocationHandler, HttpRestClientLocationHandler>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://api.ipstack.com");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
