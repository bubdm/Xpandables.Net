
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// This is the generic db context class that implements <see cref="IDataContext{T}"/> for a specific-type of entity.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public sealed class DataContext<TEntity> : IDataContext<TEntity>
        where TEntity : class
    {
        private readonly IDataContext _dataContext;
        object IDataContext<TEntity>.InternalDbSet<T>() => _dataContext.InternalDbSet<T>();

        private bool _isTracked;
        bool IDataTracker.IsTracked { get => _isTracked; set => _isTracked = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext{T}"/> class
        /// using the original data context to be wrapped.
        /// </summary>
        /// <param name="dataContext">The original data context.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataContext"/> is null.</exception>
        public DataContext(IDataContext dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        /// <summary>
        /// Returns an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public async Task<TResult?> TryFindAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
         => await _dataContext
                .AsTracking(_isTracked)
                .TryFindAsync(selector, cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TResult"/> anonymous type specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public IAsyncEnumerable<TResult> FetchAllAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            => _dataContext
                .AsTracking(_isTracked)
                .FetchAllAsync(selector, cancellationToken);

        /// <summary>
        /// Adds a collection of domain objects to the data storage that will be inserted
        /// into the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <param name="entities">The domain objects collection to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null or empty.</exception>
        public async Task AddEntityRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => await _dataContext.AddEntityRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Adds a domain object to the data storage that will be inserted
        /// into the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        public async Task AddEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dataContext.AddEntityAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Deletes the domain object matching the specified entity that will be removed from the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/>
        /// is called.
        /// </summary>
        /// <param name="deletedEntity">The entity to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="deletedEntity"/> is null.</exception>
        public async Task DeleteEntityAsync(TEntity deletedEntity, CancellationToken cancellationToken = default)
          => await _dataContext.DeleteEntityAsync(deletedEntity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Deletes the domain objects matching the predicate that will be removed from the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/>
        /// is called. You can use a third party library for performance.
        /// </summary>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public async Task DeleteEntityAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
         => await _dataContext.DeleteEntityAsync(predicate, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Updates the domain object matching the specify entity.
        /// </summary>
        /// <param name="updatedEntity">the updated entity.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        public async Task UpdateEntityAsync(TEntity updatedEntity, CancellationToken cancellationToken = default)
           => await _dataContext.UpdateEntityAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Updates the domain objects matching the predicate by using the updater.
        /// Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone. If you have property you want to set to its default,
        /// then you must explicitly set that property's value.
        /// </summary>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="updater">The delegate to be used for updating domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater"/> is null.</exception>
        public async Task UpdateEntityAsync(Expression<Func<TEntity, bool>> predicate, Func<TEntity, TEntity> updater, CancellationToken cancellationToken = default)
            => await _dataContext.UpdateEntityAsync(predicate, updater, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Persists all pending domain objects to the data storage.
        /// You can use the <see cref="OnPersistenceException"/> to manage exception.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task PersistAsync(CancellationToken cancellationToken = default) => await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Allows to set or unset the delegate that get called on persistence exception.
        /// If you want the exception to be re-thrown, the delegate should return an exception, otherwise null.
        /// To disable the delegate, just set the handler to <see langword="null"/>.
        /// </summary>
        public PersistenceExceptionHandler? OnPersistenceException { get => _dataContext.OnPersistenceException; set => _dataContext.OnPersistenceException = value; }
    }
}
