/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// This is the <see langword="abstract"/> db context class that inherits from <see cref="DbContext"/>
    /// and implements <see cref="IDataContext"/>.
    /// </summary>
    public abstract partial class DataContext : IDataContext
    {
        object IDataContext.InternalDbSet<T>() => Set<T>();

        /// <summary>
        /// Returns an entity of the <typeparamref name="T"/> type specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="T"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public virtual async Task<T?> FindAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken = default)
            where T : Entity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public virtual async Task<TResult?> FindAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="T"/> entities specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="T"/> that can be asynchronously enumerable.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public IAsyncEnumerable<T> FindAllAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken = default)
            where T : Entity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).AsAsyncEnumerable();
        }

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TResult"/> anonymous type specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerable.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public IAsyncEnumerable<TResult> FindAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).AsAsyncEnumerable();
        }

        /// <summary>
        /// Adds a domain object to the data storage that will be inserted
        /// into the database when <see cref="PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        public virtual async Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : Entity
            => await Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Adds a collection of domain objects to the data storage that will be inserted
        /// into the database when <see cref="IDataContext.PersistAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="entities">The domain objects collection to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities" /> is null or empty.</exception>
        public virtual async Task AddEntityRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : Entity
        {
            if (entities?.Any() != true)
                throw new ArgumentNullException(nameof(entities));

            await AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the domain object matching the specified entity that will be removed from the database when <see cref="PersistAsync(CancellationToken)"/>
        /// is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="deletedEntity">The entity to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="deletedEntity"/> is null.</exception>
        public virtual async Task DeleteEntityAsync<T>(T deletedEntity, CancellationToken cancellationToken = default)
            where T : Entity
        {
            if (deletedEntity is null) throw new ArgumentNullException(nameof(deletedEntity));
            cancellationToken.ThrowIfCancellationRequested();
            Remove(deletedEntity);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the domain objects matching the predicate that will be removed from the database when <see cref="IDataContext.PersistAsync(CancellationToken)" />
        /// is called. You can use a third party library with <see langword="IDataContext.SetOf{T}" /> for performance.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is null.</exception>
        public virtual async Task DeleteEntityAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            foreach (var entity in Set<T>().Where(predicate))
            {
                cancellationToken.ThrowIfCancellationRequested();
                Remove(entity);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the domain object matching the specify entity.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="updatedEntity">the updated entity.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        public virtual async Task UpdateEntityAsync<T>(T updatedEntity, CancellationToken cancellationToken = default)
            where T : Entity
        {
            if (updatedEntity is null) throw new ArgumentNullException(nameof(updatedEntity));
            cancellationToken.ThrowIfCancellationRequested();
            Update(updatedEntity);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the domain objects matching the collection of entities.
        /// Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone. If you have property you want to set to its default,
        /// then you must explicitly set that property's value.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedEntities">Contains the collection of updated values.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntities" /> is null.</exception>
        public virtual async Task UpdateEntityRangeAsync<T, TUpdated>(
            IEnumerable<TUpdated> updatedEntities, CancellationToken cancellationToken = default)
            where T : Entity
            where TUpdated : Entity
        {
            if (updatedEntities?.Any() != true)
                throw new ArgumentNullException(nameof(updatedEntities));

            foreach (var updatedEntity in updatedEntities)
            {
                if (await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(Set<T>(),
                    entity => entity.Id == updatedEntity.Id, cancellationToken)
                    .ConfigureAwait(false) is T entity)
                {
                    Entry(entity).CurrentValues.SetValues(updatedEntity);
                    Entry(entity).State = EntityState.Modified;
                }
            }
        }

        /// <summary>
        /// Updates the domain objects matching the predicate by using the updater.
        /// Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone. If you have property you want to set to its default,
        /// then you must explicitly set that property's value.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="updater">The delegate to be used for updating domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater" /> is null.</exception>
        public virtual async Task UpdateEntityAsync<T, TUpdated>(
            Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater, CancellationToken cancellationToken = default)
            where T : Entity
            where TUpdated : class
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            foreach (var entity in Set<T>().Where(predicate))
            {
                cancellationToken.ThrowIfCancellationRequested();
                Entry(entity).CurrentValues.SetValues(updater(entity));
                Entry(entity).State = EntityState.Modified;
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Persists all pending domain objects to the data storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async Task PersistAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is DbUpdateException)
            {
                ExceptionHandler(exception);
            }
        }

        [DebuggerStepThrough]
        private void ExceptionHandler(Exception exception)
        {
            var rethrowException = OnPersistenceException is null ? exception : OnPersistenceException.Invoke(exception);

            if (rethrowException is not null)
            {
                throw new InvalidOperationException(
                    "Persistence operation failed. See inner exception.",
                    exception);
            }
        }

        /// <summary>
        /// Allows to set or unset the delegate that get called on persistence exception.
        /// If you want the exception to be re-thrown, the delegate should return an exception, otherwise null.
        /// To disable the delegate, just set the handler to <see langword="null"/>.
        /// </summary>
        public PersistenceExceptionHandler? OnPersistenceException { get; set; }
    }
}
