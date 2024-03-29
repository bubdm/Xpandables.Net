﻿
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
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.Expressions;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity}"/> for EFCore.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the current context instance.
        /// </summary>
        protected virtual Context Context { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Repository{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="context">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public Repository(Context context) => Context = context ?? throw new ArgumentNullException(nameof(context));

        ///<inheritdoc/>
        public virtual async Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => await Context.Set<TEntity>().FirstOrDefaultAsync(criteria, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task<TEntity?> TryFindAsync<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, CancellationToken cancellationToken = default)
            => await Context.Set<TEntity>().OrderBy(orderBy).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TEntity> FetchAllAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default)
            => Context.Set<TEntity>().Where(criteria).AsAsyncEnumerable();

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TEntity> FetchAllAsync<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, CancellationToken cancellationToken = default)
            => Context.Set<TEntity>().OrderBy(orderBy).AsAsyncEnumerable();

        ///<inheritdoc/>
        public virtual IAsyncEnumerable<TEntity> FetchAllAsync<TKey>(
        Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
             => Context.Set<TEntity>()
                 .OrderBy(orderBy)
                 .Skip(pageIndex * pageSize).Take(pageSize)
                .AsAsyncEnumerable();

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => await Context.Set<TEntity>().CountAsync(predicate, cancellationToken).ConfigureAwait(false);

        ///<inheritdoc/>
        public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsNew)
                await Context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            else
                Context.Set<TEntity>().Update(entity);
        }

        ///<inheritdoc/>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await foreach (var entity in Context.Set<TEntity>().Where(predicate).AsAsyncEnumerable())
                Context.Set<TEntity>().Remove(entity);
        }
    }
}
