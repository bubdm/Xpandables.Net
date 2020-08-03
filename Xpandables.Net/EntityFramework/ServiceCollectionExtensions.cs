
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

using Xpandables.Net.Commands;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Helpers;
using Xpandables.Net.Queries;

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

            services.AddScoped(async serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IDataContextProvider>();
                var dataContext = await dataContextProvider.GetDataContextAsync().ConfigureAwait(false);

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
        /// Adds persistence behavior to commands and queries that are decorated with the <see cref="IBehaviorPersistence"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXPersistenceBehavior(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandPersistenceBehavior<>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryPersistenceBehavior<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextSeeder{TDataContext}"/> to the services with scoped life time that will be used
        /// to seed every data context that it's decorated with the <see cref="IBehaviorSeed"/> interface.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="dataContextSeederType">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</param>
        /// <param name="dataContextType">The type of data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXSeedBehavior(
            this IServiceCollection services, Type dataContextSeederType, Type dataContextType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            if (!typeof(IDataContextSeeder<>).TryMakeGenericType(out var seederType, out var seederException, dataContextType))
                throw new InvalidOperationException($"Unable to build the {typeof(IDataContextSeeder<>).Name} type.", seederException);

            if (!typeof(DataContextSeederBehavior<>).TryMakeGenericType(out var seederDecoratorType, out var decoException, dataContextType))
                throw new InvalidOperationException($"Unable to build the {typeof(DataContextSeederBehavior<>).Name} type.", decoException);

            services.AddScoped(seederType, dataContextSeederType);
            services.XTryDecorate(typeof(IDataContextProvider), seederDecoratorType!);
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextSeeder{TDataContext}"/> to the services with scoped life time that will be used
        /// to seed every data context that it's decorated with the <see cref="IBehaviorSeed"/> interface.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</typeparam>
        /// <typeparam name="TDataContext">The type of data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXSeedBehavior<TDataContextSeeder, TDataContext>(this IServiceCollection services)
            where TDataContextSeeder : class, IDataContextSeeder<TDataContext>
            where TDataContext : class, IDataContext, IBehaviorSeed
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContextSeeder<TDataContext>, TDataContextSeeder>();
            services.XTryDecorate<IDataContextProvider, DataContextSeederBehavior<TDataContext>>();
            return services;
        }
    }
}
