
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
using Xpandables.Net.Events;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.EntityFramework
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// The <see cref="DataLogContextDesignTimeFactory{TDataLogContext, TLogEntity, TDataLogContextProvider}"/> helper class.
    /// </summary>
    /// <typeparam name="TDataLogContext">The type of data context to build.</typeparam>
    /// <typeparam name="TLogEntity">The type of data context settings to be used.</typeparam>
    /// <typeparam name="TDataLogContextProvider">The data context provider.</typeparam>
    public abstract class DataLogContextDesignTimeFactory<TDataLogContext, TLogEntity, TDataLogContextProvider> : IDesignTimeDbContextFactory<TDataLogContext>
        where TDataLogContext : DataLogContext<TLogEntity>
        where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
        where TDataLogContextProvider : DataLogContextProvider<TDataLogContext, TLogEntity>
    {
        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args"> Arguments provided by the design-time service.</param>
        /// <returns>An instance of TContext.</returns>
        public virtual TDataLogContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.Configure<DataLogContextSettings>(config.GetSection(typeof(DataLogContextSettings).Name));
            services.AddSingleton<IInstanceCreator, InstanceCreator>();
            services.AddTransient<IDataLogContextProvider<TLogEntity>, TDataLogContextProvider>();

            using var serviceprovider = services.BuildServiceProvider();
            var dataContextProvider = serviceprovider.GetRequiredService<IDataLogContextProvider<TLogEntity>>();
            var dataContext = dataContextProvider.GetDataLogContext();
            return (TDataLogContext)dataContext;
        }
    }
}
