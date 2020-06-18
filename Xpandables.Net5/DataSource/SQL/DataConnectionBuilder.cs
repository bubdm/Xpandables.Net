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

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Allows application author to build <see cref="DataConnection"/>.
    /// </summary>
    public sealed class DataConnectionBuilder
    {
        private string? _connectionStringSource;
        private string? _poolName;
        private string? _providerName;
        private string? _userId;
        private string? _userPassword;
        private bool _useIntegratedSecurity;

        /// <summary>
        /// Returns a new instance of <see cref="DataConnection"/> using registered information.
        /// </summary>
        /// <exception cref="InvalidOperationException">Building the <see cref="DataConnection"/> failed. See inner exception.</exception>
        public DataConnection Build()
        {
            var dataConnection = new DataConnection(_connectionStringSource, _poolName, _providerName, _userId, _userPassword, _useIntegratedSecurity);
            if (!dataConnection.IsValid(out var exception))
                throw new InvalidOperationException("Building the data connection failed. See inner exception", exception);

            return dataConnection;
        }

        /// <summary>
        /// Adds the connection string source (without security information).
        /// </summary>
        /// <param name="connectionStringSource">The connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionStringSource"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="connectionStringSource"/> contains security information.</exception>
        public DataConnectionBuilder AddConnectionString(string connectionStringSource)
        {
            _connectionStringSource = connectionStringSource ?? throw new ArgumentNullException(nameof(connectionStringSource));
            if (connectionStringSource.Contains("User Id") || connectionStringSource.Contains("Password"))
                throw new ArgumentException("Connection String Source contains security information.");
            return this;
        }

        /// <summary>
        /// Adds the pool name to be used.
        /// </summary>
        /// <param name="poolName">The pool name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        public DataConnectionBuilder AddPoolName(string poolName)
        {
            _poolName = poolName ?? throw new ArgumentNullException(nameof(poolName));
            return this;
        }

        /// <summary>
        /// Adds the known provider name.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="providerName"/> is null.</exception>
        public DataConnectionBuilder AddProviderName(string providerName)
        {
            _providerName = providerName ?? throw new ArgumentNullException(nameof(providerName));
            return this;
        }

        /// <summary>
        /// Adds the known provider name from the provider type.
        /// </summary>
        /// <param name="providerType">The provider type to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        public DataConnectionBuilder AddProviderName(DataProviderType providerType)
        {
            _providerName = providerType?.DisplayName ?? throw new ArgumentNullException(nameof(providerType));
            return this;
        }

        /// <summary>
        /// Adds the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userId"/> is null.</exception>
        public DataConnectionBuilder AddUserId(string userId)
        {
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
            return this;
        }

        /// <summary>
        /// Adds the user password.
        /// </summary>
        /// <param name="userPassword">The user password.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userPassword"/> is null.</exception>
        public DataConnectionBuilder AddUserPassword(string userPassword)
        {
            _userPassword = userPassword ?? throw new ArgumentNullException(nameof(userPassword));
            return this;
        }

        /// <summary>
        /// Defines the use of integrated security. The connection string must contains the "Integrated Security=true" expression.
        /// </summary>
        public DataConnectionBuilder EnableIntegratedSecurity()
        {
            _useIntegratedSecurity = true;
            return this;
        }
    }
}
