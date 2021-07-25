
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
        /// Adds the <see cref="IHttpRestClientRequestBuilder"/> default implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> services.</returns>
        public static IXpandableServiceBuilder AddXHttpRestClientRequestBuilder(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped<IHttpRestClientRequestBuilder, HttpRestClientRequestBuilder>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IHttpRestClientResponseBuilder"/> default implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> services.</returns>
        public static IXpandableServiceBuilder AddXHttpRestClientResponseBuilder(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped<IHttpRestClientResponseBuilder, HttpRestClientResponseBuilder>();
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler(this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            => services.AddXHttpRestClientHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<TDelegatingHandler>(this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            where TDelegatingHandler : DelegatingHandler
            => services.AddXHttpRestClientHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, TDelegatingHandler>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler(this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            => services.AddXHttpRestClientHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<TDelegatingHandler>(this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            where TDelegatingHandler : DelegatingHandler
            => services.AddXHttpRestClientHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, TDelegatingHandler>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
            this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
            this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where TDelegatingHandler : DelegatingHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddScoped<TDelegatingHandler>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .AddHttpMessageHandler<TDelegatingHandler>();
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
            this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient);
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/> adding the delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
            this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where TDelegatingHandler : DelegatingHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddScoped<TDelegatingHandler>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .AddHttpMessageHandler<TDelegatingHandler>();
            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler(
            this IXpandableServiceBuilder services,
            Action<HttpClient> configureClient)
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<TDelegatingHandler>(
            this IXpandableServiceBuilder services,
            Action<HttpClient> configureClient)
            where TDelegatingHandler : DelegatingHandler
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, TDelegatingHandler>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler(
            this IXpandableServiceBuilder services,
            Action<IServiceProvider, HttpClient> configureClient)
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token, adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<TDelegatingHandler>(
            this IXpandableServiceBuilder services,
            Action<IServiceProvider, HttpClient> configureClient)
            where TDelegatingHandler : DelegatingHandler
            => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpRestClientRequestBuilder, HttpRestClientResponseBuilder, TDelegatingHandler>(configureClient);

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
            this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<HttpRestClientAuthorizationHandler>();
            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .ConfigurePrimaryHttpMessageHandler<HttpRestClientAuthorizationHandler>();

            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
            this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where TDelegatingHandler : DelegatingHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<HttpRestClientAuthorizationHandler>();
            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddScoped<TDelegatingHandler>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .ConfigurePrimaryHttpMessageHandler<HttpRestClientAuthorizationHandler>()
                .AddHttpMessageHandler<TDelegatingHandler>();

            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
            this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<HttpRestClientAuthorizationHandler>();
            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .ConfigurePrimaryHttpMessageHandler<HttpRestClientAuthorizationHandler>();

            return services;
        }

        /// <summary>
        ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpRestClientHandler"/> type
        ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
        ///  with authorization token, adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpRestClientHandler"/> and you need to register an implementation of <see cref="IHttpRestClientAuthorizationProvider"/> using
        ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
            this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
            where THttpRestClientRequestBuilder : class, IHttpRestClientRequestBuilder
            where THttpRestClientResponseBuilder : class, IHttpRestClientResponseBuilder
            where TDelegatingHandler : DelegatingHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<HttpRestClientAuthorizationHandler>();
            services.Services.AddScoped<IHttpRestClientRequestBuilder, THttpRestClientRequestBuilder>();
            services.Services.AddScoped<IHttpRestClientResponseBuilder, THttpRestClientResponseBuilder>();
            services.Services.AddScoped<TDelegatingHandler>();
            services.Services
                .AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>(configureClient)
                .ConfigurePrimaryHttpMessageHandler<HttpRestClientAuthorizationHandler>()
                .AddHttpMessageHandler<TDelegatingHandler>();

            return services;
        }

        /// <summary>
        /// Adds the specified HTTP Rest Client Authorization Provider that implements the <see cref="IHttpRestClientAuthorizationProvider"/>.
        /// </summary>
        /// <typeparam name="THttpRestClientAuthorizationProvider">The type that implements <see cref="IHttpRestClientAuthorizationProvider"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpRestClientAuthorizationProvider<THttpRestClientAuthorizationProvider>(this IXpandableServiceBuilder services)
            where THttpRestClientAuthorizationProvider : class, IHttpRestClientAuthorizationProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpRestClientAuthorizationProvider, THttpRestClientAuthorizationProvider>();
            return services;
        }

        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpHeaderAccessor">The type of HTTP request header.</typeparam>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpHeaderAccessor<THttpHeaderAccessor>(this IXpandableServiceBuilder services)
            where THttpHeaderAccessor : class, IHttpHeaderAccessor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpHeaderAccessor, THttpHeaderAccessor>();
            return services;
        }

        /// <summary>
        /// The default URI used to retrieve the IP address.
        /// </summary>
        public const string DefaultIPAddressFinderUri = "https://ipinfo.io/ip";

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressAccessor"/> implementation to retrieve the IPAddress of caller from https://ipinfo.io/ip.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpIPAddressAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var descriptor = new ServiceDescriptor(
                typeof(IHttpIPAddressAccessor),
                provider =>
                {
                    var requestBuilder = provider.GetRequiredService<IHttpRestClientRequestBuilder>();
                    var responseBuilder = provider.GetRequiredService<IHttpRestClientResponseBuilder>();
                    var client = new HttpClient(new HttpIPAddressDelegateHandler())
                    {
                        BaseAddress = new Uri(DefaultIPAddressFinderUri)
                    };
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                    return new HttpIPAddressAccessor(new HttpRestClientHandler(requestBuilder, responseBuilder, client));
                },
                ServiceLifetime.Scoped);

            services.Services.Add(descriptor);
            return services;
        }

        /// <summary>
        /// The default URI used to retrieve the IP address location.
        /// </summary>
        public const string DefaultIPAddressLocationFinderUri = "http://api.ipstack.com";

        /// <summary>
        /// Adds an <see cref="IHttpIPAddressLocationAccessor"/> implementation to retrieve the IP Address location from http://api.ipstack.com.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpIPAddressLocationAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var descriptor = new ServiceDescriptor(
                typeof(IHttpIPAddressLocationAccessor),
                provider =>
                {
                    var requestBuilder = provider.GetRequiredService<IHttpRestClientRequestBuilder>();
                    var responseBuilder = provider.GetRequiredService<IHttpRestClientResponseBuilder>();
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(DefaultIPAddressLocationFinderUri)
                    };
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                    return new HttpIPAddressLocationAccessor(new HttpRestClientHandler(requestBuilder, responseBuilder, client));
                },
                ServiceLifetime.Scoped);

            services.Services.Add(descriptor);
            return services;
        }
    }
}
