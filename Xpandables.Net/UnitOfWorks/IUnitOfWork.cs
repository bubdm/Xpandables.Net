
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
using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks;

/// <summary>
/// Provides with the base unit of work interface.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Persists all pending domain objects to the data storage according to the database provider/ORM.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous persist all operation.</returns>
    /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    Task<int> PersistAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the repository implementation that matches the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the target entity.</typeparam>
    /// <returns>An implementation of <see cref="IRepository{TEntity}"/>.</returns>
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
}
