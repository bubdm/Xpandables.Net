
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

using Xpandables.Net.Expressions;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="IEntityAccessor{TEntity}"/>.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class EntityAccessor<TEntity> : IEntityAccessor<TEntity>
        where TEntity : class
    {
        private bool _disposedValue;

        /// <summary>
        /// The data context instance.
        /// </summary>
        public IDataContext<TEntity> DataContext { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="EntityAccessor{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public EntityAccessor(IDataContext<TEntity> dataContext) =>
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        /// <summary>
        /// Provides with the query selector for the entity type.
        /// You must override this method to customize its behavior.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of <typeparamref name="TEntity"/>.</returns>
        protected virtual IQueryable<TEntity> QueryableEntity() =>
            (IQueryable<TEntity>)DataContext.InternalDbSet<TEntity>();

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria and is tracked for changes.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public async Task<TEntity?> TryFindTrackedAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(criteria).Select(s => s), true, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria applied on <paramref name="propertyExpression"/> and is tracked for changes.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TEntity?> TryFindTrackedAsync<TParam>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, CancellationToken cancellationToken = default)
            where TParam : class
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(s => s), true, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/> and can be tracked for changes.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public async Task<TResult?> TryFindTrackedAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(criteria).Select(converter), true, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria applied on <paramref name="propertyExpression"/>
        /// using the <paramref name="converter"/> and is tracked for changes.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TResult?> TryFindTrackedAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TParam : class
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(converter), true, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria and is not tracked.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public async Task<TEntity?> TryFindUnTrackedAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(criteria).Select(s => s), false, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria applied on <paramref name="propertyExpression"/> and is not tracked.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TEntity?> TryFindUnTrackedAsync<TParam>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, CancellationToken cancellationToken = default)
            where TParam : class
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(s => s), false, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/> and is not tracked.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public async Task<TResult?> TryFindUnTrackedAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(criteria).Select(converter), false, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria applied on <paramref name="propertyExpression"/>
        /// using the <paramref name="converter"/> and is not tracked.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TResult?> TryFindUnTrackedAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TParam : class
            => await DataContext.TryFindAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(converter), false, cancellationToken)
                .ConfigureAwait(false);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria, can be tracked for changes and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public IAsyncEnumerable<TResult> FetchTrackedAllAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            => DataContext.FetchAllAsync(_ => QueryableEntity().Where(criteria).Select(converter), true, cancellationToken);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria, can be tracked for changes and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public virtual IAsyncEnumerable<TResult> FetchTrackedAllAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TParam : class
            => DataContext.FetchAllAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(converter), true,
                cancellationToken);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria, is no tracked and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public virtual IAsyncEnumerable<TResult> FetchUnTrackedAllAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            => DataContext.FetchAllAsync(_ => QueryableEntity().Where(criteria).Select(converter), false,
                cancellationToken);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria, is not tracked and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public virtual IAsyncEnumerable<TResult> FetchUnTrackedAllAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TParam : class
            => DataContext.FetchAllAsync(_ => QueryableEntity().Where(propertyExpression, criteria).Select(converter), false,
                cancellationToken);

        /// <summary>
        /// Marks the specified entity to be inserted to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await DataContext.AddEntityAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Marks the specified entity to be updated to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await DataContext.UpdateEntityAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Persists all pending entities to the data storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        public async Task PersistAsync(CancellationToken cancellationToken = default)
            => await DataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

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
