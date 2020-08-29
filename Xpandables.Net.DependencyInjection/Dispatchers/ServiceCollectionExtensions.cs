
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

using Xpandables.Net.Dispatchers;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register dispatcher.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IDispatcher"/> implementation to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDispatcher(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDispatcherHandlerProvider, DispatcherHandlerProvider>();
            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDispatcher"/> type to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDispatcher">The dispatcher type implementation.</typeparam>
        /// <typeparam name="TDispatcherHandlerProvider">The dispatcher handler provider type implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDispatcher<TDispatcher, TDispatcherHandlerProvider>(this IServiceCollection services)
            where TDispatcher : class, IDispatcher
            where TDispatcherHandlerProvider : class, IDispatcherHandlerProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDispatcherHandlerProvider, TDispatcherHandlerProvider>();
            services.AddScoped<IDispatcher, TDispatcher>();
            return services;
        }
    }
}
