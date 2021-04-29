
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

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Entities;
using Xpandables.Net.Expressions;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="IEntityAccessor{TEntity}"/> for EFCore.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class EntityAccessorEFCore<TEntity> : IEntityAccessor<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the current data context.
        /// </summary>
        protected readonly DataContextEFCore Context;

        /// <summary>
        /// Gets the current DbSet of the entity type.
        /// </summary>
        protected IQueryable<TEntity> Entities { get; set; }

        internal readonly DbSet<TEntity> _entities;

        /// <summary>
        /// Initializes a new instance of <see cref="EntityAccessorEFCore{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <exception cref="ArgumentException">The <paramref name="dataContext"/> must derive from <see cref="EntityAccessorEFCore{TEntity}"/>.</exception>
        protected EntityAccessorEFCore(IDataContext dataContext)
        {
            Context = dataContext as DataContextEFCore ?? throw new ArgumentException($"Derived {nameof(DataContextEFCore)} expected.");
            Entities = Context.Set<TEntity>();
            _entities = Context.Set<TEntity>();
        }

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public virtual async Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await Context.TryFindAsync<TEntity, TEntity>(_ =>
                Entities
                    .Where(criteria)
                    .Select(s => s),
                cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="select"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="select">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public virtual async Task<TResult?> TryFindAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> select, CancellationToken cancellationToken = default)
            => await Context.TryFindAsync<TEntity, TResult>(_ =>
                Entities
                    .Where(criteria)
                    .Select(select),
                cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="select">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="select"/> is null.</exception>
        public virtual IAsyncEnumerable<TResult> FetchAllAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> select, CancellationToken cancellationToken = default)
            => Context.FetchAllAsync<TEntity, TResult>(_ =>
                _entities
                    .Where(criteria)
                    .Select(select),
                cancellationToken);

        /// <summary>
        /// Marks the specified entity to be inserted into the database when <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await Context.InsertAsync(
                entity,
                cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// Marks the specified entity to be updated into the database when <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="entity">The entity to be updated and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await Context.UpdateAsync(
                entity,
                cancellationToken)
            .ConfigureAwait(false);

        /// <summary>
        /// Marks the domain objects matching the predicate as deleted and will be removed when <see cref="IDataContextPersistence.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="predicate">Defines a set of criteria that entity should meet to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => await Context.DeleteAsync(predicate, cancellationToken).ConfigureAwait(false);

        private bool _disposedValue;

        /// <summary>
        /// Applies dispose. Override to customize the behavior.
        /// </summary>
        /// <remarks>Only dispose _dataContext when out of dependency injection process.</remarks>
        /// <param name="disposing">the disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> with <see langword="true"/>.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
