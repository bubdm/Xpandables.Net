
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

namespace Xpandables.Net.EntityFramework
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// This is the generic db context class that implements <see cref="IDataContext{T}"/> for a specific-type of entity.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public sealed class DataContext<TEntity> : IDataContext<TEntity> where TEntity : Entity
    {
        IDataContext IDataContext<TEntity>.DataContext => _dataContext;

        private readonly IDataContext _dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext{T}"/> class
        /// using the original data context to be wrapped.
        /// </summary>
        /// <param name="dataContext">The original data context.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataContext"/> is null.</exception>
        public DataContext(IDataContext dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public async Task<T?> FindAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken)
            where T : Entity => await _dataContext.FindAsync<T>(selector, cancellationToken).ConfigureAwait(false);
        public async Task<TResult?> FindAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken)
            where T : Entity => await _dataContext.FindAsync(selector, cancellationToken).ConfigureAwait(false);
        IAsyncEnumerable<T> IDataContext.FindAllAsync<T>(Func<IQueryable<T>, IQueryable<T>> selector, CancellationToken cancellationToken)
            => _dataContext.FindAllAsync<T>(selector, cancellationToken);
        IAsyncEnumerable<TResult> IDataContext.FindAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken)
            => _dataContext.FindAllAsync(selector, cancellationToken);
        Task IDataContext.AddEntityRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
            => _dataContext.AddEntityRangeAsync(entities, cancellationToken);
        Task IDataContext.AddEntityAsync<T>(T entity, CancellationToken cancellationToken)
            => _dataContext.AddEntityAsync(entity, cancellationToken);
        Task IDataContext.DeleteEntityAsync<T>(T deletedEntity, CancellationToken cancellationToken)
            => _dataContext.DeleteEntityAsync(deletedEntity, cancellationToken);
        Task IDataContext.DeleteEntityAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            => _dataContext.DeleteEntityAsync(predicate, cancellationToken);
        Task IDataContext.UpdateEntityAsync<T>(T updatedEntity, CancellationToken cancellationToken)
            => _dataContext.UpdateEntityAsync(updatedEntity, cancellationToken);
        Task IDataContext.UpdateEntityRangeAsync<T, TUpdated>(IEnumerable<TUpdated> updatedEntities, CancellationToken cancellationToken)
            => _dataContext.UpdateEntityRangeAsync<T, TUpdated>(updatedEntities, cancellationToken);
        Task IDataContext.UpdateEntityAsync<T, TUpdated>(Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater, CancellationToken cancellationToken)
            => _dataContext.UpdateEntityAsync(predicate, updater, cancellationToken);
        Task IDataContext.PersistAsync(CancellationToken cancellationToken) => _dataContext.PersistAsync(cancellationToken);
        PersistenceExceptionHandler? IDataContext.OnPersistenceException
        {
            get => _dataContext.OnPersistenceException;
            set => _dataContext.OnPersistenceException = value;
        }

        void IDisposable.Dispose() => _dataContext.Dispose();
        ValueTask IAsyncDisposable.DisposeAsync() => _dataContext.DisposeAsync();
        object IDataContext.InternalDbSet<T>() => _dataContext.InternalDbSet<T>();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
