
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Creators;
using Xpandables.Net.Events;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.EntityFramework
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// The <see cref="DataLogContextProvider{TDataContext, TLogEntity}"/> helper class.
    /// </summary>
    /// <typeparam name="TDataLogContext">The type of data log context to build.</typeparam>
    /// <typeparam name="TLogEntity">The type of log entity to be used.</typeparam>
    public abstract class DataLogContextProvider<TDataLogContext, TLogEntity> : IDataLogContextProvider<TLogEntity>
        where TDataLogContext : DataLogContext<TLogEntity>
        where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
    {
        /// <summary>
        /// Contains the <see cref="DataLogContextSettings"/> instance.
        /// </summary>
        protected readonly DataLogContextSettings DataLogContextSettings;

        /// <summary>
        /// Contains the <see cref="IInstanceCreator"/> instance.
        /// </summary>
        protected readonly IInstanceCreator InstanceCreator;

        /// <summary>
        /// Initializes a new instance of <see cref="DataLogContextProvider{TDataContext, TLogEntity}"/>.
        /// </summary>
        /// <param name="dataLogContextSettings">The data context settings.</param>
        /// <param name="instanceCreator">The instance creator.</param>
        protected DataLogContextProvider(IOptions<DataLogContextSettings> dataLogContextSettings, IInstanceCreator instanceCreator)
        {
            DataLogContextSettings = dataLogContextSettings?.Value ?? throw new ArgumentNullException(nameof(dataLogContextSettings));
            InstanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        /// <summary>
        /// Asynchronously returns an instance that will contain the ambient data log context according to the environment.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An instance of <see cref="IDataLogContext" />.</returns>
        public async Task<IDataLogContext<TLogEntity>> GetDataLogContextAsync(CancellationToken cancellationToken = default)
        {
            var options = GetDataLogContextOptions();
            var dataLogContext = (TDataLogContext)InstanceCreator.Create(typeof(TDataLogContext), options)!;

            if (DataLogContextSettings.EnsuredDeletedBefore && !await dataLogContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException($"Unable to delete database before migration {dataLogContext.Database.ProviderName}");

            if (!DataLogContextSettings.UseInMemory && DataLogContextSettings.ApplyMigrations)
                await dataLogContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

            return dataLogContext;
        }

        /// <summary>
        /// Returns an instance that will contain the ambient data context according to the environment.
        /// </summary>
        /// <returns>An instance of <see cref="IDataLogContext" />.</returns>
        public IDataLogContext<TLogEntity> GetDataLogContext()
        {
            var options = GetDataLogContextOptions();
            var dataLogContext = (TDataLogContext)InstanceCreator.Create(typeof(TDataLogContext), options)!;

            if (DataLogContextSettings.EnsuredDeletedBefore && !dataLogContext.Database.EnsureDeleted())
                throw new InvalidOperationException($"Unable to delete database before migration {dataLogContext.Database.ProviderName}");

            if (!DataLogContextSettings.UseInMemory && DataLogContextSettings.ApplyMigrations)
                dataLogContext.Database.Migrate();

            return dataLogContext;
        }

        /// <summary>
        /// When implemented in derived class, provides with the <see cref="DbContextOptions{TContext}"/> for the target context.
        /// </summary>
        /// <returns>The <see cref="DbContextOptions{TContext}"/> for the target log context.</returns>
        public abstract DbContextOptions<TDataLogContext> GetDataLogContextOptions();
    }
}
