
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
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Http;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandler(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpRestClientHandler<HttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> that uses <see cref="Newtonsoft"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandlerUsingNewtonsoft(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpRestClientHandler<HttpRestClientHandlerUsingNewtonsoft>(configureClient);
            return services;
        }

        private static void DoAddXHttpRestClientHandler<THttpRestClientHandler>(this IServiceCollection services, Action<HttpClient> configureClient)
            where THttpRestClientHandler : class, IHttpRestClientHandler => services.AddHttpClient<IHttpRestClientHandler, THttpRestClientHandler>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <typeparam name="THttpRestClientHandler"> The implementation type of the typed client (<see cref="IHttpRestClientHandler"/>). They type specified will be instantiated by the ITypedHttpClientFactory.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandler<THttpRestClientHandler>(this IServiceCollection services, Action<HttpClient> configureClient)
            where THttpRestClientHandler : class, IHttpRestClientHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddHttpClient<IHttpRestClientHandler, THttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpHeaderAccessor"/> using
        ///  the <see cref="AddXHttpHeaderAccessor{THttpHeaderAccessor}(IServiceCollection)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandlerWithAuthorizationTokenHandler(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> that use <see cref="Newtonsoft"/> and you need to register an implementation
        ///  of <see cref="IHttpHeaderAccessor"/> using the <see cref="AddXHttpHeaderAccessor{THttpHeaderAccessor}(IServiceCollection)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandlerWithAuthorizationTokenHandlerUsingNewtonsoft(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientHandlerUsingNewtonsoft>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpHeaderAccessor"/> using
        ///  the <see cref="AddXHttpHeaderAccessor{THttpHeaderAccessor}(IServiceCollection)"/> method.
        /// </summary>
        /// <typeparam name="THttpRestClientHandler"> The implementation type of the typed client (<see cref="IHttpRestClientHandler"/>). They type specified will be instantiated by the ITypedHttpClientFactory.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientHandler>(this IServiceCollection services, Action<HttpClient> configureClient)
            where THttpRestClientHandler : class, IHttpRestClientHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientHandler>(configureClient);
            return services;
        }

        private static void DoAddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientHandler>(this IServiceCollection services, Action<HttpClient> configureClient)
             where THttpRestClientHandler : class, IHttpRestClientHandler
        {
            services
                .AddHttpClient<IHttpRestClientHandler, THttpRestClientHandler>(configureClient)
                .ConfigureXPrimaryAuthorizationTokenHandler();
        }

        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpHeaderAccessor">The type of HTTP request header.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpHeaderAccessor<THttpHeaderAccessor>(this IServiceCollection services)
            where THttpHeaderAccessor : class, IHttpHeaderAccessor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpHeaderAccessor, THttpHeaderAccessor>();
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
        /// Adds an <see cref="IHttpIPAddressAccessor"/> implementation to retrieve the IPAddress of caller from https://ipinfo.io/ip.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpIPAddressAccessor<HttpIPAddressAccessor>();
            return services;
        }

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressAccessor"/> implementation to retrieve the IPAddress of caller that uses <see cref="Newtonsoft"/> from https://ipinfo.io/ip.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressAccessorUsingNewtonsoft(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpIPAddressAccessor<HttpIPAddressAccessorUsingNewtonsoft>();
            return services;
        }

        private static void DoAddXHttpIPAddressAccessor<THttpIPAddressAccessor>(this IServiceCollection services)
            where THttpIPAddressAccessor : class, IHttpIPAddressAccessor
        {
            services.AddHttpClient<IHttpIPAddressAccessor, THttpIPAddressAccessor>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://ipinfo.io/ip");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            })
             .ConfigurePrimaryHttpMessageHandler(() => new HttpIPAddressDelegateHandler());
        }

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressLocationAccessor"/> implementation to retrieve the IP Address location from http://api.ipstack.com.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressLocationAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpIPAddressLocationAccessor<HttpIPAddressLocationAccessor>();
            return services;
        }

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressLocationAccessor"/> implementation to retrieve the IP Address location that uses <see cref="Newtonsoft"/> from http://api.ipstack.com.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressLocationAccessorUsingNewtonsoft(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.DoAddXHttpIPAddressLocationAccessor<HttpIPAddressLocationAccessorUsingNewtonsoft>();
            return services;
        }

        private static void DoAddXHttpIPAddressLocationAccessor<THttpIPAddressLocationAccessor>(this IServiceCollection services)
            where THttpIPAddressLocationAccessor : class, IHttpIPAddressLocationAccessor
        {
            services.AddHttpClient<IHttpIPAddressLocationAccessor, THttpIPAddressLocationAccessor>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://api.ipstack.com");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            });
        }
    }
}
