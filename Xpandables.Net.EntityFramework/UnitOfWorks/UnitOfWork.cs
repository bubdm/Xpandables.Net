
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;
using System.Threading;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the base EFCore implementation of <see cref="IUnitOfWork"/>.
    /// </summary>
    public abstract class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DataContext
    {
        /// <summary>
        /// Gets the current <typeparamref name="TContext"/> instance.
        /// </summary>
        protected TContext Context { get; }


        /// <summary>
        /// Constructs a new instance of <see cref="UnitOfWork{TContext}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        protected UnitOfWork(TContext context) => Context = context ?? throw new ArgumentNullException(nameof(context));

        ///<inheritdoc/>
        public virtual async Task<int> PersistAsync(CancellationToken cancellationToken)
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

        ///<inheritdoc/>
        public virtual async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}
