
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Implementation of <see cref="IDataContextTenantAccessor"/>.
    /// </summary>
    public sealed class DataContextTenantAccessor : IDataContextTenantAccessor
    {
        private readonly IDictionary<string, IDataContextTenant> _dataContextFactories;

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextTenantAccessor"/> class with all the tenant factories.
        /// </summary>
        /// <param name="dataContextFactories">The collection of data context factories.</param>
        public DataContextTenantAccessor(IEnumerable<IDataContextTenant> dataContextFactories)
        {
            _dataContextFactories = dataContextFactories?.ToDictionary(d => d.Name, d => d) ?? new Dictionary<string, IDataContextTenant>();
        }

        /// <summary>
        /// Returns the data context matching the specified tenant name. If not found returns null.
        /// </summary>
        /// <param name="name">The tenant name to search for.</param>
        /// <returns>The requested data context or null if not present.</returns>
        public IDataContext? this[string name] =>
            _dataContextFactories.TryGetValue(name, out var factory) ? factory.Factory() : default;

        /// <summary>
        /// Gets the name of the ambient tenant.
        /// </summary>
        public string? TenantName { get; private set; }

        /// <summary>
        /// Returns an instance of the ambient data context matching the <see cref="TenantName" />.
        /// </summary>
        /// <returns><see cref="IDataContext" /> derived class.</returns>
        /// <exception cref="InvalidOperationException">The data context matching the current has not been registered.</exception>
        /// <exception cref="ArgumentException">The tenant name is null.</exception>
        public IDataContext GetDataContext()
        {
            _ = TenantName ?? throw new ArgumentException("The tenant name has not been set.");

            if (_dataContextFactories.TryGetValue(TenantName, out var factory))
                return factory.Factory();

            throw new InvalidOperationException(
                $"The '{TenantName}' factory has not been registered. Use services.AddXDataContextTenant<{TenantName}>() to register the target factory.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, IDataContextTenant>> GetEnumerator()
        {
            foreach (var pair in _dataContextFactories)
                yield return pair;
        }

        /// <summary>
        /// Sets the ambient tenant name using the type name.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        public void SetTenantName<TDataContext>()
            where TDataContext : class, IDataContext => TenantName = typeof(TDataContext).Name;

        /// <summary>
        /// Sets the ambient tenant name.
        /// </summary>
        /// <param name="name">The name of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> is null.</exception>
        public void SetTenantName(string name) => TenantName = name ?? throw new ArgumentNullException(nameof(name));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
