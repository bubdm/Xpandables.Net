
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
using System.Net.Http;

using Xpandables.Net5.HttpRestClient;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register HTTP rest client.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IHttpRestClientHandler"/> implementation to the collection of services.
        /// You may add <see cref="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{HttpClient})"/> or other to customize the handler behaviors.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddScoped<IHttpRestClientHandler, HttpRestClientHandler>();
            return services;
        }

        /// <summary>
        /// Adds an <see cref="HttpClient"/> named "IPGeoAddress" and "IPGeoLocation" to retrieve respectively the IPAddress and its location.
        /// You may reference the <see cref="IHttpRestClientIPGeoLocationHandler"/> to use it.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientIPGeoLocationHandler(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<HttpRestClientIPGeoAddressMessage>();
            services.AddTransient<IHttpRestClientIPGeoLocationHandler, HttpRestClientIPGeoLocationHandler>();

            services.AddHttpClient(IHttpRestClientIPGeoLocationHandler.IPGeoAddressHttpClientName, httpClient =>
             {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
                 httpClient.BaseAddress = new Uri("https://ipinfo.io/ip");
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
                 httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
             })
            .ConfigurePrimaryHttpMessageHandler<HttpRestClientIPGeoAddressMessage>();

            services.AddHttpClient(IHttpRestClientIPGeoLocationHandler.IPGeoLocationHttpClientName, httpClient =>
            {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
                httpClient.BaseAddress = new Uri("http://api.ipstack.com");
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
