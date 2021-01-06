
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Represents a set of methods to write objects to a data source.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public interface IWriteEntityAccessor<TEntity> : IDisposable
        where TEntity : Entity
    {
        /// <summary>
        /// Marks the specified entity to be inserted to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the specified entity to be updated to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
