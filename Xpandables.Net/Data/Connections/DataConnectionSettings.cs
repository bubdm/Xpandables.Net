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
using Xpandables.Net.Enumerations;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Contains data base connection information from the appsettings file.
    /// You can use <see cref="DataConnectionBuilder"/> to build data connection instance.
    /// </summary>
    public sealed class DataConnectionSettings : IDataConnection
    {
        /// <summary>
        /// Initializes a default instance of <see cref="DataConnectionSettings"/> class.
        /// </summary>
        public DataConnectionSettings() { }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionStringSource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the pool name.
        /// </summary>
        public string PoolName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; } = null!;

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public DataProviderType ProviderType => EnumerationType.FromDisplayName<DataProviderType>(ProviderName)!;

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
    }
}
