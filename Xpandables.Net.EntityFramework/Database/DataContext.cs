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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Entities;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// This is the <see langword="abstract"/> db context class that inherits from <see cref="DbContext"/>
    /// and implements <see cref="IDataContext"/> and <see cref="IDataContextPersistence"/>.
    /// </summary>
    public abstract partial class DataContext : IDataContext, IDataContextPersistence
    {
        ///<inheritdoc/>
        public virtual IQueryable<T> Query<T>() where T : class, IEntity => Set<T>();

        ///<inheritdoc/>
        public virtual async Task<TResult?> TryFindAsync<T, TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TResult> FetchAllAsync<T, TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>().AsNoTracking()).AsAsyncEnumerable();
        }

        ///<inheritdoc/>
        public virtual async Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IEntity
            => await Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task DeleteAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            foreach (var entity in Set<T>().Where(predicate))
            {
                cancellationToken.ThrowIfCancellationRequested();
                Remove(entity);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task UpdateAsync<T>(T updatedEntity, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            cancellationToken.ThrowIfCancellationRequested();
            Update(updatedEntity);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
               where T : class, IEntity
            => await Set<T>().CountAsync(predicate, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        async Task IDataContextPersistence.SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is DbUpdateException)
            {
                throw new InvalidOperationException("Persistence exception.", exception);
            }
        }
    }
}
