
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Contains data base connection information. Use <see cref="DataConnectionBuilder"/> to build connection.
    /// </summary>
    public sealed class DataConnection : ValueObject
    {
        /// <summary>
        /// Initializes a default instance of <see cref="DataConnection"/> class.
        /// </summary>
        public DataConnection() { }
        internal DataConnection(
            string? connectionStringSource,
            string? poolName,
            string? providerName,
            string? userId,
            string? userPassword,
            bool useIntegratedSecurity)
        {
            ConnectionStringSource = connectionStringSource;
            PoolName = poolName;
            ProviderName = providerName;
            UserId = userId;
            UserPassword = userPassword;
            UseIntegratedSecurity = useIntegratedSecurity;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string? ConnectionStringSource { get; set; }

        /// <summary>
        /// Gets or sets the pool name.
        /// </summary>
        public string? PoolName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string? ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the connection string user identifier.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the connection string user password.
        /// </summary>
        public string? UserPassword { get; set; }

        /// <summary>
        /// Determines whether or not to use integrated security.
        /// If <see langword="false"/>, you should provide <see cref="UserId"/> and <see cref="UserPassword"/> information.
        /// </summary>
        public bool UseIntegratedSecurity { get; set; }

        /// <summary>
        /// Returns the built connection string value.
        /// </summary>
        /// <exception cref="ArgumentException">The instance is invalid.</exception>
        public string GetConnectionString()
        {
            if (!IsValid(out var exception))
                throw new ArgumentException($"The current {GetType().Name} contains invalid information : {exception!.Message}");

            return UseIntegratedSecurity
                ? ConnectionStringSource!
                : $"{ConnectionStringSource}User Id={UserId};Password={UserPassword};";
        }

        /// <summary>
        /// Returns the built data provider type from the provider name found in the current settings.
        /// </summary>
        /// <exception cref="ArgumentException">The instance is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="ProviderName"/> is not found in the collection of providers.</exception>
        public DataProviderType GetProviderType()
        {
            if (!IsValid(out var exception))
                throw new ArgumentException($"The current {GetType().Name} contains invalid information : {exception!.Message}");

            return EnumerationType.FromDisplayName<DataProviderType>(ProviderName!)
                ?? throw new ArgumentOutOfRangeException($"The {ProviderName} not found in the available collection of providers.");
        }

        /// <summary>
        /// Determines whether or not the instance contains valid information.
        /// If so, returns <see langword="true"/> otherwise returns <see langword="false"/>.
        /// </summary>
        public bool IsValid([NotNullWhen(false)] out Exception? exception)
        {
            exception = default;
            var aggregateMessage = new StringBuilder();
            const string delimiter = " ,";

            if (string.IsNullOrWhiteSpace(ConnectionStringSource))
                aggregateMessage.Append(nameof(ConnectionStringSource)).Append(delimiter);

            if (string.IsNullOrWhiteSpace(PoolName))
                aggregateMessage.Append(nameof(PoolName)).Append(delimiter);

            if (string.IsNullOrWhiteSpace(ProviderName))
                aggregateMessage.AppendLine(nameof(ProviderName)).Append(delimiter);

            if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(UserId))
                aggregateMessage.AppendLine(nameof(UserId)).Append(delimiter);

            if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(UserPassword))
                aggregateMessage.AppendLine(nameof(UserPassword)).Append(delimiter);

            var message = aggregateMessage.ToString();
            if (message.Length > 0)
            {
                exception = new ArgumentNullException(message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// When implemented in derived class, this method will provide the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ConnectionStringSource ?? string.Empty;
            yield return PoolName ?? string.Empty;
            yield return ProviderName ?? string.Empty;
            yield return UserId ?? string.Empty;
            yield return UserPassword ?? string.Empty;
        }
    }
}
