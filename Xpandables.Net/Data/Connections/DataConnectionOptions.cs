
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
using System.Collections.Generic;

using Xpandables.Net.Data.Providers;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Contains data base connection information. Use <see cref="DataConnectionOptionsBuilder"/> to build connection.
    /// </summary>
    public sealed class DataConnectionOptions : ValueObject, IDataConnectionOptions
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionOptions"/> with the provided properties.
        /// </summary>
        /// <param name="connectionStringSource">The connection string.</param>
        /// <param name="poolName">The pool name.</param>
        /// <param name="providerType">The data base provider type.</param>
        /// <param name="userId">The optional user identifier.</param>
        /// <param name="userPassword">The optional user password.</param>
        /// <param name="useIntegratedSecurity">Enable or not use of integrated security.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionStringSource"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="userId"/> and/or <paramref name="userPassword"/> expected.</exception>
        internal DataConnectionOptions(
            string connectionStringSource,
            string poolName,
            DataProviderType providerType,
            string? userId,
            string? userPassword,
            bool useIntegratedSecurity = false)
        {
            ConnectionStringSource = connectionStringSource ?? throw new ArgumentNullException(nameof(connectionStringSource));
            PoolName = poolName ?? throw new ArgumentNullException(nameof(poolName));
            ProviderType = providerType ?? throw new ArgumentNullException(nameof(providerType));
            UserId = userId;
            UserPassword = userPassword;
            UseIntegratedSecurity = useIntegratedSecurity;

            if (!UseIntegratedSecurity && (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(UserPassword)))
                throw new ArgumentException($"When using integrated security, both '{nameof(userId)}' & '{nameof(userPassword)} must be provided");
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionStringSource { get; }

        /// <summary>
        /// Gets the pool name.
        /// </summary>
        public string PoolName { get; }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public DataProviderType ProviderType { get; }

        /// <summary>
        /// Gets the connection string user identifier.
        /// </summary>
        public string? UserId { get; }

        /// <summary>
        /// Gets the connection string user password.
        /// </summary>
        public string? UserPassword { get; }

        /// <summary>
        /// Gets the value whether or not to use integrated security.
        /// If <see langword="false"/>, you should provide <see cref="UserId"/> and <see cref="UserPassword"/> information.
        /// </summary>
        public bool UseIntegratedSecurity { get; }

        /// <summary>
        /// Provides with the list of components that comprise this class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ConnectionStringSource;
            yield return PoolName;
            yield return ProviderType;
            yield return UserId ?? string.Empty;
            yield return UserPassword ?? string.Empty;
        }
    }
}
