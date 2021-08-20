
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents a set of methods to read/write objects from a data store.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="orderBy">Defines the property for ordering.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<TEntity?> TryFindAsync<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TEntity"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TEntity> FetchAllAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TEntity"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="orderBy">Defines the property for ordering.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TEntity> FetchAllAsync<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TEntity"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="orderBy">Defines the property for ordering.</param>
        /// <param name="pageIndex">Defines the pagination index.</param>
        /// <param name="pageSize">Defines the pagination size.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        IAsyncEnumerable<TEntity> FetchAllAsync<TKey>(
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize,  CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the 
        /// number of elements in the sequence that satisfy the condition in the predicate function.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the specified entity to be inserted to the data storage on persistence according to the database provider/ORM.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the domain objects matching the predicate as deleted and will be removed according to the database provider/ORM.
        /// </summary>
        /// <param name="predicate">Defines a set of criteria that entity should meet to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
