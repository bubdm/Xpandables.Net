﻿
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

using Xpandables.Net.EntityFramework;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    ///  Provides method to register data context.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContextProvider">The type of data context accessor
        /// that implements <see cref="IDataContextProvider"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContextProvider>(this IServiceCollection services)
            where TDataContextProvider : class, IDataContextProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddXDataContext(typeof(TDataContextProvider));
        }

        /// <summary>
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time.
        /// </summary>
        /// <param name="dataContextProviderType">The data context provider type.</param>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContextProviderType"/> is null.</exception>
        public static IServiceCollection AddXDataContext(this IServiceCollection services, Type dataContextProviderType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = dataContextProviderType ?? throw new ArgumentNullException(nameof(dataContextProviderType));

            return services.AddXDataContext(dataContextProviderType, _ => { });
        }

        /// <summary>
        /// Adds the <see cref="IDataContextProvider"/> to the services with scope life time.
        /// </summary>
        /// <typeparam name="TDataContextProvider">The type of the implementation.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextProvider<TDataContextProvider>(this IServiceCollection services)
            where TDataContextProvider : class, IDataContextProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContextProvider, TDataContextProvider>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextProvider"/> to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="dataContextProviderType">The data context provider type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContextProviderType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dataContextProviderType"/> must implement the <see cref="IDataContextProvider"/> interface.</exception>
        public static IServiceCollection AddXDataContextProvider(this IServiceCollection services, Type dataContextProviderType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = dataContextProviderType ?? throw new ArgumentNullException(nameof(dataContextProviderType));
            if (!typeof(IDataContextProvider).IsAssignableFrom(dataContextProviderType))
                throw new ArgumentException($"{nameof(dataContextProviderType)} must implement {nameof(IDataContextProvider)}.");

            services.AddScoped(typeof(IDataContextProvider), dataContextProviderType);
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time using options.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="dataContextProviderType">The data context provider type.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="DataContextOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContextProviderType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dataContextProviderType"/> must implement <see cref="IDataContextProvider"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        public static IServiceCollection AddXDataContext(
            this IServiceCollection services, Type dataContextProviderType, Action<DataContextOptions> configureOptions)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            _ = dataContextProviderType ?? throw new ArgumentNullException(nameof(dataContextProviderType));

            if (!typeof(IDataContextProvider).IsAssignableFrom(dataContextProviderType))
                throw new ArgumentException($"{nameof(dataContextProviderType)} must implement {nameof(IDataContextProvider)}.");

            services.AddScoped(typeof(IDataContextProvider), dataContextProviderType);
            var definedOptions = new DataContextOptions();
            configureOptions.Invoke(definedOptions);

            services.AddScoped(serviceProvider =>
           {
               var dataContextProvider = serviceProvider.GetRequiredService<IDataContextProvider>();
               var dataContext = dataContextProvider.GetDataContext();

               dataContext.OnPersistenceException(definedOptions.IsPersistenceExceptionHandlerEnabled);
               return dataContext;
           });

            if (definedOptions.IsSeederEnabled.HasValue)
            {
                services.AddXSeedBehavior(
                    definedOptions.IsSeederEnabled.Value.DataContextSeederType,
                    definedOptions.IsSeederEnabled.Value.DataContextType);
            }

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time using options.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="DataContextOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContextProvider>(
            this IServiceCollection services, Action<DataContextOptions> configureOptions)
            where TDataContextProvider : class, IDataContextProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));

            services.AddXDataContext(typeof(TDataContextProvider), configureOptions);
            return services;
        }
    }
}