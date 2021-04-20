
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Expressions.Specifications;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Represents a set of commands to manage storage domain objects.
    /// When argument is null, an <see cref="ArgumentNullException"/> will be thrown.
    /// When a value is not found, a default value of the expected type should be returned or an empty collection if necessary.
    /// </summary>
    public interface IDataContext : IDisposable, IAsyncDisposable, IDataContextEventTracker
    {
        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TResult"/> type specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        Task<TResult?> TryFindAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TResult"/> anonymous type specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        IAsyncEnumerable<TResult> FetchAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        /// <summary>
        /// Adds a domain object to the data storage that will be inserted according to the database provider/ORM.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        /// <summary>
        /// Deletes the domain objects matching the predicate that will be removed according to the database provider/ORM.
        /// You can use a third party library for performance.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task DeleteAsync<T>(Specification<T> predicate, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        /// <summary>
        /// Updates the domain object matching the specify entity that will be persisted according to the database provider/ORM.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="updatedEntity">the updated entity.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task UpdateAsync<T>(T updatedEntity, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;
    }

}