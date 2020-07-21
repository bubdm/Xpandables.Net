
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
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net.Data;

namespace Xpandables.Net.DependencyInjection.Data
{
    /// <summary>
    /// Provides with a method to access a collection of <see cref="DataConnection"/>.
    /// </summary>
    public sealed class DataConnectionAccessor : IDataConnectionAccessor
    {
        private readonly IOptions<DataConnection[]> _dataConnextionOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionAccessor"/> with the configuration value.
        /// </summary>
        /// <param name="dataConnectionOptions">The configuration to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionOptions"/> is null.</exception>
        public DataConnectionAccessor(IOptions<DataConnection[]> dataConnectionOptions)
            => _dataConnextionOptions = dataConnectionOptions ?? throw new ArgumentNullException(nameof(dataConnectionOptions));

        /// <summary>
        /// Returns a collection of <see cref="DataConnection"/> found in the system.
        /// If not found, returns an enumerable empty list.
        /// </summary>
        public IEnumerable<DataConnection> GetDataConnections() => _dataConnextionOptions.Value ?? Enumerable.Empty<DataConnection>();
    }
}
