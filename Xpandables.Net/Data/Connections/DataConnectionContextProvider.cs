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
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using Xpandables.Net.Data.Providers;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Provides with the <see cref="DbConnection"/> from the <see cref="DbProviderFactory"/> and the <see cref="IDataConnection"/>.
    /// </summary>
    public sealed class DataConnectionContextProvider : IDataConnectionContextProvider
    {
        private readonly ConcurrentDictionary<string, DataConnectionContext> _dbConnectionCache;
        private readonly IDataFactoryProvider _dataFactoryProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionContextProvider"/>.
        /// </summary>
        /// <param name="dataFactoryProvider">The data factory provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataFactoryProvider"/> is null.</exception>
        public DataConnectionContextProvider(IDataFactoryProvider dataFactoryProvider)
        {
            _dataFactoryProvider = dataFactoryProvider ?? throw new ArgumentNullException(nameof(dataFactoryProvider));
            _dbConnectionCache = new ConcurrentDictionary<string, DataConnectionContext>();
        }

        /// <summary>
        /// Provides with a database connection using the provider and the connection string.
        /// </summary>
        /// <param name="dataConnection">The data connection to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        public async Task<DataConnectionContext> GetDataConnectionContextAsync(IDataConnection dataConnection)
        {
            _ = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));

            var connectionString = dataConnection.GetConnectionString();
            if (_dbConnectionCache.TryGetValue(connectionString, out var dataConnectionContext))
                return dataConnectionContext;

            var dbFactoryProvider = _dataFactoryProvider.GetProviderFactory(dataConnection.ProviderType)
                ?? throw new ArgumentNullException(nameof(dataConnection.ProviderType));

            var connection = await BuildConnectionAsync(dbFactoryProvider, dataConnection).ConfigureAwait(false);

            return _dbConnectionCache.GetOrAdd(connectionString, new DataConnectionContext(connection, dbFactoryProvider));
        }

        private static async Task<DbConnection> BuildConnectionAsync(DbProviderFactory dbProviderFactory, IDataConnection dataConnection)
        {
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = dataConnection.GetConnectionString();
            await dbConnection.OpenAsync().ConfigureAwait(false);
            await SpeedSqlServerResultAsync(dbConnection).ConfigureAwait(false);
            return dbConnection;
        }

        private static async Task SpeedSqlServerResultAsync(DbConnection connection)
        {
            if (connection.IsSqlConnection())
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"
                    SET ANSI_NULLS ON
                    SET ANSI_PADDING ON
                    SET ANSI_WARNINGS ON
                    SET ARITHABORT ON
                    SET CONCAT_NULL_YIELDS_NULL ON
                    SET QUOTED_IDENTIFIER ON
                    SET NUMERIC_ROUNDABORT OFF";

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
