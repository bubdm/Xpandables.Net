
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
using Microsoft.Extensions.Hosting;

using Xpandables.Net.Services;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Provides method to register Email objects.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <typeparamref name="TEmailService"/> as <see cref="IEmailService"/> type implementation to the services with singleton life time.
    /// </summary>
    /// <typeparam name="TEmailService">The email service type implementation.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXEmailService<TEmailService>(this IXpandableServiceBuilder services)
        where TEmailService : class, IHostedService, IEmailService
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddSingleton<IEmailService, TEmailService>();
        services.Services.AddHostedService(provider => provider.GetRequiredService<IEmailService>() as TEmailService);
        return services;
    }

    /// <summary>
    /// Adds the <typeparamref name="TSmsService"/> as <see cref="ISmsService"/> type implementation to the services with singleton life time.
    /// </summary>
    /// <typeparam name="TSmsService">The sms service type implementation.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXSmsService<TSmsService>(this IXpandableServiceBuilder services)
        where TSmsService : class, IHostedService, ISmsService
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddSingleton<ISmsService, TSmsService>();
        services.Services.AddHostedService(provider => provider.GetRequiredService<ISmsService>() as TSmsService);
        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="TMessage"/> and <typeparamref name="TEmailSender"/> implementation as <see cref="IEmailSender"/>
    /// to the services with scope life time.
    /// </summary>
    /// <typeparam name="TMessage">the type of message.</typeparam>
    /// <typeparam name="TEmailSender">The type of the email sender.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXEmailSender<TMessage, TEmailSender>(
        this IXpandableServiceBuilder services)
        where TMessage : class
        where TEmailSender : class, IEmailSender<TMessage>
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IEmailSender<TMessage>, TEmailSender>();
        services.Services.AddScoped(provider => (IEmailSender)provider.GetRequiredService<IEmailSender<TMessage>>());
        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="TMessage"/> and <typeparamref name="TSmsSender"/> implementation  as <see cref="ISmsSender"/>
    /// to the services with scope life time.
    /// </summary>
    /// <typeparam name="TMessage">the type of message.</typeparam>
    /// <typeparam name="TSmsSender">The type of the sms sender.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXSmsSender<TMessage, TSmsSender>(
        this IXpandableServiceBuilder services)
        where TMessage : class
        where TSmsSender : class, ISmsSender<TMessage>
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<ISmsSender<TMessage>, TSmsSender>();
        services.Services.AddScoped(provider => (ISmsSender)provider.GetRequiredService<ISmsSender<TMessage>>());
        return services;
    }
}
