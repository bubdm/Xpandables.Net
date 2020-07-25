
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
using Xpandables.Net.Data.Elements;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Mappers;

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
        /// Adds all default services for <see cref="DataBase"/> use.
        /// </summary>
        /// <typeparam name="TDataConnectionProvider">The data connection provider type.</typeparam>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataBase<TDataConnectionProvider>(this IServiceCollection services)
            where TDataConnectionProvider : class, IDataConnectionProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IDataConnectionProvider, TDataConnectionProvider>();

            services.AddScoped<IDataFactoryProvider, DataFactoryProvider>();
            services.AddTransient<IDataPropertyBuilder, DataPropertyBuilder>();
            services.AddTransient<IDataEntityBuilder, DataEntityBuilder>();
            services.AddTransient<IDataMapperRow, DataMapperRow>();
            services.AddScoped<IDataMapper, DataMapper>();
            services.AddScoped<IDataBase, DataBase>();

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
