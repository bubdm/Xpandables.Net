
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Xpandables.Net.Data.Providers;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    ///  Represents a set of values data base connection properties.
    /// </summary>
    public interface IDataConnection
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        string ConnectionStringSource { get; }

        /// <summary>
        /// Gets the pool name.
        /// </summary>
        string PoolName { get; }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        DataProviderType ProviderType { get; }

        /// <summary>
        /// Determines whether or not to use integrated security.
        /// If <see langword="false"/>, you should provide <see cref="UserId"/> and <see cref="UserPassword"/> information.
        /// </summary>
        bool UseIntegratedSecurity { get; }

        /// <summary>
        /// Gets the connection string user identifier when <see cref="UseIntegratedSecurity"/> is false.
        /// </summary>
        string? UserId { get; }

        /// <summary>
        /// Gets the connection string user password when <see cref="UseIntegratedSecurity"/> is false.
        /// </summary>
        string? UserPassword { get; }

        /// <summary>
        /// Returns the built connection string value.
        /// </summary>
        /// <exception cref="ArgumentException">The instance is invalid.</exception>
        public string GetConnectionString()
        {
            ValidateConnection();

            return UseIntegratedSecurity
                ? ConnectionStringSource!
                : $"{ConnectionStringSource}User Id={UserId};Password={UserPassword};";
        }

        private void ValidateConnection()
        {
            if (!IsValid(out var exception))
                throw new ArgumentException($"The current {GetType().Name} contains invalid information.", exception);
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

            if (string.IsNullOrWhiteSpace(ProviderType))
                aggregateMessage.AppendLine(nameof(ProviderType)).Append(delimiter);

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
    }
}