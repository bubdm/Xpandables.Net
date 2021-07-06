
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

using Xpandables.Net.Entities;
using Xpandables.Net.Expressions;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// An implementation of <see cref="IEntityAccessor{TEntity}"/> for EFCore.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class EntityAccessor<TEntity> : IEntityAccessor<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IDataContext _context;
        private readonly IQueryable<TEntity> _queryable;

        /// <summary>
        /// Initializes a new instance of <see cref="EntityAccessor{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <exception cref="ArgumentException">The <paramref name="context"/> must derive from <see cref="EntityAccessor{TEntity}"/>.</exception>
        public EntityAccessor(IDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _queryable= _context.Query<TEntity>();
        }

        ///<inheritdoc/>
        public virtual async Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await _context.TryFindAsync<TEntity, TEntity>(_ =>
                _queryable
                    .Where(criteria)
                    .Select(s => s),
                cancellationToken)
            .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<TResult?> TryFindAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> select, CancellationToken cancellationToken = default)
            => await _context.TryFindAsync<TEntity, TResult>(_ =>
                _queryable
                    .Where(criteria)
                    .Select(select),
                cancellationToken)
            .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TResult> FetchAllAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> select, CancellationToken cancellationToken = default)
            => _context.FetchAllAsync<TEntity, TResult>(_ =>
                _queryable
                    .Where(criteria)
                    .Select(select),
                cancellationToken);

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await _context
                .CountAsync(predicate, cancellationToken)
                .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _context.InsertAsync(
                entity,
                cancellationToken)
            .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _context.UpdateAsync(
                entity,
                cancellationToken)
            .ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => await _context.DeleteAsync(predicate, cancellationToken).ConfigureAwait(false);

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
