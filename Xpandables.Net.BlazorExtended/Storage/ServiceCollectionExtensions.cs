
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

using Xpandables.Net.Storage;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds local/session storage with default values.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <remarks>You can use an override to customize behaviors.</remarks>
        public static IXpandableServiceBuilder AddXStorage(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services
                .AddScoped<IStorage, InternalStorage>()
                .AddScoped<ILocalStorage, InternalLocalStorage>()
                .AddScoped<ISessionStorage, InternalSessionStorage>();

            return services;
        }

        /// <summary>
        /// Adds local/session storage with specific event handler.
        /// </summary>
        /// <typeparam name="TStorageEventHandler">The type that implement <see cref="StorageEventHandler"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXStorage<TStorageEventHandler>(this IXpandableServiceBuilder services)
            where TStorageEventHandler : StorageEventHandler
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services
                .AddScoped<TStorageEventHandler>()
                .AddScoped<IStorage, InternalStorage>()
                .AddScoped<InternalLocalStorage>()
                .AddScoped<InternalSessionStorage>()
                .AddScoped<ILocalStorage>(provider =>
                {
                    var localStorage = provider.GetRequiredService<InternalLocalStorage>();
                    var storageEventHandler = provider.GetRequiredService<TStorageEventHandler>();

                    localStorage.Changed += storageEventHandler.OnChanged;
                    localStorage.Changing += storageEventHandler.OnChanging;

                    return localStorage;
                })
                .AddScoped<ISessionStorage>(provider =>
                {
                    var sessionStorage = provider.GetRequiredService<InternalSessionStorage>();
                    var storageEventHandler = provider.GetRequiredService<TStorageEventHandler>();

                    sessionStorage.Changed += storageEventHandler.OnChanged;
                    sessionStorage.Changing += storageEventHandler.OnChanging;

                    return sessionStorage;
                });


            return services;
        }
    }
}
