using Microsoft.Extensions.Options;

using System;
using System.Diagnostics.CodeAnalysis;

using Xpandables.Net.Data;

namespace Xpandables.Samples.Business.Services
{
    public sealed class DataConnectionProvider : IDataConnectionProvider
    {
        private readonly DataConnection _dataConnection;

        public DataConnectionProvider(IOptions<DataConnection> options)
            => _dataConnection = options?.Value ?? throw new ArgumentNullException(nameof(options));

        [return: MaybeNull]
        public DataConnection GetDataConnection() => _dataConnection;
    }
}
