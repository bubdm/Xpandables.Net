
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

using Xpandables.Net.Http;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Provides method to register services.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IHttpClientRequestBuilder"/> default implementation to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> services.</returns>
    public static IXpandableServiceBuilder AddXHttpRestClientRequestBuilder(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        services.Services.AddScoped<IHttpClientRequestBuilder, HttpClientRequestBuilder>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="IHttpClientResponseBuilder"/> default implementation to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> services.</returns>
    public static IXpandableServiceBuilder AddXHttpRestClientResponseBuilder(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        services.Services.AddScoped<IHttpClientResponseBuilder, HttpClientResponseBuilder>();
        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler(this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        => services.AddXHttpRestClientHandler<HttpClientRequestBuilder, HttpClientResponseBuilder>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<TDelegatingHandler>(this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        where TDelegatingHandler : DelegatingHandler
        => services.AddXHttpRestClientHandler<HttpClientRequestBuilder, HttpClientResponseBuilder, TDelegatingHandler>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler(this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        => services.AddXHttpRestClientHandler<HttpClientRequestBuilder, HttpClientResponseBuilder>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<TDelegatingHandler>(this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        where TDelegatingHandler : DelegatingHandler
        => services.AddXHttpRestClientHandler<HttpClientRequestBuilder, HttpClientResponseBuilder, TDelegatingHandler>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
        this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddHttpClient<IHttpClientDispatcher, Http.HttpClientDispatcher>(configureClient);
        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/> adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
        this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
        where TDelegatingHandler : DelegatingHandler
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddScoped<TDelegatingHandler>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .AddHttpMessageHandler<TDelegatingHandler>();
        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
        this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddHttpClient<IHttpClientDispatcher, Http.HttpClientDispatcher>(configureClient);
        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to the collection and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/> adding the delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/>.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
        this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
        where TDelegatingHandler : DelegatingHandler
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddScoped<TDelegatingHandler>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .AddHttpMessageHandler<TDelegatingHandler>();
        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
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
        => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpClientRequestBuilder, HttpClientResponseBuilder>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
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
        => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpClientRequestBuilder, HttpClientResponseBuilder, TDelegatingHandler>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
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
        => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpClientRequestBuilder, HttpClientResponseBuilder>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token, adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
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
        => services.AddXHttpRestClientHandlerWithAuthorizationTokenHandler<HttpClientRequestBuilder, HttpClientResponseBuilder, TDelegatingHandler>(configureClient);

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
    ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
        this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<HttpClientAuthorizationHandler>();
        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .ConfigurePrimaryHttpMessageHandler<HttpClientAuthorizationHandler>();

        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
    ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
        this IXpandableServiceBuilder services, Action<HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
        where TDelegatingHandler : DelegatingHandler
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<HttpClientAuthorizationHandler>();
        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddScoped<TDelegatingHandler>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .ConfigurePrimaryHttpMessageHandler<HttpClientAuthorizationHandler>()
            .AddHttpMessageHandler<TDelegatingHandler>();

        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
    ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder>(
        this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<HttpClientAuthorizationHandler>();
        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .ConfigurePrimaryHttpMessageHandler<HttpClientAuthorizationHandler>();

        return services;
    }

    /// <summary>
    ///  Adds the <see cref="IHttpClientFactory"/> and related services to <see cref="IServiceCollection"/> and configures a binding between the default implementation of <see cref="IHttpClientDispatcher"/> type
    ///  and a named <see cref="HttpClient"/>, and adds a delegate that will be used to configure the primary <see cref="HttpMessageHandler"/> for a named <see cref="HttpClient"/> for providing
    ///  with authorization token, adding the specified delegating handler. The client name will be set to the type name of <see cref="IHttpClientDispatcher"/> and you need to register an implementation of <see cref="IHttpClientAuthorizationProvider"/> using
    ///  the <see cref="AddXHttpRestClientAuthorizationProvider{THttpRestClientAuthorizationProvider}(IXpandableServiceBuilder)"/> method.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="configureClient"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientHandlerWithAuthorizationTokenHandler<THttpRestClientRequestBuilder, THttpRestClientResponseBuilder, TDelegatingHandler>(
        this IXpandableServiceBuilder services, Action<IServiceProvider, HttpClient> configureClient)
        where THttpRestClientRequestBuilder : class, IHttpClientRequestBuilder
        where THttpRestClientResponseBuilder : class, IHttpClientResponseBuilder
        where TDelegatingHandler : DelegatingHandler
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<HttpClientAuthorizationHandler>();
        services.Services.AddScoped<IHttpClientRequestBuilder, THttpRestClientRequestBuilder>();
        services.Services.AddScoped<IHttpClientResponseBuilder, THttpRestClientResponseBuilder>();
        services.Services.AddScoped<TDelegatingHandler>();
        services.Services
            .AddHttpClient<IHttpClientDispatcher, HttpClientDispatcher>(configureClient)
            .ConfigurePrimaryHttpMessageHandler<HttpClientAuthorizationHandler>()
            .AddHttpMessageHandler<TDelegatingHandler>();

        return services;
    }

    /// <summary>
    /// Adds the specified HTTP Rest Client Authorization Provider that implements the <see cref="IHttpClientAuthorizationProvider"/>.
    /// </summary>
    /// <typeparam name="THttpRestClientAuthorizationProvider">The type that implements <see cref="IHttpClientAuthorizationProvider"/>.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpRestClientAuthorizationProvider<THttpRestClientAuthorizationProvider>(this IXpandableServiceBuilder services)
        where THttpRestClientAuthorizationProvider : class, IHttpClientAuthorizationProvider
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpClientAuthorizationProvider, THttpRestClientAuthorizationProvider>();
        return services;
    }

    /// <summary>
    /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpHeaderReader"/>.
    /// </summary>
    /// <typeparam name="THttpHeaderAccessor">The type of HTTP request header.</typeparam>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <param name="services">The collection of services.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpHeaderAccessor<THttpHeaderAccessor>(this IXpandableServiceBuilder services)
        where THttpHeaderAccessor : class, IHttpHeaderReader
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHttpHeaderReader, THttpHeaderAccessor>();
        return services;
    }

    /// <summary>
    /// The default URI used to retrieve the IP address.
    /// </summary>
    public const string DefaultIPAddressFinderUri = "https://ipinfo.io/ip";

    /// <summary>
    /// Adds an <see cref="IHttpIPAddressReader"/> implementation to retrieve the IPAddress of caller from https://ipinfo.io/ip.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpIPAddressAccessor(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        var descriptor = new ServiceDescriptor(
            typeof(IHttpIPAddressReader),
            provider =>
            {
                var requestBuilder = provider.GetRequiredService<IHttpClientRequestBuilder>();
                var responseBuilder = provider.GetRequiredService<IHttpClientResponseBuilder>();
                var client = new HttpClient(new HttpIPAddressDelegateHandler())
                {
                    BaseAddress = new Uri(DefaultIPAddressFinderUri)
                };
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                return new HttpIPAddressReader(new Http.HttpClientDispatcher(requestBuilder, responseBuilder, client));
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
    /// Adds an <see cref="IHttpIPLocationReader"/> implementation to retrieve the IP Address location from http://api.ipstack.com.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHttpIPAddressLocationAccessor(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        var descriptor = new ServiceDescriptor(
            typeof(IHttpIPLocationReader),
            provider =>
            {
                var requestBuilder = provider.GetRequiredService<IHttpClientRequestBuilder>();
                var responseBuilder = provider.GetRequiredService<IHttpClientResponseBuilder>();
                var client = new HttpClient
                {
                    BaseAddress = new Uri(DefaultIPAddressLocationFinderUri)
                };
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
                return new HttpIPLocationReader(new Http.HttpClientDispatcher(requestBuilder, responseBuilder, client));
            },
            ServiceLifetime.Scoped);

        services.Services.Add(descriptor);
        return services;
    }
}
