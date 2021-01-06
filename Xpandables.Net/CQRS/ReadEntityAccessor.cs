
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
    /// An implementation of <see cref="IReadEntityAccessor{TEntity}"/>.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class ReadEntityAccessor<TEntity> : IReadEntityAccessor<TEntity>
        where TEntity : Entity
    {
        private readonly IDataContext<TEntity> _dataContext;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="ReadEntityAccessor{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public ReadEntityAccessor(IDataContext<TEntity> dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        /// <summary>
        /// Provides with the query selector for the entity type.
        /// The <paramref name="selectConverter"/> is used for the <see cref="SelectAsync{TResult}(Expression{Func{TEntity, bool}}, Expression{Func{TEntity, TResult}}, CancellationToken)"/> method.
        /// You must override this method to customize its behavior.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="criteria">The criteria expression.</param>
        /// <param name="selectConverter">The converter expression.</param>
        /// <returns>An expression used for selecting entities.</returns>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TResult>> QueryableEntity<TResult>(
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TResult>>? selectConverter = default)
            where TResult : class
              => selectConverter is null
                ? entity => (IQueryable<TResult>)entity.Where(criteria).OrderBy(o => o.Id)
                : entity => entity.Where(criteria).OrderBy(o => o.Id).Select(selectConverter);

        /// <summary>
        /// Returns an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/>".
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public async Task<TResult> FindAsync<TResult>(
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TResult : class
            => await _dataContext.FindAsync(QueryableEntity(criteria, converter), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public async Task<TResult?> TryFindAsync<TResult>(
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TResult : class
            => await _dataContext.TryFindAsync(QueryableEntity(criteria, converter), cancellationToken).ConfigureAwait(false);


        /// <summary>
        /// Returns an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// The result must be tracked.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await _dataContext.FindAsync(QueryableEntity<TEntity>(criteria), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public virtual IAsyncEnumerable<TResult> SelectAsync<TResult>(
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TResult : class
            => _dataContext.FindAllAsync(QueryableEntity(criteria, converter), cancellationToken);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        public virtual async Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await _dataContext.TryFindAsync(QueryableEntity<TEntity>(criteria), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Applies dispose. Override to customize the behavior.
        /// </summary>
        /// <remarks>Only dispose _dataContext when out of dependency injection process.</remarks>
        /// <param name="disposing">the disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Only dispose _dataContext when out of dependency injection process.
                    // _dataContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ReadEntityAccessor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

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
