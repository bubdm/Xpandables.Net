
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
using System.IO;

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Creators;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// The <see cref="DataContextDesignTimeFactory{TDataContext, TDataContextProvider}"/> helper class.
    /// </summary>
    /// <typeparam name="TDataContext">The type of data context to build.</typeparam>
    /// <typeparam name="TDataContextProvider">The data context provider.</typeparam>
    public abstract class DataContextDesignTimeFactory<TDataContext, TDataContextProvider> : IDesignTimeDbContextFactory<TDataContext>
        where TDataContext : DataContext
        where TDataContextProvider : DataContextProvider<TDataContext>
    {
        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args"> Arguments provided by the design-time service.</param>
        /// <returns>An instance of TContext.</returns>
        public virtual TDataContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.Configure<DataContextSettings>(config.GetSection(typeof(DataContextSettings).Name));
            services.AddSingleton<IInstanceCreator, InstanceCreator>();
            services.AddTransient<IDataContextProvider, TDataContextProvider>();

            using var serviceprovider = services.BuildServiceProvider();
            var dataContextProvider = serviceprovider.GetRequiredService<IDataContextProvider>();
            var dataContext = dataContextProvider.GetDataContext();
            return (TDataContext)dataContext;
        }
    }
}
