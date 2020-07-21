
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System;

using Xpandables.Net.Creators;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// The <see cref="DataContextProvider{TDataContext, TDataContextSettings}"/> helper class.
    /// </summary>
    /// <typeparam name="TDataContext">The type of data context to build.</typeparam>
    /// <typeparam name="TDataContextSettings">The type of data context settings to be used.</typeparam>
    public abstract class DataContextProvider<TDataContext, TDataContextSettings> : IDataContextProvider
        where TDataContext : DataContext
        where TDataContextSettings : DataContextSettings, new()
    {
        /// <summary>
        /// Contains the <see cref="DataContextSettings"/> instance.
        /// </summary>
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly TDataContextSettings DataContextSettings;

        /// <summary>
        /// Contains the <see cref="IInstanceCreator"/> instance.
        /// </summary>
        protected readonly IInstanceCreator InstanceCreator;
#pragma warning restore CA1051 // Do not declare visible instance fields

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextProvider{TDataContext, TDataContextSettings}"/>.
        /// </summary>
        /// <param name="dataContextSettings">The data context settings.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        protected DataContextProvider(IOptions<TDataContextSettings> dataContextSettings, IInstanceCreator instanceCreator)
        {
            DataContextSettings = dataContextSettings?.Value ?? throw new ArgumentNullException(nameof(dataContextSettings));
            InstanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        /// <summary>
        /// Returns an instance that contains the ambient data context according to the environment.
        /// </summary>
        /// <returns>An instance of <see cref="IDataContext" />.</returns>
        public IDataContext GetDataContext()
        {
            var options = GetDataContextOptions();
            var dataContext = (TDataContext)InstanceCreator.Create(typeof(TDataContext), options)!;

            if (DataContextSettings.EnsuredDeletedBefore && !dataContext.Database.EnsureDeleted())
                throw new InvalidOperationException($"Unable to delete database before migration {dataContext.Database.ProviderName}");

            if (!DataContextSettings.UseInMemory && DataContextSettings.ApplyMigrations)
                dataContext.Database.Migrate();

            return dataContext;
        }

        /// <summary>
        /// When implemented in derived class, provides with the <see cref="DbContextOptions{TContext}"/> for the target context.
        /// </summary>
        /// <returns>The <see cref="DbContextOptions{TContext}"/> for the target context.</returns>
        public abstract DbContextOptions<TDataContext> GetDataContextOptions();
    }
}
