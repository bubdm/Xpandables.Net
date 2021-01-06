
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// An implementation of <see cref="IWriteEntityAccessor{TEntity}"/>.
    /// You must derive from this class to customize its behaviors.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public class WriteEntityAccessor<TEntity> : IWriteEntityAccessor<TEntity>
        where TEntity : Entity
    {
        private readonly IDataContext<TEntity> _dataContext;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="WriteEntityAccessor{TEntity}"/> with the context to act on.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public WriteEntityAccessor(IDataContext<TEntity> dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        /// <summary>
        /// Marks the specified entity to be inserted to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dataContext.AddEntityAsync(entity, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Marks the specified entity to be updated to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dataContext.UpdateEntityAsync(entity, cancellationToken).ConfigureAwait(false);

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
        // ~WriteEntityAccessor()
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
