
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
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Provides methods to access the <see cref="DataBaseContext"/>.
    /// </summary>
    public sealed class DataBaseContextAccessor
    {
        private readonly IDataProviderFactoryProvider _dataProviderFactoryProvider;
        private readonly DataConnectionAccessor _dataConnectionAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseContextAccessor"/> with collection of settings.
        /// </summary>
        /// <param name="dataConnectionAccessor">The settings accessor.</param>
        /// <param name="dataProviderFactoryProvider">The db provider factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataProviderFactoryProvider"/> is null.</exception>
        public DataBaseContextAccessor(DataConnectionAccessor dataConnectionAccessor, IDataProviderFactoryProvider dataProviderFactoryProvider)
        {
            _dataConnectionAccessor = dataConnectionAccessor ?? throw new ArgumentNullException(nameof(dataConnectionAccessor));
            _dataProviderFactoryProvider = dataProviderFactoryProvider ?? throw new ArgumentNullException(nameof(dataProviderFactoryProvider));
            if (dataConnectionAccessor is null) throw new ArgumentNullException(nameof(dataConnectionAccessor));
        }

        /// <summary>
        /// Returns the database context matching the pool name settings.
        /// You can use the <see cref="DataConnectionAccessor"/> to find the available settings.
        /// </summary>
        /// <param name="poolName">The target pool name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="poolName"/> not found.</exception>
        public DataBaseContext GetDataBaseContext(string poolName)
        {
            if (poolName is null) throw new ArgumentNullException(nameof(poolName));

            var dataConnections = _dataConnectionAccessor.GetDataConnections();
            if (!dataConnections.Any()) throw new ArgumentException("No database connections found in the configuration settings.");

            var settings = dataConnections.ToDictionary(d => d.PoolName!, d => d);
            if (!settings.TryGetValue(poolName, out var foundSettings))
                throw new ArgumentException($"{poolName} not found in the available collection from configuration settings.");

            return GetDataBaseContext(foundSettings);
        }

        /// <summary>
        /// Returns the database context matching the settings.
        /// You can use the <see cref="DataConnectionAccessor"/> to find the available settings.
        /// </summary>
        /// <param name="connection">The settings to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connection"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="connection"/> is invalid.</exception>
        public DataBaseContext GetDataBaseContext(DataConnection connection) => new DataBaseContext(_dataProviderFactoryProvider, connection);

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType. -or- null if there is no service object of type serviceType.</returns>
        public object GetService(Type serviceType) => _dataProviderFactoryProvider.GetRequiredService(serviceType);
    }
}
