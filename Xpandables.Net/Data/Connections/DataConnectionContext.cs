
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
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Contains information of <see cref="Connection"/> and <see cref="DbProviderFactory"/>
    /// </summary>
    public sealed class DataConnectionContext : Disposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionContext"/>.
        /// </summary>
        /// <param name="dbConnection">The connection to the data base.</param>
        /// <param name="dbProviderFactory">The data base provider factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dbProviderFactory"/> is null.</exception>
        public DataConnectionContext(DbConnection dbConnection, DbProviderFactory dbProviderFactory)
        {
            Connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            DbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
            Command = Connection.CreateCommand();
        }

        /// <summary>
        /// Makes a copy of the current instance.
        /// </summary>
        /// <param name="source">The data connection to be copied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public DataConnectionContext(DataConnectionContext source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            Connection = source.Connection;
            DbProviderFactory = source.DbProviderFactory;
            Command = source.Command;
        }

        /// <summary>
        /// Gets the connection to the data base.
        /// </summary>
        public DbConnection Connection { get; }

        /// <summary>
        /// Get the Database command instance.
        /// </summary>
        public DbCommand Command { get; }

        /// <summary>
        /// Gets the data base provider factory.
        /// </summary>
        public DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// Opens the current data connection.
        /// </summary>
        /// <param name="commandType">The target command type.</param>
        public async Task InitializeAsync(CommandType commandType)
        {
            Command.CommandType = commandType;
            if (Connection.State != ConnectionState.Open)
                await Connection.OpenAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Initializes the database transaction if necessary.
        /// </summary>
        /// <param name="dataOptions">The data options.</param>
        public async Task<DbTransaction?> InitializeTransactionAsync(IDataExecutableOptions dataOptions)
        {
            if (dataOptions.IsTransactionEnabled)
            {
                var transaction = await Connection.BeginTransactionAsync(dataOptions.IsolationLevel, dataOptions.CancellationToken).ConfigureAwait(false);
                Command.Transaction = transaction;
                return transaction;
            }

            return default;
        }

        private bool isDisposed;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                Command.Dispose();
                Connection.Dispose();
            }

            isDisposed = true;
        }

        /// <summary>
        /// Asynchronously disposes the connection.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected override async ValueTask DisposeAsync(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                await Command.DisposeAsync().ConfigureAwait(false);
                await Connection.DisposeAsync().ConfigureAwait(false);
            }

            isDisposed = true;
        }
    }
}
