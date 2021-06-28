
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

using Xpandables.Net.Storage;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds local storage with default values.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <remarks>You can use an override to customize behaviors.</remarks>
        public static IXpandableServiceBuilder AddXLocalStorage(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services
                .AddScoped<ILocalStorageEngine, LocalStorageEngine>()
                .AddScoped<ILocalStorageProvider, LocalStorageProvider>()
                .AddScoped<ILocalStorageSerializer, LocalStorageSerializer>();

            return services;
        }

        /// <summary>
        /// Adds local storage with the specific events hander.
        /// </summary>
        /// <typeparam name="TLocalStorageEventHandler">The type that implement <see cref="LocalStorageEventHandler"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <remarks>You can use an override to customize behaviors.</remarks>
        public static IXpandableServiceBuilder AddXLocalStorage<TLocalStorageEventHandler>(this IXpandableServiceBuilder services)
            where TLocalStorageEventHandler : LocalStorageEventHandler
        {
            return services.AddXLocalStorage<LocalStorageSerializer, TLocalStorageEventHandler>();
        }

        /// <summary>
        /// Adds local storage with specifics serializer and events handler.
        /// </summary>
        /// <typeparam name="TLocalStorageSerializer">The type that implement <see cref="ILocalStorageSerializer"/>.</typeparam>
        /// <typeparam name="TLocalStorageEventHandler">The type that implement <see cref="LocalStorageEventHandler"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXLocalStorage<TLocalStorageSerializer, TLocalStorageEventHandler>(this IXpandableServiceBuilder services)
            where TLocalStorageEventHandler : LocalStorageEventHandler
            where TLocalStorageSerializer : class, ILocalStorageSerializer
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services
                .AddScoped<TLocalStorageEventHandler>()
                .AddScoped<LocalStorageEngine>()
                .AddScoped<ILocalStorageEngine>(provider =>
                {
                    var localStorageEngine = provider.GetRequiredService<LocalStorageEngine>();
                    var localStorageEventHandler = provider.GetRequiredService<TLocalStorageEventHandler>();

                    localStorageEngine.Changed += localStorageEventHandler.OnChanged;
                    localStorageEngine.Changing += localStorageEventHandler.OnChanging;

                    return localStorageEngine;
                })
                .AddScoped<ILocalStorageProvider, LocalStorageProvider>()
                .AddScoped<ILocalStorageSerializer, TLocalStorageSerializer>();

            return services;
        }
    }
}
