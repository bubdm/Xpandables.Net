
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

using Xpandables.Net.Dispatchers;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Provides method to register services.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default <see cref="IDispatcher"/> implementations to the services with scoped life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXDispatcher(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IDispatcher, Dispatcher>();
        return services;
    }

    /// <summary>
    /// Adds the default <see cref="IHandlerAccessor"/> implementations to the services with scoped life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXHandlerAccessor(this IXpandableServiceBuilder services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IHandlerAccessor, HandlerAccessor>();
        return services;
    }

    /// <summary>
    /// Adds the <typeparamref name="TDomainEventDispatcher"/> as <see cref="IDomainEventDispatcher"/> type implementation to the services with scope life time.
    /// </summary>
    /// <typeparam name="TDomainEventDispatcher">The domain event dispatcher type implementation.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXDomainEventDispatcher<TDomainEventDispatcher>(this IXpandableServiceBuilder services)
        where TDomainEventDispatcher : class, IDomainEventDispatcher
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IDomainEventDispatcher, TDomainEventDispatcher>();
        return services;
    }

    /// <summary>
    /// Adds the default <see cref="IDomainEventDispatcher"/> implementation to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXDomainEventDispatcher(this IXpandableServiceBuilder services)
        => services.AddXDomainEventDispatcher<DomainEventDispatcher>();

    /// <summary>
    /// Adds the <typeparamref name="TNotificationDispatcher"/> as <see cref="INotificationDispatcher"/> type implementation to the services with scope life time.
    /// </summary>
    /// <typeparam name="TNotificationDispatcher">The notification dispatcher type implementation.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXNotificationDispatcher<TNotificationDispatcher>(this IXpandableServiceBuilder services)
        where TNotificationDispatcher : class, INotificationDispatcher
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<INotificationDispatcher, TNotificationDispatcher>();
        return services;
    }

    /// <summary>
    /// Adds the default <see cref="INotificationDispatcher"/> implementation to the services with scope life time.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXNotificationDispatcher(this IXpandableServiceBuilder services)
        => services.AddXNotificationDispatcher<NotificationDispatcher>();
}
