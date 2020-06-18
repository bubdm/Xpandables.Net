
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
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace System.Design.SQL
{
    /// <summary>
    /// Provides with a method to access a collection of <see cref="DataConnection"/>.
    /// </summary>
    public sealed class DataConnectionAccessor
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionAccessor"/> with the configuration value.
        /// </summary>
        /// <param name="configuration">The configuration to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration"/> is null.</exception>
        public DataConnectionAccessor(IConfiguration configuration)
            => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Returns a collection of <see cref="DataConnection"/> found in the system.
        /// If not found, returns an enumerable empty list.
        /// </summary>
        public IEnumerable<DataConnection> GetDataConnections()
            => _configuration.GetSection(nameof(DataConnection)).Get<DataConnection[]>() ?? Enumerable.Empty<DataConnection>();
    }
}
