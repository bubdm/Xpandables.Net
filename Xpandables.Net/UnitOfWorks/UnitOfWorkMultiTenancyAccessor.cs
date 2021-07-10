
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

using Xpandables.Net;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Implementation of <see cref="IUnitOfWorkMultiTenancyAccessor"/>.
    /// </summary>
    public sealed class UnitOfWorkMultiTenancyAccessor : IUnitOfWorkMultiTenancyAccessor
    {
        private readonly IDictionary<string, IUnitOfWorkMultiTenancy> _entityUnitOfWorkFactories;

        /// <summary>
        /// Initializes a new instance of <see cref="UnitOfWorkMultiTenancyAccessor"/> class with all the tenant factories.
        /// </summary>
        /// <param name="entityUnitOfWorkFactories">The collection of unit of work factories.</param>
        public UnitOfWorkMultiTenancyAccessor(IEnumerable<IUnitOfWorkMultiTenancy> entityUnitOfWorkFactories)
        {
            _entityUnitOfWorkFactories = entityUnitOfWorkFactories?.ToDictionary(d => d.Name, d => d) ?? new Dictionary<string, IUnitOfWorkMultiTenancy>();
        }

        ///<inheritdoc/>
        public IUnitOfWork? this[string name] =>
            _entityUnitOfWorkFactories.TryGetValue(name, out var factory) ? factory.Factory() : default;

        ///<inheritdoc/>
        public string? TenantName { get; private set; }

        ///<inheritdoc/>
        public IUnitOfWork GetUnitOfWork()
        {
            _ = TenantName ?? throw new ArgumentException("The tenant name has not been set.");

            if (_entityUnitOfWorkFactories.TryGetValue(TenantName, out var factory))
                return factory.Factory();

            throw new InvalidOperationException(
                $"The '{TenantName}' factory has not been registered. " +
                $"Use services.AddXUnitOfWorkMultiTenancy<{TenantName}>() to register the target factory.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, IUnitOfWorkMultiTenancy>> GetEnumerator()
        {
            foreach (var pair in _entityUnitOfWorkFactories)
                yield return pair;
        }

        ///<inheritdoc/>
        public void SetTenantName<TUnitOfWork>()
            where TUnitOfWork : class, IUnitOfWork => TenantName = typeof(TUnitOfWork).Name;

        ///<inheritdoc/>
        public void SetTenantName(string name) => TenantName = name ?? throw new ArgumentNullException(nameof(name));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
