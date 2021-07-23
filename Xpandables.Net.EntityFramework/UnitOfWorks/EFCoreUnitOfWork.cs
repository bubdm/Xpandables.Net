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
using System.Threading.Tasks;
using System.Threading;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the base EFCore implementation of <see cref="IUnitOfWork"/>.
    /// </summary>
    public abstract class EFCoreUnitOfWork<TContext> : UnitOfWork<TContext>
        where TContext : EFCoreContext
    {
        /// <summary>
        /// Constructs a new instance of <see cref="EFCoreUnitOfWork{TContext}"/>.
        /// </summary>
        /// <param name="unitOfWorkContextFactory">The db context factory to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="unitOfWorkContextFactory"/> is null.</exception>
        protected EFCoreUnitOfWork(IUnitOfWorkContextFactory unitOfWorkContextFactory)
            : base(unitOfWorkContextFactory) { }

        ///<inheritdoc/>
        public override async Task<int> PersistAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is DbUpdateException or DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException($"Save changes failed. See inner exception.", exception);
            }
        }

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
    }
}
