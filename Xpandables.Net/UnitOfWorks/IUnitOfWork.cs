using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Provides with the base unit of work interface.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Persists all pending domain objects to the data storage according to the database provider/ORM.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        Task<int> PersistAsync(CancellationToken cancellationToken = default);
    }
}
