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

using Xpandables.Net.Data.Providers;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Allows application author to build <see cref="IDataConnectionOptions"/>.
    /// </summary>
    public sealed class DataConnectionOptionsBuilder
    {
        private string _connectionStringSource = string.Empty;
        private string _poolName = string.Empty;
        private DataProviderType _providerType = DataProviderType.MSSQL;
        private string? _userId;
        private string? _userPassword;
        private bool _useIntegratedSecurity;

        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionOptionsBuilder"/> to build a data connection.
        /// </summary>
        public DataConnectionOptionsBuilder() { }

        /// <summary>
        /// Returns a new <see cref="IDataConnectionOptions"/> using registered information.
        /// </summary>
        /// <exception cref="ArgumentNullException">Connection string, poll name or provider type is null.</exception>
        /// <exception cref="ArgumentException">User identifier and/or user password expected.</exception>
        public IDataConnectionOptions Build()
            => new DataConnectionOptions(_connectionStringSource, _poolName, _providerType, _userId, _userPassword, _useIntegratedSecurity);

        /// <summary>
        /// Adds the connection string source (without security information).
        /// </summary>
        /// <param name="connectionStringSource">The connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionStringSource"/> is null.</exception>
        public DataConnectionOptionsBuilder AddConnectionString(string connectionStringSource)
        {
            _connectionStringSource = connectionStringSource ?? throw new ArgumentNullException(nameof(connectionStringSource));
            return this;
        }

        /// <summary>
        /// Adds the pool name to be used.
        /// </summary>
        /// <param name="poolName">The pool name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        public DataConnectionOptionsBuilder AddPoolName(string poolName)
        {
            _poolName = poolName ?? throw new ArgumentNullException(nameof(poolName));
            return this;
        }

        /// <summary>
        /// Adds the known provider name from the provider type.
        /// </summary>
        /// <param name="providerType">The provider type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        public DataConnectionOptionsBuilder AddProviderType(DataProviderType providerType)
        {
            _providerType = providerType ?? throw new ArgumentNullException(nameof(providerType));
            return this;
        }

        /// <summary>
        /// Adds the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userId"/> is null.</exception>
        public DataConnectionOptionsBuilder AddUserId(string userId)
        {
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
            return this;
        }

        /// <summary>
        /// Adds the user password.
        /// </summary>
        /// <param name="userPassword">The user password.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userPassword"/> is null.</exception>
        public DataConnectionOptionsBuilder AddUserPassword(string userPassword)
        {
            _userPassword = userPassword ?? throw new ArgumentNullException(nameof(userPassword));
            return this;
        }

        /// <summary>
        /// Defines the use of integrated security. The connection string must contains the "Integrated Security=true" expression.
        /// </summary>
        public DataConnectionOptionsBuilder EnableIntegratedSecurity()
        {
            _useIntegratedSecurity = true;
            return this;
        }
    }
}
