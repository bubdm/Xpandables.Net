
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

using Xpandables.Net.Data;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.DependencyInjection.Data;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Service collection registration methods for <see cref="DataBaseCommon"/>
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all default services for <see cref="DataBase"/> use.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBase(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddXDataBaseServices();
            services.AddXDataBaseAccessors();
            services.AddXDataBaseExecutables();
            return services;
        }

        /// <summary>
        /// Adds all services need to work with data properties and entities.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBaseServices(this IServiceCollection services)
        {
            services.AddScoped<DataPropertyBuilder>();
            services.AddScoped<DataEntityBuilder>();
            services.AddScoped<DataMapper>();
            return services;
        }

        /// <summary>
        /// Adds the services to build the <see cref="DataBase"/>.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBaseAccessors(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataProviderFactoryProvider, DataProviderFactoryProvider>();
            services.AddScoped<DataBaseAccessor>();
            services.AddScoped<IDataConnectionAccessor, DataConnectionAccessor>();
            return services;
        }

        /// <summary>
        /// Adds all the default <see cref="DataExecutable{T}"/> implementations.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBaseExecutables(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<DataExecutableProcedure>();
            services.AddTransient(typeof(DataExecutableMapper<>));
            services.AddTransient(typeof(DataExecutableQuery<>));
            services.AddTransient<DataExecutableTable>();
            services.AddTransient<DataExecutableTransaction>();
            services.AddTransient(typeof(DataExecutableSingle<>));

            return services;
        }
    }
}
