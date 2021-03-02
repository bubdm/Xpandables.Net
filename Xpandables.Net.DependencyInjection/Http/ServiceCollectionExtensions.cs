
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
using Xpandables.Net.Http.RequestBuilders;
using Xpandables.Net.Http.RequestHandlers;
using Xpandables.Net.Http.RequestLocations;
using Xpandables.Net.Http.ResponseBuilders;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandler(this IServiceCollection services, Action<HttpClient> configureClient)
            => services.AddXHttpRestClientHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, HttpRestClientAsyncEnumerableBuilder>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, THttpRestClientAsyncEnumerableBuilder>(
            this IServiceCollection services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where THttpRestClientAsyncEnumerableBuilder : class, IHttpRestClientAsyncEnumerableBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.AddScoped<IHttpRestClientAsyncEnumerableBuilder, THttpRestClientAsyncEnumerableBuilder>();
            services.AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and use NewtonSoft serializer.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IServiceCollection AddXHttpRestClientNewtonSoftHandler(this IServiceCollection services, Action<HttpClient> configureClient)
            => services.AddXHttpRestClientHandler<HttpRestClientNewtonSoftRequestBuilder, HttpRestClientNewtonSoftResponseBuilder, HttpRestClientNewtonSoftAsyncEnumerableBuilder>(configureClient);

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
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, HttpRestClientAsyncEnumerableBuilder>(configureClient);

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
        public static IServiceCollection AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, THttpRestClientAsyncEnumerableBuilder>(
            this IServiceCollection services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where THttpRestClientAsyncEnumerableBuilder : class, IHttpRestClientAsyncEnumerableBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.AddScoped<IHttpRestClientAsyncEnumerableBuilder, THttpRestClientAsyncEnumerableBuilder>();
            services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .ConfigureXPrimaryAuthorizationTokenHandler();

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
        public static IServiceCollection AddXHttpRestClientNewtonSoftHandlerWithAuthorizationTokenHandler(this IServiceCollection services, Action<HttpClient> configureClient)
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientNewtonSoftRequestBuilder, HttpRestClientNewtonSoftResponseBuilder, HttpRestClientNewtonSoftAsyncEnumerableBuilder>(configureClient);

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
        /// The default URI used to retrieve the IP address.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>")]
        public const string DefaultIPAddressFinderUri = "https://ipinfo.io/ip";

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressAccessor"/> implementation to retrieve the IPAddress of caller from https://ipinfo.io/ip.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var descriptor = new ServiceDescriptor(
                typeof(IHttpIPAddressAccessor),
                provider =>
                {
                    var requestBuilder = provider.GetRequiredService<IHttpRestClientRequestBuilder>();
                    var responseBuilder = provider.GetRequiredService<IHttpRestClientResponseBuilder>();
                    var asyncBuilder = provider.GetRequiredService<IHttpRestClientAsyncEnumerableBuilder>();
                    var client = new HttpClient(new HttpIPAddressDelegateHandler())
                    {
                        BaseAddress = new Uri(DefaultIPAddressFinderUri)
                    };
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                    return new HttpIPAddressAccessor(new HttpRestClientHandler(asyncBuilder, requestBuilder, responseBuilder, client));
                },
                ServiceLifetime.Scoped);

            services.Add(descriptor);
            return services;
        }

        /// <summary>
        /// The default URI used to retrieve the IP address location.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>")]
        public const string DefaultIPAddressLocationFinderUri = "http://api.ipstack.com";

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressLocationAccessor"/> implementation to retrieve the IP Address location from http://api.ipstack.com.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpIPAddressLocationAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var descriptor = new ServiceDescriptor(
                typeof(IHttpIPAddressLocationAccessor),
                provider =>
                {
                    var requestBuilder = provider.GetRequiredService<IHttpRestClientRequestBuilder>();
                    var responseBuilder = provider.GetRequiredService<IHttpRestClientResponseBuilder>();
                    var asyncBuilder = provider.GetRequiredService<IHttpRestClientAsyncEnumerableBuilder>();
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(DefaultIPAddressLocationFinderUri)
                    };
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                    return new HttpIPAddressLocationAccessor(new HttpRestClientHandler(asyncBuilder, requestBuilder, responseBuilder, client));
                },
                ServiceLifetime.Scoped);

            services.Add(descriptor);
            return services;
        }
    }
}
