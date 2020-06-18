
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
using System.Design.Behaviors;
using System.Design.ORM;

namespace System.Design.DependencyInjection
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
            return services.AddXDataContext<TDataContextProvider>(_ => { });
        }

        /// <summary>
        /// Adds the <see cref="IDataContextSeeder{TDataContext}"/> to the services with scoped life time that will be used
        /// to seed every data context that it's decorated with the <see cref="IBehaviorDataSeed"/> interface.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="dataContextSeederType">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</param>
        /// <param name="dataContextType">The type of data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextSeederBehavior(
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
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time using options.
        /// </summary>
        /// <typeparam name="TDataContextProvider">The type of data context accessor
        /// that implements <see cref="IDataContextProvider"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="DataContextOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContextProvider>(
            this IServiceCollection services, Action<DataContextOptions> configureOptions)
            where TDataContextProvider : class, IDataContextProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.AddScoped<IDataContextProvider, TDataContextProvider>();
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
                services.AddXDataContextSeederBehavior(
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
        /// to seed every data context that it's decorated with the <see cref="IBehaviorDataSeed"/> interface.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</typeparam>
        /// <typeparam name="TDataContext">The type of data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextSeederBehavior<TDataContextSeeder, TDataContext>(this IServiceCollection services)
            where TDataContextSeeder : class, IDataContextSeeder<TDataContext>
            where TDataContext : class, IDataContext, IBehaviorDataSeed
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContextSeeder<TDataContext>, TDataContextSeeder>();
            services.XTryDecorate<IDataContextProvider, DataContextSeederBehavior<TDataContext>>();
            return services;
        }
    }
}
