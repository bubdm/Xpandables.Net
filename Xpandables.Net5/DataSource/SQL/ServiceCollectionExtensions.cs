
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
using System.Design.DataSource.SQL;
using System.Design.DataSource.SQL.Executables;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Service collection registration methods for <see cref="DataBase"/>
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all default services for <see cref="DataBaseContext"/> use.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBaseContext(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddXDataBaseServices();
            services.AddXDataBaseContextAccessors();
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
            //services.AddScoped<DataMapperRow>();
            return services;
        }

        /// <summary>
        /// Adds the services to build the <see cref="DataBaseContext"/>.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBaseContextAccessors(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataProviderFactoryProvider, DataProviderFactoryProvider>();
            services.AddScoped<DataBaseContextAccessor>();
            services.AddScoped<DataConnectionAccessor>();
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
