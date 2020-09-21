
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

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register HTTP rest client.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an <see cref="IHttpRestClientIPLocationHandler"/> to retrieve the IPAddress.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientIPLocationHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<HttpRestClientIPLocationMessage>();
            services.AddHttpClient<IHttpRestClientIPLocationHandler, HttpRestClientIPLocationHandler>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://ipinfo.io/ip");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            })
            .ConfigurePrimaryHttpMessageHandler<HttpRestClientIPLocationMessage>();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IHttpRestClientGeoLocationHandler"/> to retrieve the user location.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientGeoLocationHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddHttpClient<IHttpRestClientGeoLocationHandler, HttpRestClientGeoLocationHandler>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://api.ipstack.com");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
