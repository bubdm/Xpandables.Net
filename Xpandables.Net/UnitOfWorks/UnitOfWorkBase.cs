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

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the base implementation of <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <typeparam name="TUnitOfWorkContext">The type of the context.</typeparam>
    public abstract class UnitOfWorkBase<TUnitOfWorkContext> : Disposable, IUnitOfWork
        where TUnitOfWorkContext : class, IUnitOfWorkContext
    {
        /// <summary>
        /// Gets the current <typeparamref name="TUnitOfWorkContext"/> instance.
        /// </summary>
        protected TUnitOfWorkContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBase{TUnitOfWorkContext}"/> class with the factory.
        /// </summary>
        /// <param name="unitOfWorkContextFactory">The factory to create the context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="unitOfWorkContextFactory"/> is null.</exception>
        protected UnitOfWorkBase(IUnitOfWorkContextFactory unitOfWorkContextFactory)
            => Context = unitOfWorkContextFactory.CreateUnitOfWorkContext<TUnitOfWorkContext>();


        private bool _isDisposed;

        ///<inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (disposing)
                {
                    Context?.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        ///<inheritdoc/>
        protected override async ValueTask DisposeAsync(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                await Context.DisposeAsync().ConfigureAwait(false);

                await base.DisposeAsync(disposing).ConfigureAwait(false);
            }
        }

        ///<inheritdoc/>
        public abstract Task<int> PersistAsync(CancellationToken cancellationToken = default);
    }
}
