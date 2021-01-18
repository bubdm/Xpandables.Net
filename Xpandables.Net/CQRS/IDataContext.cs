
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Represents a method used to handle persistence exception.
    /// If you want the exception to be re-thrown, the delegate should return an exception, otherwise null exception.
    /// If there's not delegate, the handled exception will be re-thrown normally.
    /// </summary>
    /// <param name="exception">The handled exception during persistence.</param>
    /// <returns>An exception to re-throw or null if not.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "ET001:Type name does not match file name", Justification = "<Pending>")]
    public delegate Exception? PersistenceExceptionHandler(Exception exception);

    /// <summary>
    /// Represents a set of commands to manage domain objects using EntityFrameworkCore.
    /// When argument is null, an <see cref="ArgumentNullException"/> will be thrown.
    /// When a value is not found, a default value of the expected type should be returned.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IDataContext : IDisposable, IAsyncDisposable
    {
        internal object InternalDbSet<T>() where T : Entity;

        /// <summary>
        /// Contains all notifications (domain events and domain event notifications) from entities being tracked.
        /// </summary>
        IReadOnlyCollection<INotification> Notifications { get; }

        /// <summary>
        /// Clears all notifications found in tracked entities that match the type.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification to clear.</typeparam>
        internal void ClearNotifications<TNotification>() where TNotification : INotification;

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="T"/> type specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="T"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        Task<T?> TryFindAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Tries to return an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        Task<TResult?> TryFindAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Returns an entity of the <typeparamref name="T"/> type specified by the selector.
        /// The result is tracked.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<T> FindAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Returns an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TResult> FindAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="T"/> entities specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="T"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        IAsyncEnumerable<T> FindAllAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TResult"/> anonymous type specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        IAsyncEnumerable<TResult> FindAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity;

        /// <summary>
        /// Adds a collection of domain objects to the data storage that will be inserted
        /// into the database when <see cref="PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="entities">The domain objects collection to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null or empty.</exception>
        Task AddEntityRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Adds a domain object to the data storage that will be inserted
        /// into the database when <see cref="PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Deletes the domain object matching the specified entity that will be removed from the database when <see cref="PersistAsync(CancellationToken)"/>
        /// is called.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="deletedEntity">The entity to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="deletedEntity"/> is null.</exception>
        Task DeleteEntityAsync<T>(T deletedEntity, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Deletes the domain objects matching the predicate that will be removed from the database when <see cref="PersistAsync(CancellationToken)"/>
        /// is called. You can use a third party library for performance.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        Task DeleteEntityAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Updates the domain object matching the specify entity.
        /// </summary>
        /// <typeparam name="T">The Domain object type.</typeparam>
        /// <param name="updatedEntity">the updated entity.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        Task UpdateEntityAsync<T>(T updatedEntity, CancellationToken cancellationToken = default) where T : Entity;

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
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntities"/> is null.</exception>
        Task UpdateEntityRangeAsync<T, TUpdated>(IEnumerable<TUpdated> updatedEntities, CancellationToken cancellationToken = default)
            where T : Entity
            where TUpdated : Entity;

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
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater"/> is null.</exception>
        Task UpdateEntityAsync<T, TUpdated>(Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater, CancellationToken cancellationToken = default)
            where T : Entity
            where TUpdated : class;

        /// <summary>
        /// Persists all pending domain objects to the data storage.
        /// You can use the <see cref="OnPersistenceException"/> to manage exception.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        Task PersistAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Allows to set or unset the delegate that get called on persistence exception.
        /// If you want the exception to be re-thrown, the delegate should return an exception, otherwise null.
        /// To disable the delegate, just set the handler to <see langword="null"/>.
        /// </summary>
        PersistenceExceptionHandler? OnPersistenceException { get; set; }
    }

    /// <summary>
    /// Allows an application author to manage a specific-type domain objects using EntityFrameworkCore.
    /// This interface inherits from <see cref="IDataContext"/>.
    /// When argument is null, an <see cref="ArgumentNullException"/> will be thrown.
    /// When a value is not found, a default value of the expected type should be returned.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public interface IDataContext<TEntity> : IDataContext
        where TEntity : Entity
    {
        internal IDataContext DataContext { get; }

        /// <summary>
        /// Returns an entity of the <typeparamref name="TEntity"/> type specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public async Task<TEntity?> TryFindAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> selector, CancellationToken cancellationToken = default)
            => await DataContext.TryFindAsync<TEntity>(selector, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public async Task<TResult?> TryFindAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
         => await DataContext.TryFindAsync(selector, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns an entity of the <typeparamref name="TEntity"/> type specified by the selector.
        /// The result is tracked.
        /// </summary>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TEntity> FindAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> selector, CancellationToken cancellationToken = default)
            => await DataContext.FindAsync<TEntity>(selector, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns an anonymous type of <typeparamref name="TResult"/> specified by the selector.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TResult> FindAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            => await DataContext.FindAsync(selector, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TEntity"/> entities specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TEntity"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public IAsyncEnumerable<TEntity> FindAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> selector, CancellationToken cancellationToken = default)
            => DataContext.FindAllAsync<TEntity>(selector, cancellationToken);

        /// <summary>
        /// Returns an asynchronous enumerable of <typeparamref name="TResult"/> anonymous type specified by the selector.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="selector">Expression used for selecting entities.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public IAsyncEnumerable<TResult> FindAllAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector, CancellationToken cancellationToken = default)
            => DataContext.FindAllAsync(selector, cancellationToken);

        /// <summary>
        /// Adds a collection of domain objects to the data storage that will be inserted
        /// into the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <param name="entities">The domain objects collection to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null or empty.</exception>
        public async Task AddEntityRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => await DataContext.AddEntityRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Adds a domain object to the data storage that will be inserted
        /// into the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/> is called.
        /// </summary>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        public async Task AddEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await DataContext.AddEntityAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Deletes the domain object matching the specified entity that will be removed from the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/>
        /// is called.
        /// </summary>
        /// <param name="deletedEntity">The entity to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="deletedEntity"/> is null.</exception>
        public async Task DeleteEntityAsync(TEntity deletedEntity, CancellationToken cancellationToken = default)
          => await DataContext.DeleteEntityAsync(deletedEntity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Deletes the domain objects matching the predicate that will be removed from the database when <see cref="IDataContext.PersistAsync(CancellationToken)"/>
        /// is called. You can use a third party library for performance.
        /// </summary>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public async Task DeleteEntityAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
         => await DataContext.DeleteEntityAsync(predicate, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Updates the domain object matching the specify entity.
        /// </summary>
        /// <param name="updatedEntity">the updated entity.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        public async Task UpdateEntityAsync(TEntity updatedEntity, CancellationToken cancellationToken = default)
           => await DataContext.UpdateEntityAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Updates the domain objects matching the collection of entities.
        /// Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone. If you have property you want to set to its default,
        /// then you must explicitly set that property's value.
        /// </summary>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedEntities">Contains the collection of updated values.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntities"/> is null.</exception>
        public async Task UpdateEntityRangeAsync<TUpdated>(IEnumerable<TUpdated> updatedEntities, CancellationToken cancellationToken = default)
            where TUpdated : Entity
            => await DataContext.UpdateEntityRangeAsync<TEntity, TUpdated>(updatedEntities, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Updates the domain objects matching the predicate by using the updater.
        /// Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone. If you have property you want to set to its default,
        /// then you must explicitly set that property's value.
        /// </summary>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="updater">The delegate to be used for updating domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater"/> is null.</exception>
        public async Task UpdateEntityAsync<TUpdated>(Expression<Func<TEntity, bool>> predicate, Func<TEntity, TUpdated> updater, CancellationToken cancellationToken = default)
            where TUpdated : class
            => await DataContext.UpdateEntityAsync(predicate, updater, cancellationToken).ConfigureAwait(false);
    }
}