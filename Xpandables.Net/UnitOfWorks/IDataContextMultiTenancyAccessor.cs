
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
    /// Allows use of multiple data context that need to be shared across asynchronous control flows.
    /// </summary>
    public interface IDataContextMultiTenancyAccessor : IEnumerable<KeyValuePair<string, IDataContextMultiTenancy>>
    {
        /// <summary>
        /// Gets the unique identifier of the ambient tenant.
        /// </summary>
        string? TenantName { get; }

        /// <summary>
        /// Returns an instance of the ambient data context matching the <see cref="TenantName"/> for the current scope.
        /// </summary>
        /// <returns><see cref="IUnitOfWork"/> derived class.</returns>
        /// <exception cref="InvalidOperationException">The data context tenant matching the current name has not been registered.</exception>
        /// <exception cref="ArgumentException">The tenant name is null.</exception>
        IDataContext GetDataContext();

        /// <summary>
        /// Returns the data context matching the specified tenant name. If not found returns null.
        /// </summary>
        /// <param name="name">The tenant name to search for.</param>
        /// <returns>The requested data context or null if not present.</returns>
        IDataContext? this[string name] { get; }

        /// <summary>
        /// Sets the ambient tenant name from the specified type name for the current scope.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        void SetTenantName<TDataContext>() where TDataContext : class, IDataContext;

        /// <summary>
        /// Sets the ambient tenant name for the current scope.
        /// </summary>
        /// <param name="name">The name of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        void SetTenantName(string name);
    }
}
