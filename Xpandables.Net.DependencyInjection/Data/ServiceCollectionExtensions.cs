
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

using Xpandables.Net.Data;
using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Elements;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Mappers;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Data.Providers;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Service collection registration methods for <see cref="IDataBase"/>
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all default services for <see cref="IDataBase"/> use.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBase(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataBase, DataBase>();
            services.AddSingleton<IDataFactoryProvider, DataFactoryProvider>();

            services.AddTransient<IDataMapperRow, DataMapperRow>();
            services.AddTransient<IDataMapper, DataMapper>();

            services.AddScoped<IDataExecutableProvider, DataExecutableProvider>();

            services.AddTransient<IDataPropertyBuilder, DataPropertyBuilder>();
            services.AddTransient<IDataEntityBuilder, DataEntityBuilder>();

            services.AddScoped<IDataConnectionContextProvider, DataConnectionContextProvider>();

            services.AddTransient<DataExecutableProcedure>();
            services.AddTransient(typeof(DataExecutableMapperFuncProc<>));
            services.AddTransient(typeof(DataExecutableQuery<>));
            services.AddTransient<DataExecutableTable>();
            services.AddTransient<DataExecutableTransaction>();
            services.AddTransient(typeof(DataExecutableSingle<>));

            return services;
        }

        /// <summary>
        /// Adds all default services for <see cref="IDataBase"/> use.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="DataBaseOptions"/>.</param>///
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBase(this IServiceCollection services, Action<DataBaseOptions> configureOptions)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));

            services.AddSingleton<IDataFactoryProvider, DataFactoryProvider>();

            services.AddTransient<IDataMapperRow, DataMapperRow>();
            services.AddTransient<IDataMapper, DataMapper>();

            services.AddScoped<IDataExecutableProvider, DataExecutableProvider>();

            services.AddTransient<IDataPropertyBuilder, DataPropertyBuilder>();
            services.AddTransient<IDataEntityBuilder, DataEntityBuilder>();

            services.AddScoped<IDataConnectionContextProvider, DataConnectionContextProvider>();

            services.AddTransient<DataExecutableProcedure>();
            services.AddTransient(typeof(DataExecutableMapperFuncProc<>));
            services.AddTransient(typeof(DataExecutableQuery<>));
            services.AddTransient<DataExecutableTable>();
            services.AddTransient<DataExecutableTransaction>();
            services.AddTransient(typeof(DataExecutableSingle<>));

            var definedOptions = new DataBaseOptions();
            configureOptions.Invoke(definedOptions);

            _ = (definedOptions.IsDataConnectionEnabled, definedOptions.IsDataOptionsEnabled) switch
            {
                (IDataConnection dataConnection, IDataOptions dataOptions) => services.AddScoped<IDataBase>(provider =>
                  {
                      var executableProvider = provider.GetRequiredService<IDataExecutableProvider>();
                      var contextProvider = provider.GetRequiredService<IDataConnectionContextProvider>();

                      return new DataBase(contextProvider, executableProvider, dataConnection, dataOptions);
                  }),
                (IDataConnection dataConnection, _) => services.AddScoped<IDataBase>(provider =>
                  {
                      var executableProvider = provider.GetRequiredService<IDataExecutableProvider>();
                      var contextProvider = provider.GetRequiredService<IDataConnectionContextProvider>();

                      return new DataBase(contextProvider, executableProvider, dataConnection);
                  }),
                (_, IDataOptions dataOptions) => services.AddScoped<IDataBase>(provider =>
                {
                    var executableProvider = provider.GetRequiredService<IDataExecutableProvider>();
                    var contextProvider = provider.GetRequiredService<IDataConnectionContextProvider>();

                    return new DataBase(contextProvider, executableProvider, dataOptions);
                }),
                (_, _) => services.AddScoped<IDataBase>(provider =>
                {
                    var executableProvider = provider.GetRequiredService<IDataExecutableProvider>();
                    var contextProvider = provider.GetRequiredService<IDataConnectionContextProvider>();

                    return new DataBase(contextProvider, executableProvider);
                })
            };

            return services;
        }
    }
}
