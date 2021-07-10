
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

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Allows use of multiple unit of work that need to be shared across asynchronous control flows.
    /// </summary>
    public interface IUnitOfWorkMultiTenancyAccessor : IEnumerable<KeyValuePair<string, IUnitOfWorkMultiTenancy>>
    {
        /// <summary>
        /// Gets the unique identifier of the ambient tenant.
        /// </summary>
        string? TenantName { get; }

        /// <summary>
        /// Returns an instance of the ambient unit of work matching the <see cref="TenantName"/> for the current scope.
        /// </summary>
        /// <returns><see cref="IUnitOfWork"/> derived class.</returns>
        /// <exception cref="InvalidOperationException">The unit of work tenant matching the current name has not been registered.</exception>
        /// <exception cref="ArgumentException">The tenant name is null.</exception>
        IUnitOfWork GetUnitOfWork();

        /// <summary>
        /// Returns the unit of work matching the specified tenant name. If not found returns null.
        /// </summary>
        /// <param name="name">The tenant name to search for.</param>
        /// <returns>The requested unit of work or null if not present.</returns>
        IUnitOfWork? this[string name] { get; }

        /// <summary>
        /// Sets the ambient tenant name from the specified type name for the current scope.
        /// </summary>
        /// <typeparam name="TUnitOfWork">The type of the unit of work.</typeparam>
        void SetTenantName<TUnitOfWork>() where TUnitOfWork : class, IUnitOfWork;

        /// <summary>
        /// Sets the ambient tenant name for the current scope.
        /// </summary>
        /// <param name="name">The name of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        void SetTenantName(string name);
    }
}
